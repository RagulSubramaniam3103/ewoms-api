using Microsoft.AspNetCore.SignalR;

namespace ChatMessenger_API.ChatHub
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext()?.Request.Query["userId"];
        }
    }
}