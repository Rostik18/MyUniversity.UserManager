using System.Collections.Generic;
using System.Threading.Tasks;
using MyUniversity.UserManager.Models.User;

namespace MyUniversity.UserManager.Services
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(RegisterUserModel userModel, string accessToken);
        Task<string> LoginUserAsync(string email, string password);
        Task<IEnumerable<UserModel>> GetAllUsersAsync(string accessToken);
        Task<UserModel> GetUserByIdAsync(int id, string accessToken);
    }
}
