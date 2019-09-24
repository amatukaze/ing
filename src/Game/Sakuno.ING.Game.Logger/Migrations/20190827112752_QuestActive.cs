using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Game.Logger.Migrations
{
    public partial class QuestActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
            => migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "QuestProcessTable",
                nullable: false,
                defaultValue: false);

        protected override void Down(MigrationBuilder migrationBuilder)
            => migrationBuilder.DropColumn(
                name: "IsActive",
                table: "QuestProcessTable");
    }
}
