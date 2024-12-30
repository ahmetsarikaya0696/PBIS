using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IsyeriConfiguration : IEntityTypeConfiguration<Isyeri>
    {
        public void Configure(EntityTypeBuilder<Isyeri> builder)
        {
            builder.ToTable("PBIS_ISYERI", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();

            builder.HasIndex(x => x.Aciklama).IsUnique();

            builder.HasMany(x => x.Calisanlar);

            builder.HasMany(x => x.Izinler);
        }
    }
}
