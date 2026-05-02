using LoginIdentityWebAPI.UserControlled;

namespace LoginIdentityWebAPI.Services
{
    public interface IBridgeservices
    {
        Task<bool> RegisterUserAsync(UserMainDetails userMain);
    }
}
