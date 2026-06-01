using CRMIntegration.Infra.Services.Exceptions;
using CRMIntegration.Services.BemChat;
using CRMIntegration.Services.BemChat.DTOs.Requests;
using CRMIntegration.Services.BemChat.DTOs.Responses;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRMIntegration.Infra.Services.BemChat
{
    public class BemChatService : IBemChatService
    {
        private readonly HttpClient _httpClient;
        private readonly BemChatOptions _options;
        private readonly ILogger<BemChatService> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public BemChatService(
            IHttpClientFactory httpClientFactory,
            BemChatOptions options,
            ILogger<BemChatService> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("BemChat");
        }

        /// <inheritdoc />
        public async Task<BemChatSendMessageResponse> SendTextMessageAsync(
            SendTextMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Number))
                throw new ArgumentException("Número do destinatário é obrigatório.", nameof(request));

            if (string.IsNullOrWhiteSpace(request.Body))
                throw new ArgumentException("O corpo da mensagem é obrigatório.", nameof(request));

            var body = new
            {
                number = NormalizeNumber(request.Number),
                body = request.Body,
                whatsappId = request.WhatsAppId ?? _options.DefaultWhatsAppId,
            };

            _logger.LogInformation("[BemChat] Enviando mensagem para {Number}.", body.number);

            var response = await _httpClient
                .PostAsJsonAsync("/api/messages/send", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "SEND_TEXT_MESSAGE");

            var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogInformation("[BemChat] Mensagem enviada para {Number}. Status={Status}.",
                body.number, (int)response.StatusCode);

            return new BemChatSendMessageResponse(
                Success: true,
                MessageId: TryExtractMessageId(rawContent),
                Error: null
            );
        }

        /// <inheritdoc />
        public async Task<BemChatSendMessageResponse> SendMediaMessageAsync(
            SendMediaMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Number))
                throw new ArgumentException("Número do destinatário é obrigatório.", nameof(request));

            if (request.MediaStream is null)
                throw new ArgumentException("Stream de mídia é obrigatório.", nameof(request));

            var form = new MultipartFormDataContent();

            form.Add(new StringContent(NormalizeNumber(request.Number)), "number");

            var fileContent = new StreamContent(request.MediaStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
            form.Add(fileContent, "medias", request.FileName);

            if (!string.IsNullOrWhiteSpace(request.Body))
                form.Add(new StringContent(request.Body), "body");

            var whatsAppId = request.WhatsAppId ?? _options.DefaultWhatsAppId;
            if (whatsAppId.HasValue)
                form.Add(new StringContent(whatsAppId.Value.ToString()), "whatsappId");

            _logger.LogInformation("[BemChat] Enviando mídia '{File}' para {Number}.",
                request.FileName, request.Number);

            var response = await _httpClient
                .PostAsync("/api/messages/send", form, cancellationToken);

            await EnsureSuccessAsync(response, "SEND_MEDIA_MESSAGE");

            var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return new BemChatSendMessageResponse(
                Success: true,
                MessageId: TryExtractMessageId(rawContent),
                Error: null
            );
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "[BemChat] {Operation} falhou. StatusCode={StatusCode} Body={Body}",
                    operation, (int)response.StatusCode, content);

                throw new BemChatIntegrationException(
                    $"BemChat API error on {operation}: {(int)response.StatusCode} — {content}",
                    (int)response.StatusCode);
            }
        }

        /// <summary>
        /// Garante que o número está no formato DDI+DDD+número. Ex: 5567999990001
        /// Remove caracteres não numéricos.
        /// </summary>
        private static string NormalizeNumber(string number) =>
            new(number.Where(char.IsDigit).ToArray());

        /// <summary>
        /// Tenta extrair o ID da mensagem do body de resposta (JSON livre).
        /// </summary>
        private static string? TryExtractMessageId(string rawContent)
        {
            try
            {
                using var doc = JsonDocument.Parse(rawContent);
                if (doc.RootElement.TryGetProperty("id", out var id))
                    return id.GetString();
                if (doc.RootElement.TryGetProperty("messageId", out var msgId))
                    return msgId.GetString();
            }
            catch
            {
            }
            return null;
        }
    }
}
