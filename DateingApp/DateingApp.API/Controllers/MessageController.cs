using AutoMapper;
using DateingApp.API.Helper;
using DatingApp.Model.Entity;
using DatingApp.Model.Helper;
using DatingApp.Model.Message;
using DatingApp.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly IMapper _mapper;

        public MessageController(IMessageRepository messageRepository,
        IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpGet("{userId}/{id}", Name = "GetMessage")]
        public async Task<ActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            Message messagFromRepo = await _messageRepository.GetMessage(id);

            if (messagFromRepo == null)
            {
                return NotFound();
            }

            MessageToReturnDto messageToReturn = _mapper.Map<MessageToReturnDto>(messagFromRepo);

            return Ok(messageToReturn);
        }


        [HttpGet("GetMessagesForUser/{userId}")]
        public async Task<ActionResult> GetMessagesForUser(int userId,
            [FromQuery] MessageParams param)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            param.UserId = userId;

            var messagFromRepo = await _messageRepository.GetMessagesFprUser(param);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagFromRepo);

            Response.AddPagination(messagFromRepo.CurrentPage, messagFromRepo.PageSize,
                messagFromRepo.TotalCount, messagFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{userId}/{recipientId}")]
        public async Task<ActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagFromRepo = await _messageRepository.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagFromRepo);

            return Ok(messageThread);
        }


        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateMessage(int userId,
            MessageForCreateaDto messageDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageDto.SenderId = userId;

            var message = _mapper.Map<Message>(messageDto);

            if (await _messageRepository.Insert(message))
            {
                message = await _messageRepository.GetMessageThread(message.Id);

                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);

                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }

            throw new Exception("Create the message failed on save. ");
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
