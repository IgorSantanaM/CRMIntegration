using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace CRMIntegration.Infra.Data.Contexts
{
    public class CRMIntegrationContext(DbContextOptions<CRMIntegrationContext> options) : DbContext(options)
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<CampaignMessage> CampaignMessages { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Event<Guid>>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CRMIntegrationContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
