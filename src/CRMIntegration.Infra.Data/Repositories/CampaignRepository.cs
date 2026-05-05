using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Campaings.Common;
using CRMIntegration.Domain.Campaings.Enum;
using CRMIntegration.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CRMIntegration.Infra.Data.Repositories
{
    public class CampaignRepository(CRMIntegrationContext context) : Repository<Campaign, Guid>(context), ICampaignRepository
    {
        public Task AddRangeAsync(IEnumerable<CampaignMessage> messages, CancellationToken cancellationToken = default)
        {
            return context.AddRangeAsync(messages, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return context.Campaigns.AsNoTracking()
                .CountAsync(cancellationToken);
        }

        public Task<int> CountByCampaignAndStatusAsync(Guid campaignId, MessageStatus status, CancellationToken cancellationToken = default)
        {
            return context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId && cm.Status == status).CountAsync(cancellationToken);
        }

        public Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId).CountAsync(cancellationToken);
        }

        public Task<int> CountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
        {
            return context.Campaigns.AsNoTracking()
                .Where(c => c.Status == status).CountAsync(cancellationToken);
        }

        public Task<bool> ExistsForClientInCampaignAsync(Guid campaignId, Guid clientId, CancellationToken cancellationToken = default)
        {
            return context.CampaignMessages.AsNoTracking()
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.ClientId == clientId, cancellationToken);
        }

        public Task<bool> ExistsWithNameTodayAsync(string name, CancellationToken cancellationToken = default)
        {
            DateTime today = DateTime.UtcNow.Date;
            return context.Campaigns.AsNoTracking()
                .AnyAsync(c => c.Nome == name && c.DataDisparo.Date == today, cancellationToken);
        }

        public Task<IEnumerable<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return context.Campaigns.AsNoTracking()
                .Where(c => c.Status == CampaignStatus.Processing)
                .ToListAsync(cancellationToken)
                .ContinueWith(t => t.Result.AsEnumerable(), cancellationToken);
        }

        public async Task<double?> GetAverageDeliveryTimeAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            var validMessages = await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId && cm.DataEntrega.HasValue)
                .Select(cm => new { cm.DataEntrega, cm.DataEnvio })
                .ToListAsync(cancellationToken);
            
            if (!validMessages.Any()) return null;

            return validMessages.Average(cm => (cm.DataEntrega!.Value - cm.DataEnvio).TotalSeconds);
        }

        public async Task<double?> GetAverageReadTimeAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            var validMessages = await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId && cm.DataLeitura.HasValue && cm.DataEntrega.HasValue)
                .Select(cm => new { cm.DataLeitura, cm.DataEntrega })
                .ToListAsync(cancellationToken);

            if (!validMessages.Any()) return null;
            
            return validMessages.Average(cm => (cm.DataLeitura!.Value - cm.DataEntrega!.Value).TotalSeconds);
        }

        public async Task<IEnumerable<CampaignMessage>> GetByCampaignIdAndStatusAsync(Guid campaignId, MessageStatus status, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId && cm.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CampaignMessage>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CampaignMessage>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.ClientId == clientId)
                .ToListAsync(cancellationToken);
        }

        public async Task<CampaignMessage?> GetByClientWhatsAppAndRecentCampaignAsync(string whatsapp, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Include(cm => cm.Client)
                .Where(cm => cm.Client.Whatsapp == whatsapp)
                .OrderByDescending(cm => cm.DataEnvio)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Campaign>> GetByCreatedDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .Where(c => c.DataCriacao >= startDate && c.DataCriacao <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Campaign?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .Include(c => c.Mensagens)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Campaign>> GetByScheduledDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .Where(c => c.DataDisparo >= startDate && c.DataDisparo <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CampaignMessage>> GetBySentDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.DataEnvio >= startDate && cm.DataEnvio <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .Where(c => c.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CampaignMessage>> GetByStatusAsync(MessageStatus status, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Campaign>> GetByTemplateAsync(string templateName, CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .Where(c => c.Template == templateName)
                .ToListAsync(cancellationToken);
        }

        public async Task<Campaign?> GetByVollCampaignIdAsync(string idCampanhaVoll, CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCampanhaVoll == idCampanhaVoll, cancellationToken);
        }

        public async Task<CampaignMessage?> GetByVollMessageIdAsync(string idMensagemVoll, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .FirstOrDefaultAsync(cm => cm.IdMensagemVoll == idMensagemVoll, cancellationToken);
        }

        public async Task<IEnumerable<CampaignMessage>> GetDeliveredButNotReadAsync(int hoursDelivered = 24, CancellationToken cancellationToken = default)
        {
            var dateThreshold = DateTime.UtcNow.AddHours(-hoursDelivered);
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.Status == MessageStatus.Delivered && cm.DataEntrega <= dateThreshold)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CampaignMessage>> GetFailedMessagesByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId && cm.Status == MessageStatus.Failed)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Campaign> Items, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = context.Campaigns.AsNoTracking();
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<(IEnumerable<CampaignMessage> Items, int TotalCount)> GetPaginatedByCampaignAsync(Guid campaignId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = context.CampaignMessages.AsNoTracking().Where(cm => cm.CampaignId == campaignId);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<IEnumerable<Campaign>> GetPendingFinalizationAsync(CancellationToken cancellationToken = default)
        {
            return await context.Campaigns.AsNoTracking()
                .Where(c => c.Status == CampaignStatus.Processing)
                .ToListAsync(cancellationToken);
        }

        public async Task<CampaignStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return new CampaignStatistics(0, 0, 0, 0, 0, 0, 0m, 0m, 0m, 0, 0, 0, 0, 0, 0); // Need proper implementation according to Domain logic
        }

        public async Task<MessageStatistics> GetStatisticsByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            var messages = await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.CampaignId == campaignId)
                .GroupBy(cm => cm.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            int sent = messages.FirstOrDefault(m => m.Status == MessageStatus.Sent)?.Count ?? 0;
            int delivered = messages.FirstOrDefault(m => m.Status == MessageStatus.Delivered)?.Count ?? 0;
            int read = messages.FirstOrDefault(m => m.Status == MessageStatus.Read)?.Count ?? 0;
            int failed = messages.FirstOrDefault(m => m.Status == MessageStatus.Failed)?.Count ?? 0;
            int total = sent + delivered + read + failed;
            decimal deliveryRate = total > 0 ? (decimal)delivered / total : 0m;
            decimal readRate = total > 0 ? (decimal)read / total : 0m;
            decimal failureRate = total > 0 ? (decimal)failed / total : 0m;

            return new MessageStatistics(total, sent, delivered, read, failed, deliveryRate, readRate, failureRate, null, null);
        }

        public async Task<IEnumerable<CampaignMessage>> GetWaitingDeliveryAsync(int minutesWaiting = 5, CancellationToken cancellationToken = default)
        {
            var dateThreshold = DateTime.UtcNow.AddMinutes(-minutesWaiting);
            return await context.CampaignMessages.AsNoTracking()
                .Where(cm => cm.Status == MessageStatus.Sent && cm.DataEnvio <= dateThreshold)
                .ToListAsync(cancellationToken);
        }

        public Task UpdateRangeAsync(IEnumerable<CampaignMessage> messages, CancellationToken cancellationToken = default)
        {
            context.CampaignMessages.UpdateRange(messages);
            return Task.CompletedTask;
        }

        public async Task<CampaignMessage?> GetMessageByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .FirstOrDefaultAsync(cm => cm.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsForClientInCampaignAsync(Guid campaignId, int clientId, CancellationToken cancellationToken = default)
        {
            return await context.CampaignMessages.AsNoTracking()
                .AnyAsync(cm => cm.CampaignId == campaignId && cm.Client.IdCobMais == clientId, cancellationToken);
        }

        public async Task AddMessageAsync(CampaignMessage campaignMessage, CancellationToken cancellationToken = default)
        {
            await context.CampaignMessages.AddAsync(campaignMessage, cancellationToken);
        }

        public Task IncrementSentCountAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return _context.Set<Campaign>().Where(c => c.Id == campaignId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.TotalEnviados, c => c.TotalEnviados + 1), cancellationToken);
        }
    }
}
