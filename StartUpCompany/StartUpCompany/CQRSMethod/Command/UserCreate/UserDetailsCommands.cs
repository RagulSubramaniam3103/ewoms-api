namespace StartUpCompany.CQRSMethod.Command.UserCreate
{
    public class UserDetailsCommands
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }
    public enum UserType
    {
        Admin,
        Staff,
        Student
    }
}
