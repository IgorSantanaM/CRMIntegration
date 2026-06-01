using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Clients.Common;
using CRMIntegration.Domain.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace CRMIntegration.Infra.Data.Extensions
{
    public static class ExtensionsMapping
    {
        public static async Task<PagedResult<TEntity>> ToPagedResultAsync<TEntity>(
        this IQueryable<TEntity> source,
        int pageNumber,
        int pageSize,
        CancellationToken token = default) where TEntity : class
        {
            if (token.IsCancellationRequested)
                return await Task.FromCanceled<PagedResult<TEntity>>(token);

            var totalCount = await source.CountAsync(token);

            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public static async Task<ClientStatistics> ToClientStatisticsAsync(
        this IQueryable<Client> clients,
        CancellationToken cancellationToken = default)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var stats = await clients
            .GroupBy(c => 1)
            .Select(g => new ClientStatistics(
                g.Count(),
                g.Count(c => c.Ativo),
                g.Count(c => !c.Ativo),
                g.Count(c => c.Acionavel),
                g.Count(c => !c.Acionavel),
                g.Count(c => c.DataSincronizacaoBemChat != null),
                g.Count(c => c.DataSincronizacaoBemChat == null),
                g.Count(c => c.DataUltimoAcionamento >= sevenDaysAgo),
                g.Count(c => c.DataUltimoAcionamento >= thirtyDaysAgo),
                g.Count(c => c.DataUltimoAcionamento == null),

                g.Min(c => (DateTime?)c.DataCriacao),
                g.Max(c => (DateTime?)c.DataCriacao)
            ))
            .FirstOrDefaultAsync(cancellationToken);

            return stats ?? new ClientStatistics(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, null);
        }
    }
}
