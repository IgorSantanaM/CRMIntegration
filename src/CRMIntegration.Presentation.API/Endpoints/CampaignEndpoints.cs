using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
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
        }

        #region Handlers
        private static async Task<IResult> TriggerCampaignHandler([FromBody] TriggerCampaignCommand request, [FromServices] IMediator mediator)
        {
            await mediator.Send(request);
            return Results.Ok();
        }
        #endregion
    }
}
