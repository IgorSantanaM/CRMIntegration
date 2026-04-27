using CRMIntegration.Domain.Campaings;
using CRMIntegration.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMIntegration.Infra.Data.Mappings
{
    public class CampaignMapping : EntityTypeConfiguration<Campaign>
    {
        public override void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.IdCampanhaVoll)
                .HasMaxLength(100);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Template)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ChannelIdVoll)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DataDisparo)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.TotalContatos)
                .IsRequired();

            builder.Property(x => x.DataCriacao);

            builder.Property(x => x.DataFinalizacao);

            builder.Property(x => x.TotalEnviados)
                .IsRequired();

            builder.Property(x => x.TotalEntregues)
                .IsRequired();

            builder.Property(x => x.TotalFalhas)
                .IsRequired();

            builder.Property(x => x.TotalLidos)
                .IsRequired();

            builder.HasMany(x => x.Mensagens)
                .WithOne(x => x.Campaign)
                .HasForeignKey(x => x.CampaignId);

            builder.HasIndex(x => x.IdCampanhaVoll)
                .IsUnique();
        }
    }
}
