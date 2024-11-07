
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ISellerRepository Sellers { get; }
    IMarketplaceOrderRepository Orders { get; }
    IReviewRepository Reviews { get; }

    Task<int> CompleteAsync(); 
}
