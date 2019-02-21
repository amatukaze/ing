using System;
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
                name: "JNameTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JNameTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BattleDetailEntity",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    SortieFleetState = table.Column<byte[]>(nullable: true),
                    SortieFleet2State = table.Column<byte[]>(nullable: true),
                    SupportFleetState = table.Column<byte[]>(nullable: true),
                    LbasState = table.Column<byte[]>(nullable: true),
                    LandBaseDefence = table.Column<byte[]>(nullable: true),
                    FirstBattleDetail = table.Column<byte[]>(nullable: true),
                    SecondBattleDetail = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleDetailEntity", x => x.TimeStamp);
                    table.ForeignKey(
                        name: "FK_BattleDetailEntity_BattleTable_TimeStamp",
                        column: x => x.TimeStamp,
                        principalTable: "BattleTable",
                        principalColumn: "TimeStamp",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "JNameTable",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "" });

            migrationBuilder.CreateIndex(
                name: "IX_BattleTable_MapId",
                table: "BattleTable",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_JNameTable_Name",
                table: "JNameTable",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleDetailEntity");

            migrationBuilder.DropTable(
                name: "JNameTable");

            migrationBuilder.DropTable(
                name: "BattleTable");
        }
    }
}
