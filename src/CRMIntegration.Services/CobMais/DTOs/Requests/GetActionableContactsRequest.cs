namespace CRMIntegration.Services.CobMais.DTOs.Requests
{
    public record GetActionableContactsRequest(DateTime? StartDate,
        DateTime? EndDate,
        DateTime? ChangeStartDate = null,
        DateTime? ChangeEndDate = null);
}
