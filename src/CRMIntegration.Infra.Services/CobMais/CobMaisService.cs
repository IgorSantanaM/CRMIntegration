using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.CobMais.DTOs;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using CRMIntegration.Services.CobMais.DTOs.Responses;

namespace CRMIntegration.Infra.Services.CobMais
{
    public class CobMaisService : ICobMaisService
    {
        public Task<IEnumerable<ActionableContactDto>> GetActionableContactsAsync(GetActionableContactsRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CobMaisClientResponse>> GetClientDataAsync(GetActionableContactsRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CobMaisPhoneResponse>> GetClientPhonesAsync(string chaveCliente, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task InsertCampaignDispatchEventAsync(string codigoCliente, string templateName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CobMaisInsertEventResponse> InsertEventAsync(InsertEventRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task MarkPhoneAsActionableAsync(int phoneId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task MarkPhoneAsNonActionableAsync(int phoneId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task MarkPhonesAsNonActionableBatchAsync(IEnumerable<int> phoneIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CobMaisPhoneUpdateResponse> UpdatePhoneAsync(UpdatePhoneRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
