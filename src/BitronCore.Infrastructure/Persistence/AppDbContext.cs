using BitronCore.Domain.Entities;
using BitronCore.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BitronCore.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Inspeccion> Inspecciones => Set<Inspeccion>();
    public DbSet<AnalisisRoi> AnalisisRois => Set<AnalisisRoi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InspeccionConfiguration());
        modelBuilder.ApplyConfiguration(new AnalisisRoiConfiguration());
    }
}
