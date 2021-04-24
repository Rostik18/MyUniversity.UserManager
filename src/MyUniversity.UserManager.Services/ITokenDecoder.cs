using System.Collections.Generic;

namespace MyUniversity.UserManager.Services
{
    public interface ITokenDecoder
    {
        IEnumerable<string> GetUserRoles(string accessToken);
        string GetHighestUserRole(string accessToken);
        string GetUserTenantId(string accessToken);
        int GetUserId(string accessToken);
    }
}
