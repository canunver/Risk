using Microsoft.EntityFrameworkCore.Migrations;

namespace Risk.net.Data.Migrations
{
    public partial class RiskEvreni_KontrolEdildi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KontrolEdildi",
                table: "RiskEvreni",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KontrolEdildi",
                table: "RiskEvreni");
        }
    }
}
