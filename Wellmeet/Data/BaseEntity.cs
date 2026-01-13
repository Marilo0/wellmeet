namespace Wellmeet.Data
{
    public class BaseEntity
    {
        public DateTime InsertedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;  // soft delete
        public DateTime? DeletedAt { get; set; }
    }
}
