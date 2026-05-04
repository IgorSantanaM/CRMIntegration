using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaignFromCsv;
using CRMIntegration.Presentation.API.Endpoints.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMIntegration.Presentation.API.Endpoints
{
    public class CampaignEndpoints : IEndpoint
    {
        public static void DefineEndpoints(WebApplication app)
        {
            var group = app.MapGroup("/api/campaigns").WithTags("Campaigns");

            group.MapPost("/trigger", TriggerCampaignHandler)
                .WithName("TriggerCampaign")
                .WithDescription("Triggers a campaign by its ID.");

            group.MapPost("/trigger-from-csv", TriggerCampaignFromCsvHandler)
                .WithName("TriggerCampaignFromCsv")
                .WithDescription("Triggers a campaign using a CSV file with contact information.");
        }

        #region Handlers
        private static async Task<IResult> TriggerCampaignHandler([FromBody] TriggerCampaignCommand request, [FromServices] IMediator mediator)
        {
            await mediator.Send(request);
            return Results.Ok();
        }

        private static async Task<IResult> TriggerCampaignFromCsvHandler([FromBody] TriggerCampaignFromCsvCommand request, [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(request);
            return Results.Ok(result);
        }
        #endregion
    }
}
