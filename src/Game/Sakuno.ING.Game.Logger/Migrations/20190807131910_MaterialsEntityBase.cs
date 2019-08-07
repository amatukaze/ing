using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class MaterialsEntityBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "MaterialsChangeTable",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "BattleConsumptionTable",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "MaterialsChangeTable");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "BattleConsumptionTable");
        }
    }
}
