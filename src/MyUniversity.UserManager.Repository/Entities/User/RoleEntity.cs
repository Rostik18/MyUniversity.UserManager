using System.Collections.Generic;
using MyUniversity.UserManager.Repository.Entities.Common;

namespace MyUniversity.UserManager.Repository.Entities.User
{
    public class RoleEntity : IEntity
    {
        public int Id { get; set; }
        public string Role { get; set; }

        public IEnumerable<UserRoleEntity> UserRoles { get; set; }
    }
}
