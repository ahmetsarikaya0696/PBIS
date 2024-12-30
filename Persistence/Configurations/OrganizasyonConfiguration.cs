using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class OrganizasyonConfiguration : IEntityTypeConfiguration<Organizasyon>
    {
        public void Configure(EntityTypeBuilder<Organizasyon> builder)
        {
            builder.ToTable("PBIS_ORGANIZASYON", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Kod).HasColumnName("KOD").IsRequired();

            builder.Property(x => x.Aciklama_TR).HasColumnName("ACIKLAMA_TR").IsRequired();

            builder.Property(x => x.Aciklama_EN).HasColumnName("ACIKLAMA_EN").IsRequired();

            builder.Property(x => x.OrganizasyonKodu).HasColumnName("ORGANIZASYON_KODU").IsRequired(false);

            builder.Property(x => x.AnaBirim).HasColumnName("ANA_BIRIM").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();

            builder.Property(x => x.UstBirimId).HasColumnName("UST_BIRIM_ID").IsRequired();

            builder.Property(x => x.BirimId).HasColumnName("BIRIM_ID").IsRequired(false);

            builder.HasIndex(x => x.Kod).IsUnique();
        }
    }
}
