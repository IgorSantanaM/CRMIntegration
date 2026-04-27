namespace CRMIntegration.Domain.Core.Data
{
    public interface IUnitOfwork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
