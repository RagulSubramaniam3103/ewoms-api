using EWOMS_ExternalClassLibrary_DTO.UserData_DTO.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration.ChatMessenger
{
    public class FriendRequests
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public FriendRequestStatus Status { get; set; }
    }
}
