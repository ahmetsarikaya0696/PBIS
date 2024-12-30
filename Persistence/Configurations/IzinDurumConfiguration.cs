using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinDurumConfiguration : IEntityTypeConfiguration<IzinDurum>
    {
        public void Configure(EntityTypeBuilder<IzinDurum> builder)
        {
            builder.ToTable("PBIS_IZIN_DURUM", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.HasIndex(x => x.Aciklama).IsUnique();

            builder.HasMany(x => x.Izinler);

            builder.HasMany(x => x.IzinHareketleri);
        }
    }
}
