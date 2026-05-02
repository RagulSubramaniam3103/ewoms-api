using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public string SenderId { get; set; }
        public string? ReceiverId { get; set; }

        public int? GroupId { get; set; }

        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        // 🟡 DELIVERY STATUS
        public bool IsDelivered { get; set; } = false;
        public DateTime? DeliveredAt { get; set; }

        // 🔵 READ STATUS
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // 🖼️ IMAGE SUPPORT
        public string? Image { get; set; }

        // 🎥 VIDEO SUPPORT
        public string? Video { get; set; }

        // 📄 DOCUMENT SUPPORT
        public string? Document { get; set; }
        public string? FileName { get; set; }
    }
}
