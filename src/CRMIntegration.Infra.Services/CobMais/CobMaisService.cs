using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.CobMais.DTOs;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using CRMIntegration.Services.CobMais.DTOs.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRMIntegration.Infra.Services.CobMais
{
    public class CobMaisService : ICobMaisService
    {
        private readonly HttpClient _httpClient;
        private readonly CobMaisOptions _options;
        private readonly ILogger<CobMaisService> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public CobMaisService(
            HttpClient httpClient,
            IOptions<CobMaisOptions> options,
            ILogger<CobMaisService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CobMaisClientResponse>> GetClientDataAsync(
            GetActionableContactsRequest request,
            CancellationToken cancellationToken = default)
        {
            var body = new
            {
                data_inicial = request.StartDate?.ToString("yyyy-MM-dd"),
                data_final = request.EndDate?.ToString("yyyy-MM-dd"),
                data_alteracao_inicial = request.ChangeStartDate?.ToString("yyyy-MM-dd"),
                data_alteracao_final = request.ChangeEndDate?.ToString("yyyy-MM-dd"),
            };

            _logger.LogInformation("[CobMais] Fetching client data. StartDate={StartDate} EndDate={EndDate}",
                request.StartDate, request.EndDate);

            var response = await _httpClient
                .PostAsJsonAsync("/clientes/dados_cadastrais", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "GET_CLIENT_DATA");

            var result = await response.Content
                .ReadFromJsonAsync<List<CobMaisClientResponse>>(_jsonOptions, cancellationToken);

            _logger.LogInformation("[CobMais] Fetched {Count} clients.", result?.Count ?? 0);

            return result ?? [];
        }

        public async Task<IEnumerable<ActionableContactDto>> GetActionableContactsAsync(
            GetActionableContactsRequest request,
            CancellationToken cancellationToken = default)
        {
            var clients = await GetClientDataAsync(request, cancellationToken);

            var actionable = clients
                .SelectMany(client =>
                {
                    var validPhones = client.Telefones
                        .Where(t => t.Ativo && t.Contato == true)
                        .ToList();

                    var whatsappPhone = validPhones.FirstOrDefault(t => t.Whatsapp == true);

                    var fallbackPhone = validPhones.FirstOrDefault(t =>
                        string.Equals(t.Tipo, "MOVEL", StringComparison.OrdinalIgnoreCase));

                    var phone = whatsappPhone ?? fallbackPhone ?? validPhones.FirstOrDefault();

                    if (phone is null || string.IsNullOrWhiteSpace(phone.Numero))
                        return [];

                    var normalizedNumber = NormalizePhoneNumber(phone.Numero);
                    if (string.IsNullOrEmpty(normalizedNumber))
                        return [];

                    return (IEnumerable<ActionableContactDto>)[new ActionableContactDto(
                        client.IdPessoa,
                        client.CpfCnpj,
                        client.Nome,
                        client.Codigo,
                        phone.Id,
                        normalizedNumber,
                        phone.Whatsapp ?? false,
                        client.Emails.FirstOrDefault(e => e.Ativo)?.Email
                    )];
                })
                .ToList();

            _logger.LogInformation(
                "[CobMais] {Total} actionable contacts found from {ClientCount} clients.",
                actionable.Count, clients.Count());

            return actionable;
        }

        public async Task<IEnumerable<CobMaisPhoneResponse>> GetClientPhonesAsync(
            string chaveCliente,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(chaveCliente))
                throw new ArgumentNullException(nameof(chaveCliente));

            _logger.LogDebug("[CobMais] Fetching phones for client {ChaveCliente}.", chaveCliente);

            var response = await _httpClient
                .GetAsync($"/clientes/{chaveCliente}/telefones", cancellationToken);

            await EnsureSuccessAsync(response, "GET_CLIENT_PHONES");

            var result = await response.Content
                .ReadFromJsonAsync<ClientePhonesWrapper>(_jsonOptions, cancellationToken);

            return result?.Telefones ?? [];
        }

        public async Task<CobMaisPhoneUpdateResponse> UpdatePhoneAsync(
            UpdatePhoneRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var body = new
            {
                telefone = new
                {
                    id = request.Id,
                    tipo = request.Tipo,
                    numero = request.Numero,
                    ramal = request.Ramal,
                    observacao = request.Observacao,
                    ativo = request.Ativo,
                    contato = request.Contato,
                    whatsapp = request.Whatsapp,
                }
            };

            _logger.LogDebug("[CobMais] Updating phone {PhoneId}. Contato={Contato}.",
                request.Id, request.Contato);

            var response = await _httpClient
                .PutAsJsonAsync("/clientes/telefones", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "UPDATE_PHONE");

            var wrapper = await response.Content
                .ReadFromJsonAsync<ClientePhonesWrapper>(_jsonOptions, cancellationToken);

            var updated = wrapper?.Telefones?.FirstOrDefault(t => t.Id == request.Id);

            return new CobMaisPhoneUpdateResponse(
                updated?.Id ?? request.Id,
                updated?.Tipo ?? request.Tipo,
                updated?.Numero ?? request.Numero,
                updated?.Ativo ?? request.Ativo,
                updated?.Contato ?? request.Contato,
                updated?.Whatsapp ?? false
            );
        }

        public async Task MarkPhoneAsNonActionableAsync(
            int phoneId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[CobMais] Marking phone {PhoneId} as non-actionable.", phoneId);

            await UpdatePhoneAsync(new UpdatePhoneRequest
            (
                phoneId,
                "MOVEL",
                "",
                null,
                null,
                true,
                false,
                true
            ), cancellationToken);
        }

        public async Task MarkPhoneAsActionableAsync(
            int phoneId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[CobMais] Reverting phone {PhoneId} to actionable.", phoneId);

            await UpdatePhoneAsync(new UpdatePhoneRequest
            (
                phoneId,
                "MOVEL",
                "",
                null,
                null,
                true,
                true,
                true
            ), cancellationToken);
        }

        public async Task MarkPhonesAsNonActionableBatchAsync(
            IEnumerable<int> phoneIds,
            CancellationToken cancellationToken = default)
        {
            var ids = phoneIds.ToList();

            _logger.LogInformation(
                "[CobMais] Marking {Count} phones as non-actionable in batch.", ids.Count);

            var semaphore = new SemaphoreSlim(5);
            var tasks = ids.Select(async id =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await MarkPhoneAsNonActionableAsync(id, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[CobMais] Failed to mark phone {PhoneId} as non-actionable.", id);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        public async Task<CobMaisInsertEventResponse> InsertEventAsync(
            InsertEventRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var body = new
            {
                codigo_cliente = request.CodigoCliente,
                contrato = request.Contrato,
                data_evento = request.DataEvento,
                tipo_evento = request.TipoEvento,
                descricao_evento = request.DescricaoEvento,
                tipo_canal = request.TipoCanal,
                identificacao = request.Identificacao,
            };

            _logger.LogDebug(
                "[CobMais] Inserting event '{TipoEvento}' for client {CodigoCliente}.",
                request.TipoEvento, request.CodigoCliente);

            var response = await _httpClient
                .PostAsJsonAsync("/eventos", body, _jsonOptions, cancellationToken);

            await EnsureSuccessAsync(response, "INSERT_EVENT");

            var result = await response.Content
                .ReadFromJsonAsync<CobMaisInsertEventResponse>(_jsonOptions, cancellationToken);

            return result ?? new CobMaisInsertEventResponse(null, true, null);
        }

        public async Task InsertCampaignDispatchEventAsync(
            string codigoCliente,
            string templateName,
            CancellationToken cancellationToken = default)
        {
            await InsertEventAsync(new InsertEventRequest
            (
                codigoCliente,
                null,
                DateTime.Now.ToString("yyyy-MM-dd"),
                _options.CampaignEventType,
                $"Envio de mensagem WhatsApp via campanha. Template: {templateName}",
                _options.EventChannelType,
                null
            ), cancellationToken);
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response, string operation)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "[CobMais] {Operation} failed. StatusCode={StatusCode} Body={Body}",
                    operation, (int)response.StatusCode, content);

                throw new Exception(
                    $"CobMais API error on {operation}: {(int)response.StatusCode} — {content}");
            }
        }

        private static string NormalizePhoneNumber(string rawNumber)
        {
            var digits = new string(rawNumber.Where(char.IsDigit).ToArray());
            if (digits.StartsWith("55") && digits.Length is 12 or 13)
                return digits;
            if (digits.Length is 10 or 11)
                return $"55{digits}";

            return string.Empty;
        }

        private sealed class ClientePhonesWrapper
        {
            [JsonPropertyName("telefones")]
            public List<CobMaisPhoneResponse>? Telefones { get; set; }
        }
    }
}
