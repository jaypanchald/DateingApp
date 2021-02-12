using AutoMapper;
using Dating.Model.Entity;
using Dating.Model.Message;
using Dating.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DateingApp.API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        private readonly IMapper _mapper;

        public MessageHub(IMessageRepository messageRepository,
            IPhotoRepository photoRepository,
            IUserRepository userRepository,
            IHubContext<PresenceHub> presenceHub,
            PresenceTracker tracker,
            IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _photoRepository = photoRepository;
            _presenceHub = presenceHub;
            _tracker = tracker;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            string otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.Identity.Name, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await _messageRepository.GetMessageThread(Context.User.Identity.Name, otherUser);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messages);
            await Clients.Group(groupName).SendAsync("ReciveMessageThread", messageThread);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(MessageForCreateaDto messageDto)
        {
            var username = Context.User.Identity.Name;
            
            if (username.ToLower() == messageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You cannot send messages to yourself");
            }

            User sernder = await _userRepository.GetUserbyUsername(username);
            User recipient = await _userRepository.GetUserbyUsername(messageDto.RecipientUsername);
        
            if (recipient == null)
            {
                throw new HubException("Not found user.");
            }

            var message = new Message
            {
                Sender = sernder,
                Recipient = recipient,
                Content = messageDto.Content,
                SenderUsername = sernder.UserName,
                SenderId = sernder.Id,
                RecipientUsername = recipient.UserName,
                Recipientid = recipient.Id
            };

            var groupName = GetGroupName(sernder.UserName, recipient.UserName);
            var group = await  _messageRepository.GetMessageGroup(groupName);
            if (group.Connections.Any(a => a.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
                message.IsRead = true;
            }
            else
            {
                List<string> connection = await _tracker.GetConnectionUser(recipient.UserName);
                if (connection != null)
                {
                    await _presenceHub.Clients.Clients(connection)
                        .SendAsync("NewMessageReceived", new {
                            username = sernder.UserName,
                            knownAs = sernder.KnownAs
                        });

                }
            }

            if (await _messageRepository.Insert(message))
            {
                var result = _mapper.Map<MessageToReturnDto>(message);
                if (string.IsNullOrWhiteSpace(result.RecipientPhotoUrl))
                {
                    var photos = await _photoRepository.GetListOfPhotosOfUser(message.Recipientid);
                    if (photos != null && photos.Any())
                    {
                        result.RecipientPhotoUrl = photos.FirstOrDefault(f => f.IsMain).Url;
                    }
                }
                if (string.IsNullOrWhiteSpace(result.SenderPhotoUrl))
                {
                    var photos = await _photoRepository.GetListOfPhotosOfUser(message.SenderId);
                    if (photos != null && photos.Any())
                    {
                        result.SenderPhotoUrl = photos.FirstOrDefault(f => f.IsMain).Url;
                    }
                }

                await Clients.Group(groupName).SendAsync("NewMessage", result);
            }
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.Identity.Name);

            if (group == null)
            {
                group = new Group(groupName);
                await _messageRepository.AddGroup(group);
            }
            else if (group.Connections != null && group.Connections.Any(a => a.Username == Context.User.Identity.Name))
            {
                foreach (var item in group.Connections.Where(a => a.Username == Context.User.Identity.Name))
                {
                    group.Connections.Remove(item);
                }
            }
            group.Connections.Add(connection);

            return await _messageRepository.SaveChangesAsync();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);
            if (connection != null)
            {
                await _messageRepository.RemoveConnection(connection);
            }
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}~{other}" : $"{other}~{caller}";
        }
    }
}
