namespace MyUniversity.UserManager.Repository.Entities.Common
{
    public interface ISoftDeletableEntity
    {
        bool IsSoftDeleted { get; set; }
    }
}
