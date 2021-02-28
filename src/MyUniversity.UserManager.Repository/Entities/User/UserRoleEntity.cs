namespace MyUniversity.UserManager.Repository.Entities.User
{
    public class UserRoleEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public UserEntity User { get; set; }
        public RoleEntity Role { get; set; }
    }
}
