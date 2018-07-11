using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class ResetDbModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentCreationTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: true),
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
                name: "ExpeditionCompletionTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    ExpeditionId = table.Column<int>(nullable: false),
                    ExpeditionName = table.Column<string>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    MaterialsAcquired_Fuel = table.Column<int>(nullable: false),
                    MaterialsAcquired_Bullet = table.Column<int>(nullable: false),
                    MaterialsAcquired_Steel = table.Column<int>(nullable: false),
                    MaterialsAcquired_Bauxite = table.Column<int>(nullable: false),
                    RewardItem1_ItemId = table.Column<int>(nullable: true),
                    RewardItem1_Count = table.Column<int>(nullable: true),
                    RewardItem2_ItemId = table.Column<int>(nullable: true),
                    RewardItem2_Count = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionCompletionTable", x => x.TimeStamp);
                });

            migrationBuilder.CreateTable(
                name: "ShipCreationTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: true),
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
                name: "ExpeditionCompletionTable");

            migrationBuilder.DropTable(
                name: "ShipCreationTable");
        }
    }
}
