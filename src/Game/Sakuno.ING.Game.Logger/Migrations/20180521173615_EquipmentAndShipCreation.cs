using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class EquipmentAndShipCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentCreationTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Consumption_Fuel = table.Column<int>(nullable: false),
                    Consumption_Bullet = table.Column<int>(nullable: false),
                    Consumption_Steel = table.Column<int>(nullable: false),
                    Consumption_Bauxite = table.Column<int>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    EquipmentCreated = table.Column<int>(nullable: false),
                    Secretary = table.Column<int>(nullable: false),
                    SecretaryLevel = table.Column<int>(nullable: false),
                    AdmiralLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCreationTable", x => x.TimeStamp);
                });

            migrationBuilder.CreateTable(
                name: "ShipCreationTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Consumption_Fuel = table.Column<int>(nullable: false),
                    Consumption_Bullet = table.Column<int>(nullable: false),
                    Consumption_Steel = table.Column<int>(nullable: false),
                    Consumption_Bauxite = table.Column<int>(nullable: false),
                    Consumption_Development = table.Column<int>(nullable: false),
                    IsLSC = table.Column<bool>(nullable: false),
                    ShipBuilt = table.Column<int>(nullable: false),
                    EmptyDockCount = table.Column<int>(nullable: false),
                    Secretary = table.Column<int>(nullable: false),
                    SecretaryLevel = table.Column<int>(nullable: false),
                    AdmiralLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipCreationTable", x => x.TimeStamp);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentCreationTable");

            migrationBuilder.DropTable(
                name: "ShipCreationTable");
        }
    }
}
