using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using tindog_marketplace.DAL.Entities;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(IDbConnection dbConnection)
        : base(dbConnection, dbTransaction: null, tableName: "reviews")
    {
    }

    public async Task<IEnumerable<ReviewWithUser>> GetReviewsBySellerIdWithUserAsync(int sellerId)
    {
        var sql = @"
                SELECT r.*, u.*
                FROM reviews r
                INNER JOIN users u ON r.user_id = u.id
                WHERE r.seller_id = @seller_id";

        var lookup = new Dictionary<int, ReviewWithUser>();

        var result = await _dbConnection.QueryAsync<Review, User, ReviewWithUser>(
            sql,
            (review, user) =>
            {
                if (!lookup.TryGetValue(review.Id, out var reviewWithUser))
                {
                    reviewWithUser = new ReviewWithUser
                    {
                        Review = review,
                        User = user
                    };
                    lookup.Add(review.Id, reviewWithUser);
                }

                return reviewWithUser;
            },
            new { seller_id = sellerId },
            splitOn: "id",
            transaction: _dbTransaction
        );

        return result;
    }
}
