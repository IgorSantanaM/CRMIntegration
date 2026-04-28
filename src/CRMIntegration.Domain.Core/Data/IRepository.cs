using CRMIntegration.Domain.Core.Model;

namespace CRMIntegration.Domain.Core.Data
{
    public interface IRepository<TEntity, in TId> where TEntity : IAggregateRoot
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken token = default);
    }
}
