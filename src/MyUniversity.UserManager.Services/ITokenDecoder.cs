using System.Collections.Generic;

namespace MyUniversity.UserManager.Services
{
    public interface ITokenDecoder
    {
        public IEnumerable<string> GetUserRoles(string accessToken);
        public string GetHighestUserRole(string accessToken);
        public string GetUserTenantId(string accessToken);
    }
}
