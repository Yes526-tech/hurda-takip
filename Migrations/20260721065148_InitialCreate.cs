using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FireHurdaTakip.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HurdaKayitlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tarih = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ToplamPlastikHurdasiKg = table.Column<double>(type: "double precision", nullable: false),
                    AlinanSiparisNo = table.Column<string>(type: "text", nullable: true),
                    AkumulatorHurdasiKg = table.Column<double>(type: "double precision", nullable: false),
                    IzabeyeGonderilenHurdaKg = table.Column<double>(type: "double precision", nullable: false),
                    PEnjeksiyonaGonderilenHurdaKg = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HurdaKayitlar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HurdaKayitlar");
        }
    }
}
