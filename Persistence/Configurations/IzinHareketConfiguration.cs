using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinHareketConfiguration : IEntityTypeConfiguration<IzinHareket>
    {
        public void Configure(EntityTypeBuilder<IzinHareket> builder)
        {
            builder.ToTable("PBIS_IZIN_HAREKET", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Ip).HasColumnName("IP").IsRequired();

            builder.Property(x => x.CalisanId).HasColumnName("ISLEM_YAPAN_CALISAN_ID").IsRequired();

            builder.Property(x => x.IzinId).HasColumnName("IZIN_ID").IsRequired();

            builder.Property(x => x.Sira).HasColumnName("SIRA").IsRequired(false);

            builder.Property(x => x.IslemTarihi).HasColumnName("ISLEM_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.IzinOnayTanimId).HasColumnName("IZIN_ONAY_TANIM_ID").IsRequired(false);

            builder.Property(x => x.IzinDurumId).HasColumnName("IZIN_DURUM_ID").IsRequired();

            builder.HasIndex(x => x.IzinId).IsUnique(false);

            builder.HasIndex(x => x.CalisanId).IsUnique(false);

            builder.HasOne(x => x.IzinDurum);

            builder.HasOne(x => x.Izin);

            builder.HasOne(x => x.IzinOnayTanim);

            builder.HasOne(x => x.Calisan);
        }
    }
}
