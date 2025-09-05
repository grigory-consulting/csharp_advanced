using System;

// ChatRoom 

namespace Exercises
{
    public class MessageEventsArgs : EventArgs
    {
        public string User { get; }
        public string Message { get; }
        public MessageEventsArgs(string user, string message) { User = user; Message = message; }

    }

    public class ChatRoom
    {
        public event EventHandler<MessageEventsArgs>? MessageReceived;

        public void Send(string user, string message)
        {
            Console.WriteLine($"{user} says {message}");
            MessageReceived?.Invoke(this, new MessageEventsArgs(user, message));
        }
    }

    public class ChatUser
    {
        private readonly string _name;
        public ChatUser(string name) => _name = name;

        public void Subsribe(ChatRoom room)
        {
            room.MessageReceived += (s, e) =>
            {
                if (e.User != _name)
                {
                    Console.WriteLine($"{_name} received {e.Message}");
                }

            };
        }

        public static class ProgramEventEx2
        {
            public static void Main(string[] args)
            {

                var room = new ChatRoom();
                var user1 = new ChatUser("yoda");
                var user2 = new ChatUser("dennis");

                user1.Subsribe(room);
                user2.Subsribe(room);

                room.Send("user1", "Hi, user2!");
                room.Send("user2", "Hallo, user1!");

            }

        }
    }
}