using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Take.TakeChat.Models;

namespace TakeChatClient
{
    class Program
    {
        static HttpClientHandler handler = new HttpClientHandler();
        static HttpClient http;// = new HttpClient(handler);

        const string PROMPT = ":> ";

        static string mainMenuInput;
        static UserModel user;

        static IList<RoomModel> rooms;
        static RoomModel currentRoom;
        static JsonSerializerOptions option = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        static void Main(string[] args)
        {
            http = new HttpClient(handler);
            //http.BaseAddress = new Uri("http://localhost:5000/api/");
            http.BaseAddress = new Uri("http://localhost:52199/api/");

            Console.WriteLine("Welcome to Take Chat!");

            Console.WriteLine();

            Console.Write($"Type your name {PROMPT}");
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
            Console.WriteLine("*** MAIN MENU ***");
            Console.WriteLine();
            Console.WriteLine("1: List rooms");
            Console.WriteLine("2: Select room");
            Console.WriteLine("3: Create room");
            Console.WriteLine("4: Exit");
            Console.WriteLine();
            Console.Write(PROMPT);

            mainMenuInput = Console.ReadLine();

            switch (mainMenuInput)
            {
                case "1": ListRooms(); break;
                case "2": SelectChatRoom(); break;
                case "3": CreateChatRoom(); break;
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

            rooms = JsonSerializer.Deserialize<IList<RoomModel>>(roomsRequest.Content.ReadAsStringAsync().Result, option);

            Console.WriteLine();
            Console.Write("Rooms:");
            Console.WriteLine();

            foreach (var r in rooms)
            {
                Console.WriteLine(r.Name);
            }

            Console.WriteLine();

            PrintMainMenu();
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

        static void SelectChatRoom()
        {
            Console.WriteLine();
            Console.Write($"Room name {PROMPT}");
            var roomName = Console.ReadLine();
            var room = rooms.SingleOrDefault(r => r.Name == roomName);

            if (room == null)
                PrintMainMenu();

            currentRoom = room;

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
                    ExitChatRoom();
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

        static void ExitChatRoom()
        {
            http.DeleteAsync($"rooms/{currentRoom.Id}/users/{new StringContent(user.Id)}");

            PrintMainMenu();
        }

        static void CreateChatRoom()
        {
            Console.WriteLine();
            Console.Write($"Room name {PROMPT}");
            
            var roomName = Console.ReadLine();

            var request = http.PostAsync($"rooms?roomName={roomName}", null).Result;
            var id = request.Content.ReadAsStringAsync().Result;

            rooms.Add(new RoomModel() { Id = id, Name = roomName });

            Console.WriteLine();
            PrintMainMenu();
        }

        static void PrintWrongOption()
        {
            Console.WriteLine("Wrong option. Try again");
            Console.WriteLine();
        }
    }
}
