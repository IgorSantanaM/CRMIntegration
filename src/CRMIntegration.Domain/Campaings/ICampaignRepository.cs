using CRMIntegration.Domain.Campaings.Common;
using CRMIntegration.Domain.Campaings.Enum;
using CRMIntegration.Domain.Core.Data;

namespace CRMIntegration.Domain.Campaings
{
    /// <summary>
    /// Repository interface for Campaign aggregate root
    /// </summary>
    public interface ICampaignRepository : IRepository<Campaign, Guid>
    {
        /// <summary>
        /// Get a campaign by its BemChat campaign ID
        /// </summary>
        /// <param name="idCampanhaBemChat">BemChat campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Campaign if found, null otherwise</returns>
        Task<Campaign?> GetByBemChatCampaignIdAsync(string idCampanhaBemChat, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a campaign by ID including all its messages
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Campaign with messages if found, null otherwise</returns>
        Task<Campaign?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all campaigns with a specific status
        /// </summary>
        /// <param name="status">Campaign status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of campaigns</returns>
        Task<IEnumerable<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get campaigns created within a date range
        /// </summary>
        /// <param name="startDate">Start date (inclusive)</param>
        /// <param name="endDate">End date (inclusive)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of campaigns</returns>
        Task<IEnumerable<Campaign>> GetByCreatedDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get campaigns scheduled to be sent within a date range
        /// </summary>
        /// <param name="startDate">Start date (inclusive)</param>
        /// <param name="endDate">End date (inclusive)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of campaigns</returns>
        Task<IEnumerable<Campaign>> GetByScheduledDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get campaigns that are currently being processed
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of campaigns with status Processing or Sent</returns>
        Task<IEnumerable<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get campaigns that need finalization (all messages processed but campaign still as Sent)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of campaigns ready to be finalized</returns>
        Task<IEnumerable<Campaign>> GetPendingFinalizationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated list of campaigns
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of campaigns</returns>
        Task<(IEnumerable<Campaign> Items, int TotalCount)> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get campaign statistics for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Campaign statistics</returns>
        Task<CampaignStatistics> GetStatisticsAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a campaign with the same name already exists for today
        /// </summary>
        /// <param name="name">Campaign name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsWithNameTodayAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get campaigns by template name
        /// </summary>
        /// <param name="templateName">Template name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of campaigns using this template</returns>
        Task<IEnumerable<Campaign>> GetByTemplateAsync(string templateName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Count total campaigns
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Total count</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Count campaigns by status
        /// </summary>
        /// <param name="status">Campaign status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Count of campaigns with this status</returns>
        Task<int> CountByStatusAsync(CampaignStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a message by its unique identifier
        /// </summary>
        /// <param name="id">Message ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message if found, null otherwise</returns>
        Task<CampaignMessage?> GetMessageByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a message by BemChat message ID
        /// </summary>
        /// <param name="idMensagemBemChat">BemChat message ID (wamid)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message if found, null otherwise</returns>
        Task<CampaignMessage?> GetByBemChatMessageIdAsync(string idMensagemBemChat, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all messages for a specific campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<CampaignMessage>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all messages for a specific campaign with a specific status
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="status">Message status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<CampaignMessage>> GetByCampaignIdAndStatusAsync(
            Guid campaignId,
            MessageStatus status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all messages for a specific client
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<CampaignMessage>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the most recent message for a client by WhatsApp number
        /// </summary>
        /// <param name="whatsapp">WhatsApp number</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Most recent message if found, null otherwise</returns>
        Task<CampaignMessage?> GetByClientWhatsAppAndRecentCampaignAsync(
            string whatsapp,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get messages sent within a date range
        /// </summary>
        /// <param name="startDate">Start date (inclusive)</param>
        /// <param name="endDate">End date (inclusive)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<CampaignMessage>> GetBySentDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get messages that are waiting for delivery (sent but no webhook received yet)
        /// </summary>
        /// <param name="minutesWaiting">Minimum minutes waiting</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages waiting delivery</returns>
        Task<IEnumerable<CampaignMessage>> GetWaitingDeliveryAsync(
            int minutesWaiting = 5,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get failed messages for a specific campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of failed messages</returns>
        Task<IEnumerable<CampaignMessage>> GetFailedMessagesByCampaignAsync(
            Guid campaignId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get messages with specific status
        /// </summary>
        /// <param name="status">Message status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<CampaignMessage>> GetByStatusAsync(
            MessageStatus status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get message statistics for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message statistics</returns>
        Task<MessageStatistics> GetStatisticsByCampaignAsync(
            Guid campaignId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get average delivery time for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Average delivery time in seconds, null if no data</returns>
        Task<double?> GetAverageDeliveryTimeAsync(
            Guid campaignId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get average read time for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Average read time in seconds, null if no data</returns>
        Task<double?> GetAverageReadTimeAsync(
            Guid campaignId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a message already exists for a specific client in a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsForClientInCampaignAsync(
            Guid campaignId,
            int clientId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated messages for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of messages</returns>
        Task<(IEnumerable<CampaignMessage> Items, int TotalCount)> GetPaginatedByCampaignAsync(
            Guid campaignId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get messages delivered but not read yet (for follow-up)
        /// </summary>
        /// <param name="hoursDelivered">Minimum hours since delivery</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of messages</returns>
        Task<IEnumerable<CampaignMessage>> GetDeliveredButNotReadAsync(
            int hoursDelivered = 24,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Add multiple messages in batch
        /// </summary>
        /// <param name="messages">Messages to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task AddRangeAsync(IEnumerable<CampaignMessage> messages, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update multiple messages in batch
        /// </summary>
        /// <param name="messages">Messages to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task UpdateRangeAsync(IEnumerable<CampaignMessage> messages, CancellationToken cancellationToken = default);

        /// <summary>
        /// Count messages for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Total count</returns>
        Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Count messages by status for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="status">Message status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Count</returns>
        Task<int> CountByCampaignAndStatusAsync(
            Guid campaignId,
            MessageStatus status,
            CancellationToken cancellationToken = default);
        /// <summary>
        /// Created a campaign message
        /// </summary>
        /// <param name="campaignMessage">The campaign message entity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task AddMessageAsync(CampaignMessage campaignMessage, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Increment the sent count for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task IncrementSentCountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    
    }
}
