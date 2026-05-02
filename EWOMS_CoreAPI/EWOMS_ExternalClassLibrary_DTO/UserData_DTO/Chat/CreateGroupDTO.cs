namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO.Chat
{
    public class CreateGroupDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<string>? MemberIds { get; set; }
    }
}
