using BitronCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitronCore.Infrastructure.Persistence.Configurations;

public class AnalisisRoiConfiguration : IEntityTypeConfiguration<AnalisisRoi>
{
    public void Configure(EntityTypeBuilder<AnalisisRoi> builder)
    {
        builder.ToTable("analisis_roi");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.Zona).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Densidad).HasColumnType("double precision");
        builder.Property(x => x.BrilloPromedio).HasColumnType("double precision");

        builder.HasIndex(x => x.InspeccionId);
    }
}
