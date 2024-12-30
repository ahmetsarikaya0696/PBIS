using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinOnayTanimCalisanConfiguration : IEntityTypeConfiguration<IzinOnayTanimCalisan>
    {
        public void Configure(EntityTypeBuilder<IzinOnayTanimCalisan> builder)
        {
            builder.ToTable("PBIS_IZIN_ONAY_TANIM_CALISAN", "OPUSER").HasKey(x => new { x.IzinOnayTanimId, x.CalisanId });

            builder.Property(x => x.IzinOnayTanimId).HasColumnName("IZIN_ONAY_TANIM_ID").IsRequired();

            builder.Property(x => x.CalisanId).HasColumnName("CALISAN_ID").IsRequired();

            builder.HasOne(x => x.Calisan);

            builder.HasOne(x => x.IzinOnayTanim);
        }
    }
}
