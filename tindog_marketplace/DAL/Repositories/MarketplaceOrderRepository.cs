using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using tindog_marketplace.DAL.Entities;

public class MarketplaceOrderRepository : GenericRepository<MarketplaceOrder>, IMarketplaceOrderRepository
{
    public MarketplaceOrderRepository(IDbConnection dbConnection)
        : base(dbConnection, dbTransaction: null, tableName: "orders")
    {
    }

    public async Task<IEnumerable<OrderWithSeller>> GetOrdersByUserIdWithSellerAsync(int userId)
    {
        var sql = @"
                SELECT o.*, s.*
                FROM orders o
                INNER JOIN sellers s ON o.seller_id = s.id
                WHERE o.buyer_id = @user_id";

        var lookup = new Dictionary<int, OrderWithSeller>();

        var result = await _dbConnection.QueryAsync<MarketplaceOrder, Seller, OrderWithSeller>(
            sql,
            (order, seller) =>
            {
                if (!lookup.TryGetValue(order.Id, out var orderWithSeller))
                {
                    orderWithSeller = new OrderWithSeller
                    {
                        Order = order,
                        Seller = seller
                    };
                    lookup.Add(order.Id, orderWithSeller);
                }

                return orderWithSeller;
            },
            new { user_id = userId },
            splitOn: "id",
            transaction: _dbTransaction
        );

        return result;
    }
}