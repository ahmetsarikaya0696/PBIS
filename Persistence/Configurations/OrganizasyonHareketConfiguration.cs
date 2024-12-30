using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class OrganizasyonHareketConfiguration : IEntityTypeConfiguration<OrganizasyonHareket>
    {
        public void Configure(EntityTypeBuilder<OrganizasyonHareket> builder)
        {
            builder.ToTable("PBIS_ORGANIZASYON_HAREKET", "OPUSER").HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();

            builder.Property(x => x.Ip).HasColumnName("IP").IsRequired();

            builder.Property(x => x.CalisanId).HasColumnName("ISLEM_YAPAN_CALISAN_ID").IsRequired();

            builder.Property(x => x.OrganizasyonId).HasColumnName("ORGANIZASYON_ID").IsRequired();

            builder.Property(x => x.Islem).HasColumnName("ISLEM").IsRequired();

            builder.Property(x => x.IslemTarihi).HasColumnName("ISLEM_TARIHI").IsRequired();
        }
    }
}
