using tindog_marketplace.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository : IGenericRepository<User>
    {
    Task<User> GetByEmailAsync(string email); 
    Task<UserWithOrders> GetUserWithOrdersAsync(int userId);
}

public class UserWithOrders
{
    public User User { get; set; }
    public IEnumerable<MarketplaceOrder> Orders { get; set; }
}