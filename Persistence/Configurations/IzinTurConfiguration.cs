using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinTurConfiguration : IEntityTypeConfiguration<IzinTur>
    {
        public void Configure(EntityTypeBuilder<IzinTur> builder)
        {
            builder.ToTable("PBIS_IZIN_TUR", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.Property(x => x.IzinFormTipi).HasColumnName("IZIN_FORM_TIPI").IsRequired();

            builder.Property(x => x.SabitGunSayisi).HasColumnName("SABIT_GUN_SAYISI").IsRequired(false);

            builder.Property(x => x.TatilGunleriSayilir).HasColumnName("TATIL_GUNLERI_SAYILIR").IsRequired();

            builder.Property(x => x.SenelikIzinMi).HasColumnName("SENELIK_IZIN_MI").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();

            builder.HasIndex(x => x.Aciklama).IsUnique();

            builder.HasMany(x => x.Izinler);
        }
    }
}
