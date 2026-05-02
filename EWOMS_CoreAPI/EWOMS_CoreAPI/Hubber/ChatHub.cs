using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EWOMS_CoreAPI.Hubber
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        private readonly UserConnectionManager _online;

        public ChatHub(ApplicationDbContext db, UserConnectionManager online)
        {
            _db = db;
            _online = online;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    _online.Add(userId, Context.ConnectionId);

                    // Mark undelivered messages
                    var undelivered = await _db.EWOMS_ChatMessages
                        .Where(x => x.ReceiverId == userId && !x.IsDelivered)
                        .ToListAsync();

                    foreach (var msg in undelivered)
                    {
                        msg.IsDelivered = true;
                        msg.DeliveredAt = DateTime.Now;
                    }

                    if (undelivered.Any())
                    {
                        await _db.SaveChangesAsync();
                    }

                    // Tell others I'm online
                    await Clients.Others.SendAsync("UserStatusChanged", userId, true);

                    // Tell me who else is online
                    var onlineUsers = _online.GetOnlineUsers();
                    foreach (var otherId in onlineUsers)
                    {
                        if (otherId != userId)
                        {
                            await Clients.Caller.SendAsync("UserStatusChanged", otherId, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception silently or handle it
                Console.WriteLine($"Hub Error: {ex.Message}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                _online.Remove(Context.ConnectionId);

                if (!string.IsNullOrEmpty(userId) && !_online.IsOnline(userId))
                {
                    await Clients.All.SendAsync("UserStatusChanged", userId, false);
                }
            }
            catch { }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string message, string? image = null, string? video = null, string? document = null, string? fileName = null)
        {
            try
            {
                var senderId = Context.UserIdentifier ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
                {
                    Console.WriteLine($"[ChatHub] SendMessage failed: senderId ({senderId}) or receiverId ({receiverId}) is null");
                    return;
                }

                Console.WriteLine($"[ChatHub] Sending message from {senderId} to {receiverId}. Image Length: {(image?.Length ?? 0)}, Video Length: {(video?.Length ?? 0)}, Doc Length: {(document?.Length ?? 0)}");

                var msg = new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = message,
                    Image = image,
                    Video = video,
                    Document = document,
                    FileName = fileName,
                    SentAt = DateTime.Now,
                    IsDelivered = false,
                    IsRead = false
                };

                _db.EWOMS_ChatMessages.Add(msg);
                await _db.SaveChangesAsync();

                // Notify receiver connections
                var receiverConnections = _online.GetConnections(receiverId);
                if (receiverConnections != null && receiverConnections.Any())
                {
                    msg.IsDelivered = true;
                    msg.DeliveredAt = DateTime.Now;
                    await _db.SaveChangesAsync();

                    await Clients.Clients(receiverConnections).SendAsync("ReceiveMessage", msg);
                    Console.WriteLine($"[ChatHub] Message delivered to receiver {receiverId}");
                }

                // Notify all connections of the sender (including the one who sent it)
                var senderConnections = _online.GetConnections(senderId);
                if (senderConnections != null && senderConnections.Any())
                {
                    await Clients.Clients(senderConnections).SendAsync("ReceiveMessage", msg);
                    Console.WriteLine($"[ChatHub] Message echoed back to sender {senderId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatHub] ERROR in SendMessage: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // Rethrow to let SignalR handle the error if needed
            }
        }

        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task SendGroupMessage(int groupId, string message, string? image = null, string? video = null, string? document = null, string? fileName = null)
        {
            try
            {
                var senderId = Context.UserIdentifier ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(senderId)) return;

                var msg = new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = null, // Null because it's a group message
                    GroupId = groupId,
                    Message = message,
                    Image = image,
                    Video = video,
                    Document = document,
                    FileName = fileName,
                    SentAt = DateTime.Now,
                    IsDelivered = true, // Groups don't track delivery per user the same way
                    IsRead = false
                };

                _db.EWOMS_ChatMessages.Add(msg);
                await _db.SaveChangesAsync();

                // Send to group
                await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ChatHub] ERROR in SendGroupMessage: {ex.Message}");
                throw;
            }
        }

        public async Task MarkAsRead(string senderId)
        {
            var receiverId = Context.UserIdentifier ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId)) return;

            var unreadMessages = await _db.EWOMS_ChatMessages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                    msg.ReadAt = DateTime.Now;
                }
                await _db.SaveChangesAsync();

                // Notify the original sender that their messages were read
                var senderConnections = _online.GetConnections(senderId);
                if (senderConnections != null && senderConnections.Any())
                {
                    await Clients.Clients(senderConnections).SendAsync("MessagesRead", receiverId);
                }
            }
        }
    }
}
