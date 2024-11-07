using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using tindog_marketplace.DAL.Entities;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IDbConnection dbConnection)
        : base(dbConnection, dbTransaction: null, "users")
    {
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM users WHERE email = @Email";
        return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email }, transaction: _dbTransaction);
    }

    public async Task<UserWithOrders> GetUserWithOrdersAsync(int userId)
    {
        var sql = @"
                SELECT u.*, o.*
                FROM users u
                LEFT JOIN orders o ON u.id = o.buyer_id
                WHERE u.id = @user_id";

        var lookup = new Dictionary<int, UserWithOrders>();

        var result = await _dbConnection.QueryAsync<User, MarketplaceOrder, UserWithOrders>(
            sql,
            (user, order) =>
            {
                if (!lookup.TryGetValue(user.Id, out var userWithOrders))
                {
                    userWithOrders = new UserWithOrders
                    {
                        User = user,
                        Orders = new List<MarketplaceOrder>()
                    };
                    lookup.Add(user.Id, userWithOrders);
                }

                if (order != null)
                {
                    ((List<MarketplaceOrder>)userWithOrders.Orders).Add(order);
                }

                return userWithOrders;
            },
            new { user_id = userId },
            splitOn: "id",
            transaction: _dbTransaction
        );

        return result.FirstOrDefault();
    }
}
