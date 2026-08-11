using Microsoft.EntityFrameworkCore.Migrations;

namespace Risk.net.Data.Migrations
{
    public partial class OlayRaporlamaKategori : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OlayRaporlama_TanimOlayKategori_OlayKategoriKod",
                table: "OlayRaporlama");

            migrationBuilder.DropIndex(
                name: "IX_OlayRaporlama_OlayKategoriKod",
                table: "OlayRaporlama");

            migrationBuilder.CreateTable(
                name: "OlayRaporlamaKategori",
                columns: table => new
                {
                    Kod = table.Column<string>(type: "varchar(40)", nullable: false),
                    OlayRaporlamaKod = table.Column<string>(type: "varchar(40)", nullable: true),
                    OlayKategoriKod = table.Column<string>(type: "varchar(40)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlayRaporlamaKategori", x => x.Kod);
                    table.ForeignKey(
                        name: "FK_OlayRaporlamaKategori_OlayRaporlama_OlayRaporlamaKod",
                        column: x => x.OlayRaporlamaKod,
                        principalTable: "OlayRaporlama",
                        principalColumn: "Kod",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OlayRaporlamaKategori_TanimOlayKategori_OlayKategoriKod",
                        column: x => x.OlayKategoriKod,
                        principalTable: "TanimOlayKategori",
                        principalColumn: "Kod",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OlayRaporlamaKategori_OlayKategoriKod",
                table: "OlayRaporlamaKategori",
                column: "OlayKategoriKod");

            migrationBuilder.CreateIndex(
                name: "IX_OlayRaporlamaKategori_OlayRaporlamaKod",
                table: "OlayRaporlamaKategori",
                column: "OlayRaporlamaKod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OlayRaporlamaKategori");

            migrationBuilder.CreateIndex(
                name: "IX_OlayRaporlama_OlayKategoriKod",
                table: "OlayRaporlama",
                column: "OlayKategoriKod");

            migrationBuilder.AddForeignKey(
                name: "FK_OlayRaporlama_TanimOlayKategori_OlayKategoriKod",
                table: "OlayRaporlama",
                column: "OlayKategoriKod",
                principalTable: "TanimOlayKategori",
                principalColumn: "Kod",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
