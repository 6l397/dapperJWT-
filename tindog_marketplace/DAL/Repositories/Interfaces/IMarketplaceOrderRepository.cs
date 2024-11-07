using tindog_marketplace.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMarketplaceOrderRepository : IGenericRepository<MarketplaceOrder>
{
    Task<IEnumerable<OrderWithSeller>> GetOrdersByUserIdWithSellerAsync(int userId); 
}

public class OrderWithSeller
{
    public MarketplaceOrder Order { get; set; }
    public Seller Seller { get; set; }
}