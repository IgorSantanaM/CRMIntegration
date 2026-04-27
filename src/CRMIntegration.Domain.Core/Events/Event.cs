using MediatR;

namespace CRMIntegration.Domain.Core.Events
{
    public abstract record Event<TId> : Message<TId>, INotification where TId : notnull
    {
        public DateTime TimeStamp { get; set; }
        protected Event(TId aggregateId) : base(aggregateId)
        {
            TimeStamp = DateTime.UtcNow;
        }
    }
}
