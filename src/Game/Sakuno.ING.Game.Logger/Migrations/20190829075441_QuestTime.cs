using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class QuestTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CheckedTime",
                table: "QuestProcessTable",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedTime",
                table: "QuestProcessTable");
        }
    }
}
