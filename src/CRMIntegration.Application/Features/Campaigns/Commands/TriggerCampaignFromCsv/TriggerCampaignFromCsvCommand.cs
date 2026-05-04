using CRMIntegration.Application.Features.Campaigns.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaignFromCsv
{
    public record TriggerCampaignFromCsvCommand(IFormFile CsvFile, string TemplateName)
        : IRequest<TriggerCampaignFromCsvResponse>;
}
