using System.Collections.Generic;
using MyUniversity.UserManager.Repository.Entities.Common;
using MyUniversity.UserManager.Repository.Entities.University;

namespace MyUniversity.UserManager.Repository.Entities.User
{
    public class UserEntity : IEntity, ISoftDeletableEntity, ITenantSpecificEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string TenantId { get; set; }
        public bool IsSoftDeleted { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public IEnumerable<UserRoleEntity> UserRoles { get; set; }
        public UniversityEntity University { get; set; }
    }
}
