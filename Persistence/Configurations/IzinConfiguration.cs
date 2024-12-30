using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinConfiguration : IEntityTypeConfiguration<Izin>
    {
        public void Configure(EntityTypeBuilder<Izin> builder)
        {
            builder.ToTable("PBIS_IZIN", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.CalisanId).HasColumnName("CALISAN_ID").IsRequired();

            builder.Property(x => x.Ip).HasColumnName("IP").IsRequired();

            builder.Property(x => x.IstekTarihi).HasColumnName("ISTEK_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.BaslangicTarihi).HasColumnName("BASLANGIC_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.BitisTarihi).HasColumnName("BITIS_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.MahsubenBaslangicTarihi).HasColumnName("MAHSUBEN_BASLANGIC_TARIHI").HasColumnType("DATE").IsRequired(false);

            builder.Property(x => x.IseBaslamaTarihi).HasColumnName("ISE_BASLAMA_TARIHI").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.IzinDurumId).HasColumnName("IZIN_DURUM_ID").IsRequired();

            builder.Property(x => x.YerineBakacakKisi).HasColumnName("YERINE_BAKACAK_KISI").IsRequired(false);

            builder.Property(x => x.Adres).HasColumnName("ADRES").IsRequired(false);

            builder.Property(x => x.Telefon).HasColumnName("TELEFON").IsRequired(false);

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired(false);

            builder.Property(x => x.YillikIzinUcretiIstegi).HasColumnName("YILLIK_IZIN_UCRET_ISTEGI").IsRequired();

            builder.Property(x => x.IzinTurId).HasColumnName("IZIN_TUR_ID").IsRequired();

            builder.Property(x => x.IsyeriId).HasColumnName("ISYERI_ID").IsRequired();

            builder.Property(x => x.BirimId).HasColumnName("BIRIM_ID").IsRequired();

            builder.Property(x => x.UnvanId).HasColumnName("UNVAN_ID").IsRequired();


            builder.HasIndex(x => x.CalisanId).IsUnique(false);

            builder.HasIndex(x => x.IsyeriId).IsUnique(false);

            builder.HasIndex(x => x.BirimId).IsUnique(false);

            builder.HasIndex(x => x.UnvanId).IsUnique(false);


            builder.HasOne(x => x.Calisan);

            builder.HasMany(x => x.IzinHareketleri);

            builder.HasOne(x => x.IzinDurum);

            builder.HasOne(x => x.IzinTur);

            builder.HasOne(x => x.Isyeri);

            builder.HasOne(x => x.Birim);

            builder.HasOne(x => x.Unvan);

        }
    }
}
