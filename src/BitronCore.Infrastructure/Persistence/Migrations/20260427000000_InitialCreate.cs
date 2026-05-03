using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BitronCore.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "inspecciones",
            columns: table => new
            {
                TransaccionId = table.Column<Guid>(type: "uuid", nullable: false),
                DispositivoId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Linea = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModeloDetectado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                TiempoTotalMs = table.Column<int>(type: "integer", nullable: false),
                VersionModeloKnn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                VeredictoGlobal = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                EvidenciaUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_inspecciones", x => x.TransaccionId);
            });

        migrationBuilder.CreateTable(
            name: "analisis_roi",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                InspeccionId = table.Column<Guid>(type: "uuid", nullable: false),
                Zona = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Densidad = table.Column<double>(type: "double precision", nullable: false),
                BrilloPromedio = table.Column<double>(type: "double precision", nullable: false),
                EsOk = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_analisis_roi", x => x.Id);
                table.ForeignKey(
                    name: "FK_analisis_roi_inspecciones_InspeccionId",
                    column: x => x.InspeccionId,
                    principalTable: "inspecciones",
                    principalColumn: "TransaccionId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_inspecciones_Timestamp", table: "inspecciones", column: "Timestamp");
        migrationBuilder.CreateIndex(name: "IX_inspecciones_DispositivoId", table: "inspecciones", column: "DispositivoId");
        migrationBuilder.CreateIndex(name: "IX_inspecciones_Linea", table: "inspecciones", column: "Linea");
        migrationBuilder.CreateIndex(name: "IX_inspecciones_VeredictoGlobal", table: "inspecciones", column: "VeredictoGlobal");
        migrationBuilder.CreateIndex(name: "IX_inspecciones_Linea_Timestamp", table: "inspecciones", columns: new[] { "Linea", "Timestamp" });
        migrationBuilder.CreateIndex(name: "IX_analisis_roi_InspeccionId", table: "analisis_roi", column: "InspeccionId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "analisis_roi");
        migrationBuilder.DropTable(name: "inspecciones");
    }
}
