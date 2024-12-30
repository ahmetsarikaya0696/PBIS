using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class DogrulamaConfiguration : IEntityTypeConfiguration<Dogrulama>
    {
        public void Configure(EntityTypeBuilder<Dogrulama> builder)
        {
            builder.ToTable("PBIS_DOGRULAMA", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Kod).HasColumnName("KOD").HasColumnType("VARCHAR2(6)").IsRequired();

            builder.Property(x => x.SonKullanimTarihi).HasColumnName("SON_KULLANIM_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.Yontem).HasColumnName("YONTEM").IsRequired();

            builder.Property(x => x.Gecerli).HasColumnName("GECERLI").IsRequired();

            builder.Property(x => x.CalisanId).HasColumnName("CALISAN_ID").IsRequired();

            builder.Property(x => x.IzinId).HasColumnName("IZIN_ID").IsRequired(false);
        }
    }
}
