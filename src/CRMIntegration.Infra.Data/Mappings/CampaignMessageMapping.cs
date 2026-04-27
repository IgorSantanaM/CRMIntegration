using CRMIntegration.Domain.Campaings;
using CRMIntegration.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMIntegration.Infra.Data.Mappings
{
    public class CampaignMessageMapping : EntityTypeConfiguration<CampaignMessage>
    {
        public override void Configure(EntityTypeBuilder<CampaignMessage> builder)
        {
            builder.ToTable("CampaignMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.CampaignId)
                .IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.Property(x => x.IdMensagemVoll)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.DataEnvio)
                .IsRequired();

            builder.Property(x => x.DataEntrega);

            builder.Property(x => x.DataLeitura);

            builder.Property(x => x.DataFalha);

            builder.Property(x => x.WebhookPayload);

            builder.Property(x => x.MensagemErro)
                .HasMaxLength(500);

            builder.HasOne(x => x.Campaign)
                .WithMany(x => x.Mensagens)
                .HasForeignKey(x => x.CampaignId);

            builder.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId);

            builder.HasIndex(x => new { x.CampaignId, x.ClientId })
                .IsUnique();
        }
    }
}
