using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class RetDetayConfiguration : IEntityTypeConfiguration<RetDetay>
    {
        public void Configure(EntityTypeBuilder<RetDetay> builder)
        {
            builder.ToTable("PBIS_RET_DETAY", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Detay).HasColumnName("DETAY").IsRequired();

            builder.Property(x => x.IzinHareketId).HasColumnName("IZIN_HAREKET_ID").IsRequired();

            builder.Property(x => x.RetSebepId).HasColumnName("RET_SEBEP_ID").IsRequired();

            builder.HasOne(x => x.IzinHareket)
                   .WithOne(x => x.RetDetay)
                   .HasForeignKey<RetDetay>(x => x.IzinHareketId)
                   .IsRequired();
        }
    }
}
