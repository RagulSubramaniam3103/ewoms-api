namespace IdentityWebAPI_User.MainModel.UserEndModel
{
    public class CustomerRegister
    {
        public string UserName { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
    public class CustomerLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
