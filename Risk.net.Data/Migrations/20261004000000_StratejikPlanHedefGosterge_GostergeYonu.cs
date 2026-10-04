using Microsoft.EntityFrameworkCore.Migrations;

namespace Risk.net.Data.Migrations
{
    public partial class StratejikPlanHedefGosterge_GostergeYonu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GostergeYonu",
                table: "StratejikPlanHedefGosterge",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GostergeYonu",
                table: "StratejikPlanHedefGosterge");
        }
    }
}
