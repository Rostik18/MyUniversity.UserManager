using System;
using System.Collections.Generic;
using System.Linq;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Repository.Entities.User;

namespace MyUniversity.UserManager.Services.Implementation
{
    public class PermissionResolver : IPermissionResolver
    {
        private readonly ITokenDecoder _tokenDecoder;

        public PermissionResolver(ITokenDecoder tokenDecoder)
        {
            _tokenDecoder = tokenDecoder;
        }

        #region User

        public bool CanUserCreateUserWithRoles(IReadOnlyCollection<RoleEntity> newUserRoles, string accessToken)
        {
            var highestUserRole = _tokenDecoder.GetHighestUserRole(accessToken);

            switch (highestUserRole)
            {
                case RolesConstants.SuperAdmin:
                case RolesConstants.Service:
                case RolesConstants.UniversityAdmin
                    when newUserRoles.All(x => x.Role != RolesConstants.SuperAdmin) &&
                         newUserRoles.All(x => x.Role != RolesConstants.Service):
                case RolesConstants.Teacher
                    when newUserRoles.All(x => x.Role == RolesConstants.Student):
                    return true;
                default:
                    return false;
            }
        }

        public bool CanUserExistWithoutTenant(IEnumerable<RoleEntity> newUserRoles)
        {
            return !newUserRoles.Any(x =>
                x.Role == RolesConstants.UniversityAdmin ||
                x.Role == RolesConstants.Teacher ||
                x.Role == RolesConstants.Student);
        }

        #endregion

        #region University

        public bool CanUserCreateUniversity(string accessToken)
        {
            return _tokenDecoder.GetHighestUserRole(accessToken) == RolesConstants.SuperAdmin;
        }

        public bool CanUserReadAllUniversities(string accessToken)
        {
            var highestRole = _tokenDecoder.GetHighestUserRole(accessToken);

            return highestRole == RolesConstants.SuperAdmin ||
                   highestRole == RolesConstants.Service;
        }

        public bool CanUserUpdateUniversity(string accessToken)
        {
            return _tokenDecoder.GetHighestUserRole(accessToken) == RolesConstants.SuperAdmin;
        }

        public bool CanUserDeleteUniversity(string accessToken)
        {
            return _tokenDecoder.GetHighestUserRole(accessToken) == RolesConstants.SuperAdmin;
        }

        #endregion

        #region Roles

        public IEnumerable<string> WhichRolesUserHasAccessTo(string accessToken)
        {
            var highestUserRole = _tokenDecoder.GetHighestUserRole(accessToken);

            switch (highestUserRole)
            {
                case RolesConstants.SuperAdmin:
                case RolesConstants.Service:
                    return new[]
                    {
                        RolesConstants.SuperAdmin, RolesConstants.Service, RolesConstants.UniversityAdmin,
                        RolesConstants.Teacher, RolesConstants.Student
                    };
                case RolesConstants.UniversityAdmin:
                    return new[]
                    {
                        RolesConstants.UniversityAdmin, RolesConstants.Teacher, RolesConstants.Student
                    };
                case RolesConstants.Teacher:
                    return new[]
                    {
                        RolesConstants.Teacher, RolesConstants.Student
                    };
                default:
                    return Array.Empty<string>();
            }
        }

        #endregion
    }
}
