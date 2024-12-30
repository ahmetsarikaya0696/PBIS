using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class CalisanConfiguration : IEntityTypeConfiguration<Calisan>
    {
        public void Configure(EntityTypeBuilder<Calisan> builder)
        {
            builder.ToTable("PBIS_CALISAN", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Ad).HasColumnName("AD").IsRequired();

            builder.Property(x => x.Soyad).HasColumnName("SOYAD").IsRequired();

            builder.Property(x => x.Tc).HasColumnName("TC").HasColumnType("VARCHAR2(11)").IsRequired();

            builder.Property(x => x.SicilNo).HasColumnName("SICIL_NO").IsRequired();

            builder.Property(x => x.IsyeriId).HasColumnName("ISYERI_ID").IsRequired();

            builder.Property(x => x.BirimId).HasColumnName("BIRIM_ID").IsRequired();

            builder.Property(x => x.UnvanId).HasColumnName("UNVAN_ID").IsRequired();

            builder.Property(x => x.DogumTarihi).HasColumnName("DOGUM_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.IseBaslamaTarihi).HasColumnName("ISE_BASLAMA_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.IstenCikisTarihi).HasColumnName("ISTEN_CIKIS_TARIHI").HasColumnType("DATE").IsRequired(false);

            builder.Property(x => x.KullaniciAdi).HasColumnName("KULLANICI_ADI").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();

            builder.Property(x => x.LREF).HasColumnName("LREF").IsRequired(false);


            builder.HasIndex(x => x.Tc).IsUnique();

            builder.HasIndex(x => x.SicilNo).IsUnique();

            builder.HasIndex(x => x.KullaniciAdi).IsUnique();

            builder.HasIndex(x => x.IsyeriId).IsUnique(false);

            builder.HasIndex(x => x.BirimId).IsUnique(false);

            builder.HasIndex(x => x.UnvanId).IsUnique(false);

            builder.HasMany(x => x.Izinler);

            builder.HasMany(x => x.IzinOnayTanimCalisanlar);

            builder.HasOne(x => x.Isyeri);

            builder.HasOne(x => x.Unvan);

            builder.HasMany(x => x.IzinHareketleri);

            builder.Property(x => x.Telefon)
                   .IsRequired()
                   .HasColumnName("TELEFON");
        }
    }
}
