using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class YetkiliConfiguration : IEntityTypeConfiguration<Yetkili>
    {
        public void Configure(EntityTypeBuilder<Yetkili> builder)
        {
            builder.ToTable("PBIS_YETKILI", "OPUSER");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .IsRequired()
                   .HasColumnName("ID");

            builder.Property(x => x.CalisanId)
                   .IsRequired()
                   .HasColumnName("CALISAN_ID");

            builder.HasIndex(x => x.CalisanId).IsUnique();
        }
    }
}
