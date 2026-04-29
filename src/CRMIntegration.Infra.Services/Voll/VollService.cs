using CRMIntegration.Services.Voll;
using CRMIntegration.Services.Voll.DTOs.Requests;
using CRMIntegration.Services.Voll.DTOs.Responses;

namespace CRMIntegration.Infra.Services.Voll
{
    public class VollService : IVollService
    {
        public Task<VollCampaignResponse> CreateBulkCampaignAsync(BulkCampaignRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VollContactResponse> CreateContactAsync(CreateContactRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VollCampaignResponse>> GetAllCampaignsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VollCampaignReportResponse> GetCampaignReportAsync(string campaignId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetOrCreateContactAsync(CreateContactRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VollTemplateResponse?> GetTemplateByNameAsync(string templateName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VollTemplateResponse>> GetTemplatesAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetWebhookAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VollContactResponse?> SearchContactAsync(SearchContactRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VollContactResponse?> SearchContactByWhatsAppAsync(string whatsapp, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VollSendMessageResponse> SendTemplateMessageAsync(SendTemplateMessageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetWebhookAsync(string url, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateContactCustomFieldsAsync(string chatId, UpdateCustomFieldRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
