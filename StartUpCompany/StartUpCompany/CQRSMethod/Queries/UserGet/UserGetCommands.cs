using MediatR;

namespace StartUpCompany.CQRSMethod.Queries.UserGet
{
    public class UserGetCommands : IRequest<Object>
    {
        public string Email { get; set; } = "test@gmail.com";
        public string Password { get; set; } = "Test@1234";
    }
}
