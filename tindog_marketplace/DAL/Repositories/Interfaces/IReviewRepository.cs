using tindog_marketplace.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<IEnumerable<ReviewWithUser>> GetReviewsBySellerIdWithUserAsync(int sellerId); 
}

public class ReviewWithUser
{
    public Review Review { get; set; }
    public User User { get; set; }
}