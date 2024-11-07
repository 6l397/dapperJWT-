using tindog_marketplace.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;


public interface ISellerRepository : IGenericRepository<Seller>
{
    Task<IEnumerable<SellerWithReviews>> GetTopSellersWithReviewsAsync(int topCount);
}

public class SellerWithReviews
{
    public Seller Seller { get; set; }
    public IEnumerable<Review> Reviews { get; set; }
}