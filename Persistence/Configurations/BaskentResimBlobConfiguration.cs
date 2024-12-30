using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class BaskentResimBlobConfiguration : IEntityTypeConfiguration<BaskentResimBlob>
    {
        public void Configure(EntityTypeBuilder<BaskentResimBlob> builder)
        {
            builder.ToTable("BASKENT_RESIM_BLOB", "OPUSER").HasNoKey();

            builder.Property(x => x.LREF).HasColumnName("LREF").HasColumnType("NUMBER(11)").IsRequired(false);

            builder.Property(x => x.RESIM).HasColumnName("RESIM").HasColumnType("BLOB").IsRequired(false);
        }
    }
}
