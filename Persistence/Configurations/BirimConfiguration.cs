using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class BirimConfiguration : IEntityTypeConfiguration<Birim>
    {
        public void Configure(EntityTypeBuilder<Birim> builder)
        {
            builder.ToTable("PBIS_BIRIM", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();

            builder.HasIndex(x => x.Aciklama).IsUnique();

            builder.HasMany(x => x.Calisanlar);

            builder.HasMany(x => x.Izinler);

            builder.HasOne(x => x.OrganizasyonSemasi)
                   .WithOne(x => x.Birim)
                   .HasForeignKey<Organizasyon>(x => x.BirimId);
        }
    }
}
