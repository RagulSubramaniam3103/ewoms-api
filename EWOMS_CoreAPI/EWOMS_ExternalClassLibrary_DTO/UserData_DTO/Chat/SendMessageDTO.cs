namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO.Chat
{
    public class SendMessageDTO
    {
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public string? Image { get; set; }
        public string? Video { get; set; }
        public string? Document { get; set; }
        public string? FileName { get; set; }
    }
}
