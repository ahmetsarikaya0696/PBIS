using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinGrubuConfiguration : IEntityTypeConfiguration<IzinGrup>
    {
        public void Configure(EntityTypeBuilder<IzinGrup> builder)
        {
            builder.ToTable("PBIS_IZIN_GRUBU", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.Property(x => x.CalisanId).HasColumnName("CALISAN_ID").IsRequired(false);

            builder.Property(x => x.UnvanId).HasColumnName("UNVAN_ID").IsRequired(false);

            builder.Property(x => x.BirimId).HasColumnName("BIRIM_ID").IsRequired(false);

            builder.Property(x => x.IsyeriId).HasColumnName("ISYERI_ID").IsRequired(false);

            builder.Property(x => x.BaslangicTarihi).HasColumnName("BASLANGIC_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.BitisTarihi).HasColumnName("BITIS_TARIHI").HasColumnType("DATE").IsRequired();

            builder.HasMany(x => x.IzinGrupIzinOnayTanimlari);

            builder.HasIndex(x => x.CalisanId).IsUnique();

            builder.HasIndex(x => x.UnvanId).IsUnique(false);

            builder.HasIndex(x => x.BirimId).IsUnique(false);

            builder.HasIndex(x => x.IsyeriId).IsUnique(false);
        }
    }
}
