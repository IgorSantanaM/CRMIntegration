namespace CRMIntegration.Domain.Clients.Common
{
    public record ClientStatistics(int TotalClients,
        int ActiveClients,
        int InactiveClients,
        int ActionableClients,
        int NonActionableClients,
        int SynchronizedWithVoll,
        int NotSynchronizedWithVoll,
        int ActionedInLast7Days,
        int ActionedInLast30Days,
        int NeverActioned,
        DateTime? OldestClientDate,
        DateTime? NewestClientDate);
}

