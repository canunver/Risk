using Microsoft.EntityFrameworkCore.Migrations;

namespace Risk.net.Data.Migrations
{
    public partial class RiskYonetimi_RiskAzaltmaPlani_KontrolEdildi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KontrolEdildi",
                table: "RiskYonetimi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KontrolEdildi",
                table: "RiskAzaltmaPlani",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KontrolEdildi",
                table: "RiskYonetimi");

            migrationBuilder.DropColumn(
                name: "KontrolEdildi",
                table: "RiskAzaltmaPlani");
        }
    }
}
