namespace CRMIntegration.Domain.Campaings.Common
{
    public record MessageStatistics(int TotalMessages,
        int TotalSent,
        int TotalDelivered,
        int TotalRead,
        int TotalFailed,
        decimal DeliveryRate,
        decimal ReadRate,
        decimal FailureRate,
        double? AverageDeliveryTimeSeconds,
        double? AverageReadTimeSeconds);
}
