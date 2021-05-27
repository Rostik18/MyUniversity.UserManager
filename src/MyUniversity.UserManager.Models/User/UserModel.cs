using System.Collections.Generic;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Models.University;

namespace MyUniversity.UserManager.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string TenantId { get; set; }

        public IEnumerable<RoleModel> Roles { get; set; }
        public UniversityModel University { get; set; }
    }
}
