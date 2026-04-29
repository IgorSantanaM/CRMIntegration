using CRMIntegration.Services.CobMais.DTOs;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using CRMIntegration.Services.CobMais.DTOs.Responses;

namespace CRMIntegration.Services.CobMais
{
    public interface ICobMaisService
    {
        /// <summary>
        /// [POST /consulta/clientes/dados_cadastrais]
        /// Returns all clients with their phones, emails and addresses.
        /// </summary>
        Task<IEnumerable<CobMaisClientResponse>> GetClientDataAsync(
            GetActionableContactsRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /consulta/clientes/dados_cadastrais]
        /// Returns only clients that have at least one phone with contato=true and ativo=true.
        /// Phones are filtered: only whatsapp=true OR all movel phones when whatsapp is null.
        /// This is the primary method for campaign targeting.
        /// </summary>
        Task<IEnumerable<ActionableContactDto>> GetActionableContactsAsync(
            GetActionableContactsRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [GET /cobranca/clientes/{ChaveCliente}/telefones]
        /// Returns all phones for a specific client.
        /// ChaveCliente can be id_pessoa or cpfcnpj.
        /// </summary>
        Task<IEnumerable<CobMaisPhoneResponse>> GetClientPhonesAsync(
            string chaveCliente,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [PUT /cobranca/clientes/telefones]
        /// Updates phone data — primarily used to set contato=false after successful campaign send.
        /// </summary>
        Task<CobMaisPhoneUpdateResponse> UpdatePhoneAsync(
            UpdatePhoneRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [PUT /cobranca/clientes/telefones]
        /// Marks phone as non-actionable (contato=false).
        /// Called after a successful WhatsApp message delivery.
        /// </summary>
        Task MarkPhoneAsNonActionableAsync(
            int phoneId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [PUT /cobranca/clientes/telefones]
        /// Marks phone as actionable (contato=true).
        /// Called when a WhatsApp message fails to allow retry.
        /// </summary>
        Task MarkPhoneAsActionableAsync(
            int phoneId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [PUT /cobranca/clientes/telefones]
        /// Marks multiple phones as non-actionable in batch.
        /// Called at the end of a successful campaign send.
        /// </summary>
        Task MarkPhonesAsNonActionableBatchAsync(
            IEnumerable<int> phoneIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /cobranca/eventos]
        /// Inserts a CobMais event — used to register campaign dispatch as a collection event.
        /// </summary>
        Task<CobMaisInsertEventResponse> InsertEventAsync(
            InsertEventRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /cobranca/eventos]
        /// Inserts a WhatsApp campaign dispatch event for a client.
        /// Convenience method that wraps InsertEventAsync with fixed event type.
        /// </summary>
        Task InsertCampaignDispatchEventAsync(
            string codigoCliente,
            string templateName,
            CancellationToken cancellationToken = default);
    }
}
