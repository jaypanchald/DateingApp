using AutoMapper;
using DateingApp.API.Helper;
using Dating.Model.Entity;
using Dating.Model.Helper;
using Dating.Model.Message;
using Dating.Repository.PagedList;
using Dating.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DateingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IMapper _mapper;

        public MessageController(IMessageRepository messageRepository,
            IUserRepository userRepository,
            IPhotoRepository photoRepository,
            IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _photoRepository = photoRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(MessageForCreateaDto messageDto)
        {
            var username = User.Identity.Name;
            if (username.ToLower() == messageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send message to your self.");
            }

            User sernder = await _userRepository.GetUserbyUsername(username);
            User recipient = await _userRepository.GetUserbyUsername(messageDto.RecipientUsername);

            if (recipient == null)
            {
                return NotFound();
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
                return Ok(result);
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult> GetMessagesForUser(
           [FromQuery] MessageParams param)
        {
            param.Username = User.Identity.Name;

            PagedList<Message> messagFromRepo = await _messageRepository.GetMessagesForUser(param);

            IEnumerable<MessageToReturnDto> messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagFromRepo);
            var result = new PagedList<MessageToReturnDto>(messages.ToList(), messagFromRepo.TotalCount, messagFromRepo.TotalPages, messagFromRepo.PageSize);

            Response.AddPagination(param.PageNumber, result.PageSize,
                result.TotalCount, result.TotalPages);

            return Ok(result);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult> GetMessageThread(string username)
        {
            var messagFromRepo = await _messageRepository.GetMessageThread(User.Identity.Name, username);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagFromRepo);

            return Ok(messageThread);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.Identity.Name;
            var message = await _messageRepository.GetMessage(id);

            if (message.Sender.UserName != username 
                && message.Recipient.UserName != username)
            {
                return Unauthorized();
            }

            if (message.Sender.UserName == username)
                message.SenderDeleted = true;

            if (message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                if (await _messageRepository.Delete(message))
                {
                    return Ok();
                }
            }
            else if (message.SenderDeleted || message.RecipientDeleted)
            {
                return Ok();
            }

            return BadRequest("Problem deleteding the message.");
        }

        [HttpPost("DeleteMessage/{userId}/{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _messageRepository.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }
            else if (messageFromRepo.Recipientid == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                if (await _messageRepository.Delete(messageFromRepo))
                {
                    return NoContent();
                }
            }
            else
            {
                if (await _messageRepository.Update(messageFromRepo))
                {
                    return NoContent();
                }
            }

            throw new Exception("Error deleting the message.");
        }

        
        [HttpPost("markMessageRead/{userId}/{id}")]
        public async Task<IActionResult> MarkMessageRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _messageRepository.GetMessage(id);

            if (messageFromRepo.Recipientid != userId)
            {
                return Unauthorized();
            }

            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.UtcNow;

            if (await _messageRepository.Update(messageFromRepo))
            {
                return NoContent();
            }

            throw new Exception("");
        }

    }
}
