using System.Threading.Tasks;
using MyUniversity.UserManager.Models.User;

namespace MyUniversity.UserManager.Services
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(RegisterUserModel userModel);
        Task<string> LoginUserAsync(string email, string password);
        Task<UserModel> GetUserAsync(int id);
        Task<UserModel> GetUserAsync(string email);
    }
}
