using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Take.TakeChat.Domain;
using Take.TakeChat.Models;
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

        [HttpGet]
        public IEnumerable<RoomModel> Get()
        {
            var rooms = roomRepository.GetRooms();

            return rooms.Select(r => new RoomModel()
            {
                Id = r.Id,
                Name = r.Name
            });
        }

        [HttpPost]
        public void Post([FromBody] RoomModel roomModel)
        {
            var room = new Room()
            {
                Name = roomModel.Name
            };

            roomRepository.CreateRoom(room);
        }

        [HttpGet("{id}/users")]
        public IEnumerable<UserModel> GetRoomUsers(string id)
        {
            var users = roomRepository.GetRoomUsers(id);

            var ret = users.Select(u=> new UserModel() 
            {
                Id = u.Id,
                Name = u.Name
            });

            return ret;
        }

        [HttpPost("{id}/users")]
        public void PostUser(string id, [FromBody] UserModel userModel)
        {
            var user = new User()
            {
                Id = userModel.Id,
                Name = userModel.Name
            };

            roomRepository.RegisterUserInRoom(id, user);
        }

        [HttpDelete("{id}/users/{userId}")]
        public void DeleteUser(string id, [FromBody] string userId)
        {
            roomRepository.RemoveUserFromRoom(id, userId);
        }

        [HttpGet("{id}/messages")]
        public IEnumerable<MessageReturnModel> GetMessages(string id, string userId)
        {
            var messages = roomRepository.GetMessagesForUser(id, userId);

            var ret = messages.Select(m => new MessageReturnModel()
            {
                FromUserId = m.FromUserId,
                IsPrivate = m.IsPrivate,
                Text = m.Text
            });

            return ret;
        }

        [HttpPost("{id}/messages")]
        public void PostMessage(string id, [FromBody] MessageReceivedModel messageReceivedModel)
        {
            var message = new Message()
            {
                FromUserId = messageReceivedModel.FromUserId,
                IsPrivate = messageReceivedModel.IsPrivate,
                RoomId = id,
                Text = messageReceivedModel.Text,
                ToUserId = messageReceivedModel.ToUserId
            };

            roomRepository.SaveMessage(message);
        }
    }
}
