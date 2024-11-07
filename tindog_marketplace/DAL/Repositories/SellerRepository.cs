using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using tindog_marketplace.DAL.Entities;
    public class SellerRepository : GenericRepository<Seller>, ISellerRepository
    {
        public SellerRepository(IDbConnection dbConnection)
            : base(dbConnection, dbTransaction: null, tableName: "sellers")
        {
        }

        public async Task<IEnumerable<SellerWithReviews>> GetTopSellersWithReviewsAsync(int topCount)
        {
            var sql = @"
                SELECT s.*, r.*
                FROM sellers s
                LEFT JOIN reviews r ON s.id = r.seller_id
                ORDER BY s.created_at DESC
                LIMIT @TopCount";

            var lookup = new Dictionary<int, SellerWithReviews>();

            var result = await _dbConnection.QueryAsync<Seller, Review, SellerWithReviews>(
                sql,
                (seller, review) =>
                {
                    if (!lookup.TryGetValue(seller.Id, out var sellerWithReviews))
                    {
                        sellerWithReviews = new SellerWithReviews
                        {
                            Seller = seller,
                            Reviews = new List<Review>()
                        };
                        lookup.Add(seller.Id, sellerWithReviews);
                    }

                    if (review != null)
                    {
                        ((List<Review>)sellerWithReviews.Reviews).Add(review);
                    }

                    return sellerWithReviews;
                },
                new { TopCount = topCount },
                splitOn: "id",
                transaction: _dbTransaction
            );

            return result.Distinct();
        }
    }
