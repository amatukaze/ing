using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class Exercise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBattleDetail",
                table: "BattleTable",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLandBaseDefense",
                table: "BattleTable",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExerciseTable",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    EnemyId = table.Column<int>(nullable: false),
                    EnemyLevel = table.Column<int>(nullable: false),
                    EnemyName = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: true),
                    AdmiralExperience = table.Column<int>(nullable: true),
                    BaseExperience = table.Column<int>(nullable: true),
                    EnemyFleetName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTable", x => x.TimeStamp);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseDetails",
                columns: table => new
                {
                    ExerciseEntityTimeStamp = table.Column<long>(nullable: false),
                    SortieFleetState = table.Column<byte[]>(nullable: true),
                    FirstBattleDetail = table.Column<string>(nullable: true),
                    SecondBattleDetail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseDetails", x => x.ExerciseEntityTimeStamp);
                    table.ForeignKey(
                        name: "FK_ExerciseDetails_ExerciseTable_ExerciseEntityTimeStamp",
                        column: x => x.ExerciseEntityTimeStamp,
                        principalTable: "ExerciseTable",
                        principalColumn: "TimeStamp",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseDetails");

            migrationBuilder.DropTable(
                name: "ExerciseTable");

            migrationBuilder.DropColumn(
                name: "HasBattleDetail",
                table: "BattleTable");

            migrationBuilder.DropColumn(
                name: "HasLandBaseDefense",
                table: "BattleTable");
        }
    }
}
