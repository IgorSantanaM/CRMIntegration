namespace CRMIntegration.Domain.Campaings.Common
{
    public record CampaignStatistics(int TotalCampaigns,
        int TotalContacts,
        int TotalSent,
        int TotalDelivered,
        int TotalRead,
        int TotalFailed,
        decimal AverageDeliveryRate,
        decimal AverageReadRate,
        decimal AverageFailureRate,
        int CampaignsCreated,
        int CampaignsProcessing,
        int CampaignsSent,
        int CampaignsFinished,
        int CampaignsFailed,
        int CampaignsCancelled);
}
