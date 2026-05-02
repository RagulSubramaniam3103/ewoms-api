namespace StartUpCompany.CQRSMethod.Queries.UserControlled
{
    public class LoginCommandResponse
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public string? UserLogin { get; set; }
        public string? UserPassword { get; set; }
        public string? Message { get; set; }
        public string? UserRole { get; set; }
    }
}
