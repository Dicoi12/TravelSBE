namespace TravelSBE.Entity.Helper
{
    public class BaseAuditEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
