using MyUniversity.UserManager.Repository.Entities.Common;
using MyUniversity.UserManager.Repository.Entities.User;
using System.Collections.Generic;

namespace MyUniversity.UserManager.Repository.Entities.University
{
    public class UniversityEntity : ITenantSpecificEntity
    {
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        public IEnumerable<UserEntity> Users { get; set; }
    }
}
