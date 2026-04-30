namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign
{
    public record SendContactMessageCommand(Guid CampaignId,
        Guid CorrelationId,
        int IdPessoa,
        string CpfCnpj,
        string Nome,
        string PhoneNumber,
        int PhoneId,
        string TemplateName);
    
}