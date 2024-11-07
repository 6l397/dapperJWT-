namespace tindog_marketplace.DAL.Entities
{
    public class MarketplaceOrder : BaseEntity
    {
        public int seller_id { get; set; }

        public int buyer_id { get; set; }

        public string product_name { get; set; }
        public int quantity { get; set; }
        public decimal total_price { get; set; }

        public string order_status { get; set; } = "pending"; 

        public enum OrderStatusEnum
        {
            Pending,
            Shipped,
            Delivered,
            Cancelled
        }
    }

}
