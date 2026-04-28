using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Domain.Core.Model;
using CRMIntegration.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CRMIntegration.Infra.Data.Repositories
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, IAggregateRoot
    {
        protected CRMIntegrationContext _context;
        protected DbSet<TEntity> _dbSet;

        public Repository(CRMIntegrationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public Task AddAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<TEntity?> GetByIdAsync(TId id, CancellationToken token = default)
        {
            return _dbSet.FindAsync(id, token).AsTask();
        }

        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
