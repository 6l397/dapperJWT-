using System.Collections.Generic;
using System.Threading.Tasks;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(); 
    Task<T> GetByIdAsync(int id); 
    Task<int> AddAsync(T entity); 
    Task<int> AddRangeAsync(IEnumerable<T> entities); 
    Task UpdateAsync(T entity); 
    Task DeleteAsync(int id); 
    Task ReplaceAsync(T entity); 
}