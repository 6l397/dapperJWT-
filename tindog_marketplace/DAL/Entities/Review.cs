namespace tindog_marketplace.DAL.Entities
{
    public class Review : BaseEntity
    {
        public int seller_id { get; set; }

        public int user_id { get; set; }

        public int Rating { get; set; } 
        public string Comment { get; set; }
    }

}
