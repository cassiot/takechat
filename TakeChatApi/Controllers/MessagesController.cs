using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Take.TakeChat.Api.Models;
using Take.TakeChat.Domain;
using Take.TakeChat.Repository;

namespace Take.TakeChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private IRoomRepository roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        [HttpGet("{id}/messages")]
        public IEnumerable<MessageReturn> Get(string id, string userId)
        {
            var messages = roomRepository.GetMessagesForUser(id, userId);

            var ret = messages.Select(m => new MessageReturn()
            {
                FromUserId = m.FromUserId,
                IsPrivate = m.IsPrivate,
                Text = m.Text
            });

            return ret;
        }

        [HttpPost("{id}/messages")]
        public void Post(string id, [FromBody] MessageReceived messageReceived)
        {
            var message = new Message()
            {
                FromUserId = messageReceived.FromUserId,
                IsPrivate = messageReceived.IsPrivate,
                RoomId = id,
                Text = messageReceived.Text,
                ToUserId = messageReceived.ToUserId
            };

            roomRepository.SaveMessage(message);
        }
    }
}
