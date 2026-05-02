using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ChatMessenger_API.ChatHub
{
    public class ChatHub : Hub
    {
        // userId -> connectionId
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            Console.WriteLine($"CONNECTED USER: {userId}");

            if (!string.IsNullOrEmpty(userId))
            {
                _connections[userId] = Context.ConnectionId;
                await Clients.All.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                _connections.TryRemove(userId, out _);

                await Clients.All.SendAsync("UserOffline", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // 💬 SEND MESSAGE (FIXED)
        public async Task SendMessage(string receiver, string message, string sender)
        {
            Console.WriteLine($"➡ SendMessage called");
            Console.WriteLine($"Sender: {sender}");
            Console.WriteLine($"Receiver: {receiver}");
            Console.WriteLine($"Message: {message}");

            if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(message))
                return;

            sender = sender.Trim().ToLower();
            receiver = receiver.Trim().ToLower();

            if (_connections.TryGetValue(receiver, out var receiverConnectionId))
            {
                Console.WriteLine("✔ Receiver found, sending message");

                await Clients.Client(receiverConnectionId)
                    .SendAsync("ReceiveMessage", sender, message);
            }
            else
            {
                Console.WriteLine("❌ Receiver NOT connected");
            }

            await Clients.Caller
                .SendAsync("ReceiveMessage", sender, message);
        }
        // ✍️ TYPING
        public async Task Typing(string receiver)
        {
            var sender = Context.UserIdentifier;

            if (string.IsNullOrEmpty(sender))
                return;

            sender = sender.Trim().ToLower();
            receiver = receiver.Trim().ToLower();

            if (sender == receiver)
                return;

            if (_connections.TryGetValue(receiver, out var connectionId))
            {
                await Clients.Client(connectionId)
                    .SendAsync("UserTyping", sender);
            }
        }
    }
}