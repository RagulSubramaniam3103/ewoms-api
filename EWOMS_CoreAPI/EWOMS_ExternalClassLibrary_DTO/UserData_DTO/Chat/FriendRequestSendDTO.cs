using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO.Chat
{
    public class FriendRequestSendDTO
    {
        public string ReceiverId { get; set; }
    }
    public class FriendRequestDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public enum FriendRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
