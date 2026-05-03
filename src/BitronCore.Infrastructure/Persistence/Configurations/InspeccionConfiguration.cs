using BitronCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitronCore.Infrastructure.Persistence.Configurations;

public class InspeccionConfiguration : IEntityTypeConfiguration<Inspeccion>
{
    public void Configure(EntityTypeBuilder<Inspeccion> builder)
    {
        builder.ToTable("inspecciones");

        builder.HasKey(x => x.TransaccionId);
        builder.Property(x => x.TransaccionId).ValueGeneratedNever();

        builder.Property(x => x.DispositivoId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Linea).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ModeloDetectado).HasMaxLength(20).IsRequired();
        builder.Property(x => x.VersionModeloKnn).HasMaxLength(20).IsRequired();
        builder.Property(x => x.VeredictoGlobal).HasMaxLength(2).IsRequired();
        builder.Property(x => x.EvidenciaUrl).HasMaxLength(500);
        builder.Property(x => x.CreadoEn).HasDefaultValueSql("now()");

        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => x.DispositivoId);
        builder.HasIndex(x => x.Linea);
        builder.HasIndex(x => x.VeredictoGlobal);
        builder.HasIndex(x => new { x.Linea, x.Timestamp });

        builder.HasMany(x => x.AnalisisRois)
               .WithOne(x => x.Inspeccion)
               .HasForeignKey(x => x.InspeccionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
