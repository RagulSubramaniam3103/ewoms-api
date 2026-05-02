using StartUpCompany.CQRSMethod.Queries.UserControlled;

namespace StartUpCompany.Services.UserControlled
{
    public interface IUserLoginServices
    {
        Task<object> UserLogin(UserControlled_QueryCommand userctlcmd);
    }
}
