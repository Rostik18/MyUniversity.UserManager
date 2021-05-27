
namespace MyUniversity.UserManager.Repository.Entities.Common
{
    public interface ITenantSpecificEntity
    {
        string TenantId { get; set; }
    }
}
