namespace tindog_marketplace.DAL.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }

}
