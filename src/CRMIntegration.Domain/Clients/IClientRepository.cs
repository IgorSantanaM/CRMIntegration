using CRMIntegration.Domain.Clients.Common;
using CRMIntegration.Domain.Clients.Filters;
using CRMIntegration.Domain.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Clients
{
    /// <summary>
    /// Repository interface for Client aggregate root
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Get a client by CobMais ID
        /// </summary>
        /// <param name="idCobMais">CobMais client ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Client if found, null otherwise</returns>
        Task<Client?> GetByIdCobMaisAsync(int idCobMais, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a client by Voll ID
        /// </summary>
        /// <param name="idVoll">Voll client ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Client if found, null otherwise</returns>
        Task<Client?> GetByIdVollAsync(string idVoll, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a client by WhatsApp number
        /// </summary>
        /// <param name="whatsapp">WhatsApp number</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Client if found, null otherwise</returns>
        Task<Client?> GetByWhatsAppAsync(string whatsapp, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a client by CPF/CNPJ
        /// </summary>
        /// <param name="cpfCnpj">CPF or CNPJ</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Client if found, null otherwise</returns>
        Task<Client?> GetByCPFCNPJAsync(string cpfCnpj, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a client by email
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Client if found, null otherwise</returns>
        Task<Client?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all actionable clients (can receive messages)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of actionable clients</returns>
        Task<PagedResult<Client>> GetActionableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all active clients
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of active clients</returns>
        Task<PagedResult<Client>> GetActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients that are actionable and active (ready for campaigns)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients ready for campaigns</returns>
        Task<PagedResult<Client>> GetReadyForCampaignAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients synchronized with Voll
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of synchronized clients</returns>
        Task<PagedResult<Client>> GetSynchronizedWithVollAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients not synchronized with Voll yet
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients pending synchronization</returns>
        Task<PagedResult<Client>> GetNotSynchronizedWithVollAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients that need resynchronization with Voll (old sync date)
        /// </summary>
        /// <param name="daysToExpire">Days after which synchronization is considered expired</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients needing resynchronization</returns>
        Task<PagedResult<Client>> GetNeedingResynchronizationAsync(
            int daysToExpire = 30,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients created within a date range
        /// </summary>
        /// <param name="startDate">Start date (inclusive)</param>
        /// <param name="endDate">End date (inclusive)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients</returns>
        Task<PagedResult<Client>> GetByCreatedDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients that were recently actioned
        /// </summary>
        /// <param name="days">Number of days to consider as recent</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of recently actioned clients</returns>
        Task<PagedResult<Client>> GetRecentlyActionedAsync(
            int days = 7,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients that haven't been actioned recently
        /// </summary>
        /// <param name="days">Minimum number of days since last action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients not recently actioned</returns>
        Task<PagedResult<Client>> GetNotRecentlyActionedAsync(
            int days = 30,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get inactive clients
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of inactive clients</returns>
        Task<PagedResult<Client>> GetInactiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients marked as non-actionable
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of non-actionable clients</returns>
        Task<PagedResult<Client>> GetNonActionableAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Search clients by name (partial match)
        /// </summary>
        /// <param name="name">Name to search</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of matching clients</returns>
        Task<PagedResult<Client>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated list of clients
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients</returns>
        Task<PagedResult<Client>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated list of clients with filtering
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="filter">Client filter criteria</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of filtered clients</returns>
        Task<PagedResult<Client>> GetPaginatedWithFilterAsync(
            int pageNumber,
            int pageSize,
            ClientFilter filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get client statistics
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Client statistics</returns>
        Task<ClientStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a client with the given WhatsApp number already exists
        /// </summary>
        /// <param name="whatsapp">WhatsApp number</param>
        /// <param name="excludeId">Client ID to exclude from check (for updates)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsWithWhatsAppAsync(
            string whatsapp,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a client with the given CPF/CNPJ already exists
        /// </summary>
        /// <param name="cpfCnpj">CPF or CNPJ</param>
        /// <param name="excludeId">Client ID to exclude from check (for updates)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsWithCPFCNPJAsync(
            string cpfCnpj,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if a client with the given CobMais ID already exists
        /// </summary>
        /// <param name="idCobMais">CobMais ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsWithCobMaisIdAsync(int idCobMais, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients by multiple IDs
        /// </summary>
        /// <param name="ids">List of client IDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients</returns>
        Task<PagedResult<Client>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get clients by multiple CobMais IDs
        /// </summary>
        /// <param name="idsCobMais">List of CobMais IDs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged result of clients</returns>
        Task<PagedResult<Client>> GetByCobMaisIdsAsync(
            IEnumerable<int> idsCobMais,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Add multiple clients in batch
        /// </summary>
        /// <param name="clients">Clients to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task AddRangeAsync(IEnumerable<Client> clients, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update multiple clients in batch
        /// </summary>
        /// <param name="clients">Clients to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task UpdateRangeAsync(IEnumerable<Client> clients, CancellationToken cancellationToken = default);

        /// <summary>
        /// Count total clients
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Total count</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Count active clients
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Active clients count</returns>
        Task<int> CountActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Count actionable clients
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Actionable clients count</returns>
        Task<int> CountActionableAsync(CancellationToken cancellationToken = default);
    }
}
