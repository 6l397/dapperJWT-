using System.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _dbConnection;
    private IDbTransaction _dbTransaction;

    public IUserRepository Users { get; private set; }
    public ISellerRepository Sellers { get; private set; }
    public IMarketplaceOrderRepository Orders { get; private set; }
    public IReviewRepository Reviews { get; private set; }

    public UnitOfWork(IUserRepository userRepository,
                      ISellerRepository sellerRepository,
                      IMarketplaceOrderRepository orderRepository,
                      IReviewRepository reviewRepository,
                      IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
        if (_dbConnection.State == ConnectionState.Closed)
        {
            _dbConnection.Open();
        }
        _dbTransaction = dbConnection.BeginTransaction(); 

        Users = userRepository;
        Sellers = sellerRepository;
        Orders = orderRepository;
        Reviews = reviewRepository;
    }

    public async Task<int> CompleteAsync()
    {
        try
        {
            _dbTransaction.Commit();
            return await Task.FromResult(1);
        }
        catch
        {
            _dbTransaction.Rollback();
            throw;
        }
    }

    public void Dispose()
    {
        _dbTransaction?.Dispose();
        _dbConnection?.Dispose();
    }
}