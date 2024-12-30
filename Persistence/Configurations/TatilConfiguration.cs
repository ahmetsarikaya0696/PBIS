using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class TatilConfiguration : IEntityTypeConfiguration<Tatil>
    {
        public void Configure(EntityTypeBuilder<Tatil> builder)
        {
            builder.ToTable("PBIS_TATIL", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Tarih).HasColumnName("TARIH").HasColumnType("DATE").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.HasIndex(x => x.Tarih).IsUnique();
        }
    }
}
