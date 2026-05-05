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
            return context.Campaigns.AsNoTracking().CountAsync(cancellationToken);
        }

        public Task<int> CountByCampaignAndStatusAsync(Guid campaignId, MessageStatus status, CancellationToken cancellationToken = default)
        {
            return context.CampaignMessages.AsNoTracking().Where(cm => cm.CampaignId == campaignId && cm.Status == status).CountAsync(cancellationToken);
        }

        public Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return context.CampaignMessages.AsNoTracking().Where(cm => cm.CampaignId == campaignId).CountAsync(cancellationToken);
        }

        public Task<int> CountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsForClientInCampaignAsync(Guid campaignId, Guid clientId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsWithNameTodayAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<double?> GetAverageDeliveryTimeAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<double?> GetAverageReadTimeAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetByCampaignIdAndStatusAsync(Guid campaignId, MessageStatus status, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignMessage?> GetByClientWhatsAppAndRecentCampaignAsync(string whatsapp, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetByCreatedDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Campaign?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetByScheduledDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetBySentDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetByStatusAsync(MessageStatus status, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetByTemplateAsync(string templateName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Campaign?> GetByVollCampaignIdAsync(string idCampanhaVoll, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignMessage?> GetByVollMessageIdAsync(string idMensagemVoll, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetDeliveredButNotReadAsync(int hoursDelivered = 24, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetFailedMessagesByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<Campaign> Items, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<CampaignMessage> Items, int TotalCount)> GetPaginatedByCampaignAsync(Guid campaignId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetPendingFinalizationAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CampaignStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MessageStatistics> GetStatisticsByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignMessage>> GetWaitingDeliveryAsync(int minutesWaiting = 5, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<CampaignMessage> messages, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<CampaignMessage?> GetMessageByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsForClientInCampaignAsync(Guid campaignId, int clientId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddMessageAsync(CampaignMessage campaignMessage, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task IncrementSentCountAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            return _context.Set<Campaign>().Where(c => c.Id == campaignId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.TotalEnviados, c => c.TotalEnviados + 1), cancellationToken);
        }
    }
}
