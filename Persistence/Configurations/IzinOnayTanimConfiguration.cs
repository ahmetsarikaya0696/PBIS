using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinOnayTanimConfiguration : IEntityTypeConfiguration<IzinOnayTanim>
    {
        public void Configure(EntityTypeBuilder<IzinOnayTanim> builder)
        {
            builder.ToTable("PBIS_IZIN_ONAY_TANIM", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.Property(x => x.PersonelSubeYetkisi).HasColumnName("PERSONEL_SUBE_YETKISI_VAR").IsRequired();

            builder.Property(x => x.MerkezMuduruYetkisi).HasColumnName("MERKEZ_MUDUR_YETKISI_VAR").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();

            builder.HasIndex(x => x.Aciklama).IsUnique();

            builder.HasMany(x => x.IzinGrupIzinOnayTanimlari);

            builder.HasMany(x => x.IzinHareketleri);

            builder.HasMany(x => x.OnayTanimCalisanlar);
        }
    }
}
