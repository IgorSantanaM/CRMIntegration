using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Infra.Data.Contexts;

public class UnitOfWork(CRMIntegrationContext context) : IUnitOfwork, IAsyncDisposable, IDisposable
{
    private bool _disposed;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        if (_disposed) return;

        context.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await context.DisposeAsync().ConfigureAwait(false);
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}