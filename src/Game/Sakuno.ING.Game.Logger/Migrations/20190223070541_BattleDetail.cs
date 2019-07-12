using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class BattleDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    CompletionTime = table.Column<long>(nullable: false),
                    MapId = table.Column<int>(nullable: false),
                    MapName = table.Column<string>(nullable: true),
                    RouteId = table.Column<int>(nullable: false),
                    EventKind = table.Column<int>(nullable: false),
                    BattleKind = table.Column<int>(nullable: false),
                    CombinedFleetType = table.Column<int>(nullable: false),
                    MapRank = table.Column<int>(nullable: true),
                    MapGaugeType = table.Column<int>(nullable: true),
                    MapGaugeNumber = table.Column<int>(nullable: true),
                    MapGaugeHP = table.Column<int>(nullable: true),
                    MapGaugeMaxHP = table.Column<int>(nullable: true),
                    Rank = table.Column<int>(nullable: true),
                    AdmiralExperience = table.Column<int>(nullable: true),
                    BaseExperience = table.Column<int>(nullable: true),
                    MapCleared = table.Column<bool>(nullable: true),
                    EnemyFleetName = table.Column<string>(nullable: true),
                    UseItemAcquired = table.Column<int>(nullable: true),
                    ShipDropped = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleTable", x => x.TimeStamp);
                });

            migrationBuilder.CreateTable(
                name: "BattleDetails",
                columns: table => new
                {
                    BattleEntityTimeStamp = table.Column<long>(nullable: false),
                    SortieFleetState = table.Column<byte[]>(nullable: true),
                    SortieFleet2State = table.Column<byte[]>(nullable: true),
                    SupportFleetState = table.Column<byte[]>(nullable: true),
                    LbasState = table.Column<byte[]>(nullable: true),
                    LandBaseDefence = table.Column<string>(nullable: true),
                    FirstBattleDetail = table.Column<string>(nullable: true),
                    SecondBattleDetail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleDetails", x => x.BattleEntityTimeStamp);
                    table.ForeignKey(
                        name: "FK_BattleDetails_BattleTable_BattleEntityTimeStamp",
                        column: x => x.BattleEntityTimeStamp,
                        principalTable: "BattleTable",
                        principalColumn: "TimeStamp",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleTable_MapId",
                table: "BattleTable",
                column: "MapId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleDetails");

            migrationBuilder.DropTable(
                name: "BattleTable");
        }
    }
}
