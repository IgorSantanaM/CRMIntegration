using CRMIntegration.Domain.Clients;
using CRMIntegration.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRMIntegration.Infra.Data.Mappings
{
    public class ClientMapping : EntityTypeConfiguration<Client>
    {
        public override void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.IdBemChat)
                .HasMaxLength(100);

            builder.Property(x => x.IdCobMais)
                .IsRequired();

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Whatsapp)
                .IsRequired()
                .HasMaxLength(13);

            builder.Property(x => x.CPFCNPJ)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(x => x.IdTelefoneCobMais);

            builder.Property(x => x.Acionavel)
                .IsRequired();

            builder.Property(x => x.Ativo)
                .IsRequired();

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            builder.Property(x => x.DataUltimoAcionamento);

            builder.Property(x => x.DataSincronizacaoBemChat);

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.HasIndex(x => x.IdCobMais)
                .IsUnique();

            builder.HasIndex(x => x.IdBemChat)
                .IsUnique();
        }
    }
}
