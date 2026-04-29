using CRMIntegration.Services.Voll.DTOs.Requests;
using CRMIntegration.Services.Voll.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll
{
    public interface IVollService
    {
        /// <summary>
        /// [POST /crm/chat]
        /// Creates a new contact in Voll CRM.
        /// Returns the created contact's chat ID.
        /// </summary>
        Task<VollContactResponse> CreateContactAsync(
            CreateContactRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /crm/chat/search]
        /// Searches for a contact by WhatsApp number, email or custom field.
        /// Returns null when not found.
        /// </summary>
        Task<VollContactResponse?> SearchContactAsync(
            SearchContactRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /crm/chat/search?search_field=whatsapp]
        /// Convenience: searches contact by WhatsApp number.
        /// Returns null when not found.
        /// </summary>
        Task<VollContactResponse?> SearchContactByWhatsAppAsync(
            string whatsapp,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ensures a contact exists in Voll: searches first, creates if not found.
        /// Returns the contact's Voll chat ID.
        /// </summary>
        Task<string> GetOrCreateContactAsync(
            CreateContactRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /crm/chat/setting/{chat-id}/custom-field]
        /// Adds or updates custom fields for an existing contact.
        /// </summary>
        Task UpdateContactCustomFieldsAsync(
            string chatId,
            UpdateCustomFieldRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /crm/campaign]
        /// Creates and triggers a bulk WhatsApp campaign with template messages.
        /// Each receiver can have individual variable substitution.
        /// </summary>
        Task<VollCampaignResponse> CreateBulkCampaignAsync(
            BulkCampaignRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /crm/campaign]
        /// Returns all campaigns.
        /// </summary>
        Task<IEnumerable<VollCampaignResponse>> GetAllCampaignsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /crm/campaign/{campaign_id}/report]
        /// Returns delivery and read report for a campaign.
        /// Used to reconcile webhook events.
        /// </summary>
        Task<VollCampaignReportResponse> GetCampaignReportAsync(
            string campaignId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /v1/messages]
        /// Sends a WhatsApp template message to a single recipient.
        /// Returns the message ID (wamid) for webhook tracking.
        /// </summary>
        Task<VollSendMessageResponse> SendTemplateMessageAsync(
            SendTemplateMessageRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /v1/configs/templates]
        /// Returns paginated list of approved WhatsApp templates.
        /// </summary>
        Task<IEnumerable<VollTemplateResponse>> GetTemplatesAsync(
            int limit = 10,
            int offset = 0,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /v1/configs/templates]
        /// Returns a template by name.
        /// Returns null if template does not exist or is not approved.
        /// </summary>
        Task<VollTemplateResponse?> GetTemplateByNameAsync(
            string templateName,
            CancellationToken cancellationToken = default);

        // ---- Webhook: /v1/configs/webhook ----

        /// <summary>
        /// [POST /v1/configs/webhook]
        /// Registers or updates the webhook URL for message status events.
        /// </summary>
        Task SetWebhookAsync(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /v1/configs/webhook]
        /// Returns the currently configured webhook URL.
        /// </summary>
        Task<string?> GetWebhookAsync(CancellationToken cancellationToken = default);
    }
}
