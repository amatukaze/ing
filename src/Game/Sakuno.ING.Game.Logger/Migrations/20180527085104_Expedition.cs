using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class Expedition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpeditionCompletionTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    ExpeditionId = table.Column<int>(nullable: false),
                    ExpeditionName = table.Column<string>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    MaterialsAcquired_Fuel = table.Column<int>(nullable: false),
                    MaterialsAcquired_Bullet = table.Column<int>(nullable: false),
                    MaterialsAcquired_Steel = table.Column<int>(nullable: false),
                    MaterialsAcquired_Bauxite = table.Column<int>(nullable: false),
                    RewardItem1_ItemId = table.Column<int>(nullable: false),
                    RewardItem1_Count = table.Column<int>(nullable: false),
                    RewardItem2_ItemId = table.Column<int>(nullable: false),
                    RewardItem2_Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionCompletionTable", x => x.TimeStamp);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpeditionCompletionTable");
        }
    }
}
