using CRMIntegration.Domain.Core.Model;

namespace CRMIntegration.Domain.Core.Data
{
    public interface IRepository<TEntity, in TId> where TEntity : IAggregateRoot
    {
        Task AddAsync(TEntity entity, CancellationToken token = default);
        Task UpdateAsync(TEntity entity, CancellationToken token = default);
        Task DeleteAsync(TEntity entity, CancellationToken token = default);
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken token = default);
    }
}
