using System;
using System.Diagnostics;
using System.Linq;
using Take.TakeChat.Domain;
using Take.TakeChat.Repository;
using Xunit;

namespace TakeChatTests
{
    public class ChatTests
    {
        private RoomRepository roomRepository;

        public ChatTests()
        {
            roomRepository = new RoomRepository();
            roomRepository.Reset();
        }

        [Fact]
        public void RoomCreationTest()
        {
            var room = new Room()
            {
                Name = "Test Room",
            };

            roomRepository.CreateRoom(room);

            var rooms = roomRepository.GetRooms();
            Assert.Single(rooms);
        }

        [Fact]
        public void UserEnterRoomTest()
        {
            var room = new Room()
            {
                Name = "Test Room",
            };

            roomRepository.CreateRoom(room);

            var user = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User" };

            roomRepository.RegisterUserInRoom(room.Id, user);

            Assert.Single(room.Users);
        }

        [Fact]
        public void BroadCastMessageInRoom()
        {
            var room = new Room()
            {
                Name = "Test Room",
            };

            roomRepository.CreateRoom(room);

            var user1 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 1" };
            var user2 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 2" };
            var user3 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 3" };

            roomRepository.RegisterUserInRoom(room.Id, user1);
            roomRepository.RegisterUserInRoom(room.Id, user2);
            roomRepository.RegisterUserInRoom(room.Id, user3);

            roomRepository.SaveMessage(new Message() 
            {
                FromUserId = user1.Id,
                IsPrivate = false,
                RoomId = room.Id,
                Text = "Broadcast test",
                ToUserId = ""
            });
            
            var m2 = roomRepository.GetMessagesForUser(room.Id, user2.Id);
            var m3 = roomRepository.GetMessagesForUser(room.Id, user3.Id);
            
            Assert.Single(m2);
            Assert.Single(m3);
        }

        [Fact]
        public void SendMessageToOneUser()
        {
            var room = new Room()
            {
                Name = "Test Room",
            };

            roomRepository.CreateRoom(room);

            var user1 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 1" };
            var user2 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 2" };
            var user3 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 3" };

            roomRepository.RegisterUserInRoom(room.Id, user1);
            roomRepository.RegisterUserInRoom(room.Id, user2);
            roomRepository.RegisterUserInRoom(room.Id, user3);

            roomRepository.SaveMessage(new Message()
            {
                FromUserId = user1.Id,
                IsPrivate = false,
                RoomId = room.Id,
                Text = "Broadcast test",
                ToUserId = user2.Id
            });

            var m2 = roomRepository.GetMessagesForUser(room.Id, user2.Id);
            var m3 = roomRepository.GetMessagesForUser(room.Id, user3.Id);

            Assert.Single(m2);
            Assert.Single(m3);
        }

        [Fact]
        public void SendPrivateMessageToOneUser()
        {
            var room = new Room()
            {
                Name = "Test Room",
            };

            roomRepository.CreateRoom(room);

            var user1 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 1" };
            var user2 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 2" };
            var user3 = new User() { Id = Guid.NewGuid().ToString(), Name = "Test User 3" };

            roomRepository.RegisterUserInRoom(room.Id, user1);
            roomRepository.RegisterUserInRoom(room.Id, user2);
            roomRepository.RegisterUserInRoom(room.Id, user3);

            roomRepository.SaveMessage(new Message()
            {
                FromUserId = user1.Id,
                IsPrivate = true,
                RoomId = room.Id,
                Text = "Broadcast test",
                ToUserId = user2.Id
            });

            var m2 = roomRepository.GetMessagesForUser(room.Id, user2.Id);
            var m3 = roomRepository.GetMessagesForUser(room.Id, user2.Id);

            Assert.Single(m2);
            Assert.Empty(m3);
        }

    }
}
