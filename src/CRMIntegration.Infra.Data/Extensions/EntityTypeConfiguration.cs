using CRMIntegration.Domain.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMIntegration.Infra.Data.Extensions
{
    public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : Entity<Guid>
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
