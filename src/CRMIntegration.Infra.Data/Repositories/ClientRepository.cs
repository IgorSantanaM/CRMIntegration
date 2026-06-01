using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Clients.Common;
using CRMIntegration.Domain.Core.Model;
using CRMIntegration.Infra.Data.Contexts;
using CRMIntegration.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRMIntegration.Infra.Data.Repositories
{
    public class ClientRepository(CRMIntegrationContext context) : Repository<Client, Guid>(context), IClientRepository
    {
        public Task AddRangeAsync(IEnumerable<Client> clients, CancellationToken cancellationToken = default)
        {
            return context.AddRangeAsync(clients, cancellationToken);
        }

        public Task<int> CountActionableAsync(CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.Acionavel).AsNoTracking().CountAsync(cancellationToken);
        }

        public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.Ativo).AsNoTracking().CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().CountAsync(cancellationToken);
        }

        public Task<bool> ExistsWithCobMaisIdAsync(int idCobMais, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().AnyAsync(c => c.IdCobMais == idCobMais, cancellationToken);
        }

        public Task<bool> ExistsWithCPFCNPJAsync(string cpfCnpj, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().AnyAsync(c => c.CPFCNPJ.ToUpper() == cpfCnpj.ToUpper()
                        && (excludeId == null || c.Id != excludeId), cancellationToken);
        }

        public Task<bool> ExistsWithWhatsAppAsync(string whatsapp, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().AnyAsync(c => c.Whatsapp == whatsapp
                        && (excludeId == null || c.Id != excludeId), cancellationToken);
        }

        public Task<PagedResult<Client>> GetActionableAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.Acionavel).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetActiveAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.Ativo).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetByCobMaisIdsAsync(IEnumerable<int> idsCobMais, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => idsCobMais.Contains(c.IdCobMais)).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<Client?> GetByCPFCNPJAsync(string cpfCnpj, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.CPFCNPJ == cpfCnpj, cancellationToken);
        }

        public Task<PagedResult<Client>> GetByCreatedDateRangeAsync(DateTime startDate,
            DateTime endDate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.DataCriacao >= startDate && c.DataCriacao <= endDate)
                                  .AsNoTracking()
                                  .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<Client?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Email.ToUpper() == email.ToUpper(), cancellationToken);
        }

        public Task<Client?> GetByIdCobMaisAsync(int idCobMais, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.IdCobMais == idCobMais, cancellationToken);
        }

        public Task<PagedResult<Client>> GetByIdsAsync(IEnumerable<Guid> ids, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => ids.Contains(c.Id)).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<Client?> GetByIdBemChatAsync(string idBemChat, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.IdBemChat == idBemChat, cancellationToken);
        }

        public Task<Client?> GetByWhatsAppAsync(string whatsapp, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Whatsapp == whatsapp, cancellationToken);
        }

        public Task<PagedResult<Client>> GetInactiveAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => !c.Ativo).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetNeedingResynchronizationAsync(int pageNumber, int pageSize, int daysToExpire = 30, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.DataSincronizacaoBemChat.HasValue
                         && c.DataSincronizacaoBemChat.Value.AddDays(daysToExpire) < DateTime.UtcNow)
                         .AsNoTracking()
                         .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetNonActionableAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => !c.Acionavel).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetNotRecentlyActionedAsync(int pageNumber, int pageSize, int days = 30, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.DataUltimoAcionamento.HasValue
                        && c.DataUltimoAcionamento.Value.AddDays(days) < DateTime.UtcNow)
                        .AsNoTracking()
                        .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetNotSynchronizedWithBemChatAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => !c.DataSincronizacaoBemChat.HasValue).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetPaginatedWithFilterAsync(int pageNumber, int pageSize, ClientFilter filter, CancellationToken cancellationToken = default)
        {
            var query = context.Clients.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(c => c.Nome.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(c => c.Email.Contains(filter.Email));

            if (!string.IsNullOrEmpty(filter.CPFCNPJ))
                query = query.Where(c => c.CPFCNPJ.Contains(filter.CPFCNPJ));

            if (filter.Ativo.HasValue)
                query = query.Where(c => c.Ativo == filter.Ativo.Value);

            if (filter.Acionavel.HasValue)
                query = query.Where(c => c.Acionavel == filter.Acionavel.Value);

            return query.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetReadyForCampaignAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.Acionavel && c.Ativo).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> GetRecentlyActionedAsync(int pageNumber, int pageSize, int days = 7, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.DataUltimoAcionamento.HasValue
                        && c.DataUltimoAcionamento.Value.AddDays(days) >= DateTime.UtcNow)
                        .AsNoTracking()
                        .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<ClientStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return context.Clients.AsNoTracking().ToClientStatisticsAsync(cancellationToken);
        }

        public Task<PagedResult<Client>> GetSynchronizedWithBemChatAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.DataSincronizacaoBemChat.HasValue).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PagedResult<Client>> SearchByNameAsync(string name, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return context.Clients.Where(c => c.Nome.Contains(name)).AsNoTracking().ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task UpdateRangeAsync(IEnumerable<Client> clients, CancellationToken cancellationToken = default)
        {
            context.UpdateRange(clients);
            return Task.CompletedTask;
        }
    }
}
