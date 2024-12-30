using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class RetSebepConfiguration : IEntityTypeConfiguration<RetSebep>
    {
        public void Configure(EntityTypeBuilder<RetSebep> builder)
        {
            builder.ToTable("PBIS_RET_SEBEP", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Aciklama).HasColumnName("ACIKLAMA").IsRequired();

            builder.HasIndex(x => x.Aciklama).IsUnique();

            builder.Property(x => x.Duzenlenebilir).HasColumnName("DUZENLENEBILIR").IsRequired();

            builder.Property(x => x.PersonelSubeyeOzgu).HasColumnName("PERSONEL_SUBEYE_OZGU").IsRequired();

            builder.Property(x => x.Aktif).HasColumnName("AKTIF").IsRequired();
        }
    }
}
