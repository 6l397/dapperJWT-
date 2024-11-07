using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using Dapper;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected IDbConnection _dbConnection;
    protected IDbTransaction _dbTransaction;
    private readonly string _tableName;

    protected GenericRepository(IDbConnection dbConnection, IDbTransaction dbTransaction, string tableName)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
        _tableName = tableName;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var sql = $"SELECT * FROM {_tableName}";
        return await _dbConnection.QueryAsync<T>(sql, transaction: _dbTransaction);
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE id = @Id";
        var result = await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id }, transaction: _dbTransaction);
        if (result == null)
            throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found.");
        return result;
    }

    public async Task<int> AddAsync(T entity)
    {
        var insertQuery = GenerateInsertQuery();
        var newId = await _dbConnection.ExecuteScalarAsync<int>(insertQuery, entity, transaction: _dbTransaction);
        return newId;
    }

    public async Task<int> AddRangeAsync(IEnumerable<T> entities)
    {
        var insertQuery = GenerateInsertQuery();
        var inserted = await _dbConnection.ExecuteAsync(insertQuery, entities, transaction: _dbTransaction);
        return inserted;
    }

    public async Task UpdateAsync(T entity)
    {
        var updateQuery = GenerateUpdateQuery();
        await _dbConnection.ExecuteAsync(updateQuery, entity, transaction: _dbTransaction);
    }

    public async Task DeleteAsync(int id)
    {
        var sql = $"DELETE FROM {_tableName} WHERE id = @Id";
        await _dbConnection.ExecuteAsync(sql, new { Id = id }, transaction: _dbTransaction);
    }

    public async Task ReplaceAsync(T entity)
    {
        var updateQuery = GenerateUpdateQuery();
        await _dbConnection.ExecuteAsync(updateQuery, entity, transaction: _dbTransaction);
    }

    private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

    private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
    {
        return (from prop in listOfProperties
                let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                select prop.Name).ToList();
    }

    private string GenerateUpdateQuery()
    {
        var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
        var properties = GenerateListOfProperties(GetProperties);
        properties.ForEach(property =>
        {
            if (!property.Equals("Id"))
            {
                updateQuery.Append($"{property} = @{property},");
            }
        });
        updateQuery.Remove(updateQuery.Length - 1, 1); 
        updateQuery.Append(" WHERE id = @Id");
        return updateQuery.ToString();
    }

    private string GenerateInsertQuery()
    {
        var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");
        insertQuery.Append("(");
        var properties = GenerateListOfProperties(GetProperties);
        properties.Remove("Id"); 
        properties.ForEach(prop => { insertQuery.Append($"{prop},"); });
        insertQuery.Remove(insertQuery.Length - 1, 1).Append(") VALUES (");
        properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
        insertQuery.Remove(insertQuery.Length - 1, 1).Append(")");
        insertQuery.Append("; SELECT LAST_INSERT_ID()");
        return insertQuery.ToString();
    }
}
