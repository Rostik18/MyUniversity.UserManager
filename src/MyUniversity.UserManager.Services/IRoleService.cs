using System.Collections.Generic;
using System.Threading.Tasks;
using MyUniversity.UserManager.Models.Roles;

namespace MyUniversity.UserManager.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetRolesAsync(string accessToken);
    }
}
