using System.Threading.Tasks;
using MyUniversity.UserManager.Models.User;

namespace MyUniversity.UserManager.Services
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(RegisterUserModel userModel, string accessToken);
        Task<string> LoginUserAsync(string email, string password);
    }
}
