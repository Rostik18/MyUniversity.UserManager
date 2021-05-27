using System.Collections.Generic;

namespace MyUniversity.UserManager.Models.User
{
    public class RegisterUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string UniversityId { get; set; }
        public string Password { get; set; }

        public IEnumerable<int> Roles { get; set; }
    }
}
