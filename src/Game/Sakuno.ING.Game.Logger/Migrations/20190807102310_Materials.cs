using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class Materials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleConsumptionTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    MapId = table.Column<int>(nullable: false),
                    Consumption_Fuel = table.Column<int>(nullable: false),
                    Consumption_Bullet = table.Column<int>(nullable: false),
                    Consumption_Steel = table.Column<int>(nullable: false),
                    Consumption_Bauxite = table.Column<int>(nullable: false),
                    Consumption_InstantBuild = table.Column<int>(nullable: false),
                    Consumption_InstantRepair = table.Column<int>(nullable: false),
                    Consumption_Development = table.Column<int>(nullable: false),
                    Consumption_Improvement = table.Column<int>(nullable: false),
                    ActualConsumption_Fuel = table.Column<int>(nullable: false),
                    ActualConsumption_Bullet = table.Column<int>(nullable: false),
                    ActualConsumption_Steel = table.Column<int>(nullable: false),
                    ActualConsumption_Bauxite = table.Column<int>(nullable: false),
                    ActualConsumption_InstantBuild = table.Column<int>(nullable: false),
                    ActualConsumption_InstantRepair = table.Column<int>(nullable: false),
                    ActualConsumption_Development = table.Column<int>(nullable: false),
                    ActualConsumption_Improvement = table.Column<int>(nullable: false),
                    Acquired_Fuel = table.Column<int>(nullable: false),
                    Acquired_Bullet = table.Column<int>(nullable: false),
                    Acquired_Steel = table.Column<int>(nullable: false),
                    Acquired_Bauxite = table.Column<int>(nullable: false),
                    Acquired_InstantBuild = table.Column<int>(nullable: false),
                    Acquired_InstantRepair = table.Column<int>(nullable: false),
                    Acquired_Development = table.Column<int>(nullable: false),
                    Acquired_Improvement = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleConsumptionTable", x => x.TimeStamp);
                });

            migrationBuilder.CreateTable(
                name: "HomeportStateTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeportStateTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialsChangeTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Materials_Fuel = table.Column<int>(nullable: false),
                    Materials_Bullet = table.Column<int>(nullable: false),
                    Materials_Steel = table.Column<int>(nullable: false),
                    Materials_Bauxite = table.Column<int>(nullable: false),
                    Materials_InstantBuild = table.Column<int>(nullable: false),
                    Materials_InstantRepair = table.Column<int>(nullable: false),
                    Materials_Development = table.Column<int>(nullable: false),
                    Materials_Improvement = table.Column<int>(nullable: false),
                    Reason = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialsChangeTable", x => x.TimeStamp);
                });

            migrationBuilder.CreateTable(
                name: "QuestProcessTable",
                columns: table => new
                {
                    QuestId = table.Column<int>(nullable: false),
                    CounterId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestProcessTable", x => new { x.QuestId, x.CounterId });
                });

            migrationBuilder.CreateTable(
                name: "ShipSortieStateTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    LastSortie = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipSortieStateTable", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleConsumptionTable");

            migrationBuilder.DropTable(
                name: "HomeportStateTable");

            migrationBuilder.DropTable(
                name: "MaterialsChangeTable");

            migrationBuilder.DropTable(
                name: "QuestProcessTable");

            migrationBuilder.DropTable(
                name: "ShipSortieStateTable");
        }
    }
}
