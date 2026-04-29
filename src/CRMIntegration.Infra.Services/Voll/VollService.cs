using CRMIntegration.Services.Voll;
using CRMIntegration.Services.Voll.DTOs.Requests;
using CRMIntegration.Services.Voll.DTOs.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRMIntegration.Infra.Services.Voll
{
    public class VollService : IVollService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VollService> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public VollService(
            IHttpClientFactory httpClientFactory,
            ILogger<VollService> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Voll");
        }


        public async Task<VollContactResponse> CreateContactAsync(
            CreateContactRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Whatsapp))
                throw new ArgumentException("WhatsApp number is required.", nameof(request));

            var body = new
            {
                whatsapp = request.Whatsapp,
                email = request.Email,
                chat_name = request.ChatName,
                custom_fields = request.CustomFields?.Select(f => new { id = f.Id, value = f.Value }),
                chat_operator = request.ChatOperator,
            };

            _logger.LogInformation("[Voll] Creating contact WhatsApp={Whatsapp} Name={Name}.",
                request.Whatsapp, request.ChatName);

            var response = await _httpClient
                .PostAsJsonAsync("/crm/chat", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "CREATE_CONTACT");

            var result = await response.Content
                .ReadFromJsonAsync<VollContactResponse>(_jsonOptions, cancellationToken);

            _logger.LogInformation("[Voll] Contact created. VollId={VollId}.", result?.Id);

            return result ?? throw new VollIntegrationException("Empty response on CREATE_CONTACT.", 200);
        }

        public async Task<VollContactResponse?> SearchContactAsync(
            SearchContactRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var url = BuildSearchUrl(request);

            _logger.LogDebug("[Voll] Searching contact. Field={Field} Value={Value}.",
                request.SearchField, request.SearchValue);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            await EnsureSuccessAsync(response, "SEARCH_CONTACT");

            var results = await response.Content
                .ReadFromJsonAsync<List<VollContactResponse>>(_jsonOptions, cancellationToken);

            return results?.FirstOrDefault();
        }

        public async Task<VollContactResponse?> SearchContactByWhatsAppAsync(
            string whatsapp,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(whatsapp))
                throw new ArgumentNullException(nameof(whatsapp));

            return await SearchContactAsync(new SearchContactRequest
            (
                "whatsapp",
                whatsapp,
                "equal to",
                null
            ), cancellationToken);
        }

        public async Task<string> GetOrCreateContactAsync(
            CreateContactRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("[Voll] GetOrCreate contact WhatsApp={Whatsapp}.", request.Whatsapp);

            var existing = await SearchContactByWhatsAppAsync(request.Whatsapp, cancellationToken);

            if (existing is not null)
            {
                _logger.LogDebug("[Voll] Contact already exists. VollId={VollId}.", existing.Id);
                return existing.Id;
            }

            var created = await CreateContactAsync(request, cancellationToken);
            return created.Id;
        }
        public async Task UpdateContactCustomFieldsAsync(
            string chatId,
            UpdateCustomFieldRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(chatId))
                throw new ArgumentNullException(nameof(chatId));

            var body = new
            {
                data = request.Data.Select(f => new { id = f.Id, value = f.Value })
            };

            _logger.LogDebug("[Voll] Updating custom fields for chat {ChatId}.", chatId);

            var response = await _httpClient
                .PostAsJsonAsync($"/crm/chat/setting/{chatId}/custom-field", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "UPDATE_CUSTOM_FIELDS");
        }

        public async Task<VollCampaignResponse> CreateBulkCampaignAsync(
            BulkCampaignRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (!request.Receivers.Any())
                throw new ArgumentException("Campaign must have at least one receiver.", nameof(request));

            if (!request.Messages.Any())
                throw new ArgumentException("Campaign must have at least one message.", nameof(request));

            var body = new
            {
                receivers = request.Receivers.Select(r => new
                {
                    to = r.To,
                    variables = r.Variables,
                }),
                title = request.Title,
                channel = request.Channel,
                action = request.Action,
                mode = request.Mode,
                schedule = request.Schedule,
                messages = request.Messages.Select(m => new
                {
                    type = m.Type,
                    template = new
                    {
                        language = new
                        {
                            policy = m.Template.Language.Policy,
                            code = m.Template.Language.Code,
                        },
                        name = m.Template.Name,
                        components = m.Template.Components.Select(c => new
                        {
                            type = c.Type,
                            parameters = c.Parameters.Select(p => new
                            {
                                type = p.Type,
                                text = p.Text,
                            }),
                        }),
                    },
                }),
            };

            _logger.LogInformation(
                "[Voll] Creating bulk campaign '{Title}' with {Count} receivers.",
                request.Title, request.Receivers.Count);

            var response = await _httpClient
                .PostAsJsonAsync("/crm/campaign", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "CREATE_BULK_CAMPAIGN");

            var result = await response.Content
                .ReadFromJsonAsync<VollCampaignResponse>(_jsonOptions, cancellationToken);

            _logger.LogInformation("[Voll] Campaign created. VollCampaignId={CampaignId}.", result?.Id);

            return result ?? throw new VollIntegrationException("Empty response on CREATE_BULK_CAMPAIGN.", 200);
        }
        public async Task<IEnumerable<VollCampaignResponse>> GetAllCampaignsAsync(
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("[Voll] Fetching all campaigns.");

            var response = await _httpClient.GetAsync("/crm/campaign", cancellationToken);

            await EnsureSuccessAsync(response, "GET_ALL_CAMPAIGNS");

            var result = await response.Content
                .ReadFromJsonAsync<List<VollCampaignResponse>>(_jsonOptions, cancellationToken);

            return result ?? [];
        }

        public async Task<VollCampaignReportResponse> GetCampaignReportAsync(
            string campaignId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));

            _logger.LogDebug("[Voll] Fetching report for campaign {CampaignId}.", campaignId);

            var response = await _httpClient
                .GetAsync($"/crm/campaign/{campaignId}/report", cancellationToken);

            await EnsureSuccessAsync(response, "GET_CAMPAIGN_REPORT");

            var result = await response.Content
                .ReadFromJsonAsync<VollCampaignReportResponse>(_jsonOptions, cancellationToken);

            return result ?? throw new VollIntegrationException("Empty response on GET_CAMPAIGN_REPORT.", 200);
        }
        public async Task<VollSendMessageResponse> SendTemplateMessageAsync(
            SendTemplateMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.To))
                throw new ArgumentException("Recipient number is required.", nameof(request));

            var body = new
            {
                to = request.To,
                recipient_type = request.RecipientType,
                type = request.Type,
                schedule = request.Schedule,
                template = new
                {
                    language = new
                    {
                        policy = request.Template.Language.Policy,
                        code = request.Template.Language.Code,
                    },
                    name = request.Template.Name,
                    components = request.Template.Components.Select(c => new
                    {
                        type = c.Type,
                        parameters = c.Parameters.Select(p => new { type = p.Type, text = p.Text }),
                    }),
                },
            };

            _logger.LogInformation("[Voll] Sending template '{Template}' to {To}.",
                request.Template.Name, request.To);

            var response = await _httpClient
                .PostAsJsonAsync("/v1/messages", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "SEND_TEMPLATE_MESSAGE");

            var result = await response.Content
                .ReadFromJsonAsync<VollSendMessageResponse>(_jsonOptions, cancellationToken);

            _logger.LogInformation("[Voll] Message sent. MessageId={MessageId}.", result?.MessageId);

            return result ?? throw new VollIntegrationException("Empty response on SEND_TEMPLATE_MESSAGE.", 200);
        }

        public async Task<IEnumerable<VollTemplateResponse>> GetTemplatesAsync(
            int limit = 10,
            int offset = 0,
            CancellationToken cancellationToken = default)
        {
            var url = $"/v1/configs/templates?limit={limit}&offset={offset}&fetch_count=true&json=true";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            await EnsureSuccessAsync(response, "GET_TEMPLATES");

            var result = await response.Content
                .ReadFromJsonAsync<VollTemplatesWrapper>(_jsonOptions, cancellationToken);

            return result?.Templates ?? [];
        }

        public async Task<VollTemplateResponse?> GetTemplateByNameAsync(
            string templateName,
            CancellationToken cancellationToken = default)
        {
            var templates = await GetTemplatesAsync(100, 0, cancellationToken);

            return templates.FirstOrDefault(t =>
                string.Equals(t.Name, templateName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.Status, "APPROVED", StringComparison.OrdinalIgnoreCase));
        }

        public async Task SetWebhookAsync(string url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            _logger.LogInformation("[Voll] Setting webhook URL={Url}.", url);

            var body = new { url };

            var response = await _httpClient
                .PostAsJsonAsync("/v1/configs/webhook", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "SET_WEBHOOK");
        }

        public async Task<string?> GetWebhookAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("/v1/configs/webhook", cancellationToken);

            await EnsureSuccessAsync(response, "GET_WEBHOOK");

            var result = await response.Content
                .ReadFromJsonAsync<VollWebhookResponse>(_jsonOptions, cancellationToken);

            return result?.Url;
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "[Voll] {Operation} failed. StatusCode={StatusCode} Body={Body}",
                    operation, (int)response.StatusCode, content);

                throw new VollIntegrationException(
                    $"Voll API error on {operation}: {(int)response.StatusCode} — {content}",
                    (int)response.StatusCode);
            }
        }

        private static string BuildSearchUrl(SearchContactRequest request)
        {
            var url = $"/crm/chat/search?search_field={Uri.EscapeDataString(request.SearchField)}" +
                      $"&search_value={Uri.EscapeDataString(request.SearchValue)}" +
                      $"&condition={Uri.EscapeDataString(request.Condition)}";

            if (!string.IsNullOrWhiteSpace(request.Fields))
                url += $"&fields={Uri.EscapeDataString(request.Fields)}";

            return url;
        }

        private sealed class VollTemplatesWrapper
        {
            [JsonPropertyName("templates")]
            public List<VollTemplateResponse>? Templates { get; set; }
        }

        private sealed class VollWebhookResponse
        {
            [JsonPropertyName("url")]
            public string? Url { get; set; }
        }
    }

    public class VollIntegrationException : Exception
    {
        public int StatusCode { get; }

        public VollIntegrationException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
