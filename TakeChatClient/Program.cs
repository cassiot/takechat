using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Take.TakeChat.Models;

namespace TakeChatClient
{
    class Program
    {
        static HttpClient http = new HttpClient();

        static string mainMenuInput;
        static UserModel user;

        static void Main(string[] args)
        {
            http.BaseAddress = new Uri("http://localhost");

            Console.WriteLine("Welcome to Take Chat!");

            Console.WriteLine();

            Console.WriteLine("Type your name: ");
            var userName = Console.ReadLine().Trim();

            user = new UserModel()
            {
                Id = Guid.NewGuid().ToString(),
                Name = userName

            };

            Console.WriteLine();

            PrintMainMenu();
        }

        static void PrintMainMenu()
        {
            Console.WriteLine("1: List rooms");
            Console.WriteLine("2: Select room");
            Console.WriteLine("3: Create room");
            Console.WriteLine("4: Exit");

            mainMenuInput = Console.ReadLine();

            switch (mainMenuInput)
            {
                case "1": break;
                case "2": break;
                case "3": break;
                case "4": break;
                default:
                    PrintWrongOption();
                    PrintMainMenu();

                    break;
            }
        }
        static void ListRooms()
        {
            var roomsRequest = http.GetAsync("rooms").Result;
            var rooms = JsonSerializer.Deserialize<IEnumerable<RoomModel>>(roomsRequest.Content.ReadAsStringAsync().Result);

            Console.WriteLine("Rooms");
            Console.WriteLine();

            foreach (var r in rooms)
            {
                Console.WriteLine(r.Name);
            }

            Console.WriteLine();
        }

        static void ListUsers(string roomId)
        {
            var usersRequest = http.GetAsync($"rooms/{roomId}/users").Result;
            var users = JsonSerializer.Deserialize<IEnumerable<UserModel>>(usersRequest.Content.ReadAsStringAsync().Result);

            Console.WriteLine("Users in this room");
            Console.WriteLine();

            foreach (var u in users)
            {
                Console.WriteLine(u.Name);
            }

            Console.WriteLine();
        }

        static void EnterChatRoom(RoomModel room)
        {
            http.PostAsync($"rooms/{room.Id}/users", new StringContent(JsonSerializer.Serialize(user)));

            Console.WriteLine($"Welcome to chat room {room.Name}");
            Console.WriteLine();
            Console.WriteLine($"Type your message and hit <Enter> to send. Type <Esc> to return to main menu");
            Console.WriteLine($"[userName]:: to send message to specific user");
            Console.WriteLine($"Use p.[userName]:: to send private message to specific user");

            ListUsers(room.Id);

            ConsoleKeyInfo input;
            var sb = new StringBuilder();

            while (true)
            {
                input = Console.ReadKey();

                if (input.Key != ConsoleKey.Enter)
                {
                    sb.Append(input.KeyChar);
                    break;
                }
                else if (input.Key != ConsoleKey.Escape)
                {
                    ExitChatRoom(room);
                    PrintMainMenu();
                    return;
                }
            }

            var text = sb.ToString().Trim();
            string msgText = "";
            string userTo = "";
            var split = text.Split("::");

            if (split.Length > 0)
            {
                userTo = split[0].Replace("p.", "");
                msgText = split[1];
            }
            else
            {
                msgText = split[0];
            }

            var message = new MessageReceivedModel()
            {
                FromUserId = user.Id,
                IsPrivate = text.StartsWith("p"),
                Text = msgText,
                ToUserId = userTo
            };

            http.PostAsync($"rooms/{room.Id}/messages", new StringContent(JsonSerializer.Serialize(message)));

            Console.WriteLine();
        }

        static void ExitChatRoom(RoomModel room)
        {
            http.DeleteAsync($"rooms/{room.Id}/users/{new StringContent(user.Id)}");
        }

        static string CreateRoom(string roomName)
        {
            var room = new RoomModel() 
            {
                Name = roomName
            };

            var id = http.PostAsync($"rooms", new StringContent(JsonSerializer.Serialize(room))).Result.Content.ReadAsStringAsync().Result;

            return id;
        }

        static void PrintWrongOption()
        {
            Console.WriteLine("Wrong option. Try again");
            Console.WriteLine();
        }
    }
}
