using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Take.TakeChat.Models;

namespace TakeChatClient
{
    class Program
    {
        static HttpClientHandler handler = new HttpClientHandler();
        static HttpClient http;

        const string PROMPT = ":> ";

        static string mainMenuInput;
        static UserModel user;

        static IList<RoomModel> rooms = new List<RoomModel>();
        static RoomModel currentRoom;
        static IEnumerable<UserModel> users;

        static JsonSerializerOptions option = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("You must inform the host address");
                Console.WriteLine("Usege: takechatclient.exe http://yourhostaddress");
                Environment.Exit(1);
            }

            http = new HttpClient(handler);
            //http.BaseAddress = new Uri("http://localhost:5000/api/");
            //http.BaseAddress = new Uri("http://localhost:52199/api/");
            
            
            http.BaseAddress = new Uri(args[0] + "/api/");

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

            GetRooms();

            mainMenuInput = Console.ReadLine();

            switch (mainMenuInput)
            {
                case "1": ListRooms(); break;
                case "2": SelectChatRoom(); break;
                case "3": CreateChatRoom(); break;
                case "4": Environment.Exit(0); break;
                default:
                    PrintWrongOption();
                    PrintMainMenu();

                    break;
            }
        }

        static void GetRooms()
        {
            var roomsRequest = http.GetAsync("rooms").Result;

            rooms = JsonSerializer.Deserialize<IList<RoomModel>>(roomsRequest.Content.ReadAsStringAsync().Result, option);
        }

        static void ListRooms()
        {
            GetRooms();

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

        static void GetUsers()
        {
            var usersRequest = http.GetAsync($"rooms/{currentRoom.Id}/users").Result;
            users = JsonSerializer.Deserialize<IEnumerable<UserModel>>(usersRequest.Content.ReadAsStringAsync().Result, option);
        }

        static void ListUsers()
        {
            GetUsers();

            Console.WriteLine("Users in this room:");
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
            {
                Console.WriteLine("Room not found");
                PrintMainMenu();
            }

            currentRoom = room;

            var postReques = http.PostAsync($"rooms/{room.Id}/users", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")).Result;

            Console.WriteLine();
            Console.WriteLine($"Welcome to chat room {room.Name}");
            Console.WriteLine();
            Console.WriteLine($"Type your message and hit <Enter> to send. Type <Esc> to return to main menu");
            Console.WriteLine($"Use [userName]:: to send message to specific user");
            Console.WriteLine($"Use p.[userName]:: to send private message to specific user");

            ListUsers();

            ConsoleKeyInfo input;
            var sb = new StringBuilder();

            var updateTaskCancellation = new CancellationTokenSource();

            ///This task runs in background and update rooms, users and messages periodically
            var updateTask = Task.Factory.StartNew(async () => 
            {
                while (true)
                {
                    GetRooms();
                    GetUsers();
                    PrintNewMessages();

                    await Task.Delay(3000);
                }
            }, updateTaskCancellation.Token);

            while (true)
            {
                input = Console.ReadKey();

                if (input.Key == ConsoleKey.Enter)
                {
                    var text = sb.ToString().Trim();
                    sb.Clear();

                    var msgText = SendMessage(text);

                    Console.WriteLine();
                    Console.WriteLine($"{user.Name}: {msgText}");
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    updateTaskCancellation.Cancel();
                    ExitChatRoom();
                    PrintMainMenu();

                    return;
                }
                else
                    sb.Append(input.KeyChar);
            }
        }
        static IEnumerable<MessageReturnModel> GetMessages()
        {
            var messagesRequest = http.GetAsync($"rooms/{currentRoom.Id}/messages?userId={user.Id}").Result;
            var messages = JsonSerializer.Deserialize<IEnumerable<MessageReturnModel>>(messagesRequest.Content.ReadAsStringAsync().Result, option);

            return messages;
        }

        static void PrintNewMessages()
        {
            var messages = GetMessages();

            foreach (var m in messages)
            {
                var fromUser = users.Where(u => u.Id == m.FromUserId).SingleOrDefault();
                var toUser = users.Where(u => u.Id == m.ToUserId).SingleOrDefault(); 
                
                var toOne = toUser  == null ? "" : "@" + toUser.Name;
                Console.WriteLine($"{fromUser.Name}: {toOne} {m.Text}");
            }
        }

        static string SendMessage(string text)
        {
            string msgText = "";
            UserModel userTo = null;
            var split = text.Split("::");

            if (split.Length > 1)
            {
                //Private messages start with p.
                var userToName = split[0].Replace("p.", "");
                userTo = users.Where(u => u.Name == userToName).SingleOrDefault();
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
                Text = msgText.Trim(),
                ToUserId = userTo == null ? null : userTo.Id
            };

            var r = http.PostAsync($"rooms/{currentRoom.Id}/messages", new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json")).Result;

            return msgText;
        }

        static void ExitChatRoom()
        {
            var deleteRequest = http.DeleteAsync($"rooms/{currentRoom.Id}/users/{user.Id}").Result;
            users = null;
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
