using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class IzinGrupIzinOnayTanimConfiguration : IEntityTypeConfiguration<IzinGrupIzinOnayTanim>
    {
        public void Configure(EntityTypeBuilder<IzinGrupIzinOnayTanim> builder)
        {
            builder.ToTable("PBIS_IZIN_GRUP_IZIN_ONAY_TANIM", "OPUSER").HasKey(x => new { x.IzinGrupId, x.IzinOnayTanimId });

            builder.Property(x => x.IzinOnayTanimId).HasColumnName("IZIN_ONAY_TANIM_ID").IsRequired();

            builder.Property(x => x.IzinGrupId).HasColumnName("IZIN_GRUP_ID").IsRequired();

            builder.Property(x => x.OnayTanimSirasi).HasColumnName("ONAY_TANIM_SIRASI").IsRequired();

            builder.HasOne(x => x.IzinGrup);

            builder.HasOne(x => x.IzinOnayTanim);
        }
    }
}
