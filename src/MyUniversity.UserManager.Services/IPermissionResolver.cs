using System.Collections.Generic;
using MyUniversity.UserManager.Repository.Entities.User;

namespace MyUniversity.UserManager.Services
{
    public interface IPermissionResolver
    {
        bool CanUserCreateUserWithRoles(IReadOnlyCollection<RoleEntity> newUserRoles, string accessToken);
        bool CanUserExistWithoutTenant(IEnumerable<RoleEntity> newUserRoles);
        IEnumerable<string> WhichRolesUserHasAccessTo(string accessToken);

        bool CanUserCreateUniversity(string accessToken);
        bool CanUserReadAllUniversities(string accessToken);
        bool CanUserUpdateUniversity(string accessToken);
        bool CanUserDeleteUniversity(string accessToken);
    }
}
