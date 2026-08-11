using Microsoft.EntityFrameworkCore.Migrations;

namespace Risk.net.Data.Migrations
{
    public partial class StratejikPlanIzlemeDonemDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GerceklesenDegerYilSonu",
                table: "StratejikPlanIzlemeDonem",
                type: "decimal(13,4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "GerceklesenDeger",
                table: "StratejikPlanIzlemeDonem",
                type: "decimal(13,4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GerceklesenDegerYilSonu",
                table: "StratejikPlanIzlemeDonem",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,4)");

            migrationBuilder.AlterColumn<int>(
                name: "GerceklesenDeger",
                table: "StratejikPlanIzlemeDonem",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(13,4)");
        }
    }
}
