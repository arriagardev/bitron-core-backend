using System;
using BitronCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BitronCore.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("BitronCore.Domain.Entities.AnalisisRoi", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer");

            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

            b.Property<double>("BrilloPromedio")
                .HasColumnType("double precision");

            b.Property<double>("Densidad")
                .HasColumnType("double precision");

            b.Property<bool>("EsOk")
                .HasColumnType("boolean");

            b.Property<Guid>("InspeccionId")
                .HasColumnType("uuid");

            b.Property<string>("Zona")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            b.HasKey("Id");
            b.HasIndex("InspeccionId");
            b.ToTable("analisis_roi");
        });

        modelBuilder.Entity("BitronCore.Domain.Entities.Inspeccion", b =>
        {
            b.Property<Guid>("TransaccionId")
                .HasColumnType("uuid");

            b.Property<DateTime>("CreadoEn")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<string>("DispositivoId")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            b.Property<string>("EvidenciaUrl")
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            b.Property<string>("Linea")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("character varying(50)");

            b.Property<string>("ModeloDetectado")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)");

            b.Property<DateTime>("Timestamp")
                .HasColumnType("timestamp with time zone");

            b.Property<int>("TiempoTotalMs")
                .HasColumnType("integer");

            b.Property<string>("VeredictoGlobal")
                .IsRequired()
                .HasMaxLength(2)
                .HasColumnType("character varying(2)");

            b.Property<string>("VersionModeloKnn")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)");

            b.HasKey("TransaccionId");
            b.HasIndex("DispositivoId");
            b.HasIndex("Linea");
            b.HasIndex("Timestamp");
            b.HasIndex("VeredictoGlobal");
            b.HasIndex(new[] { "Linea", "Timestamp" });
            b.ToTable("inspecciones");
        });

        modelBuilder.Entity("BitronCore.Domain.Entities.AnalisisRoi", b =>
        {
            b.HasOne("BitronCore.Domain.Entities.Inspeccion", "Inspeccion")
                .WithMany("AnalisisRois")
                .HasForeignKey("InspeccionId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Inspeccion");
        });

        modelBuilder.Entity("BitronCore.Domain.Entities.Inspeccion", b =>
        {
            b.Navigation("AnalisisRois");
        });
#pragma warning restore 612, 618
    }
}
