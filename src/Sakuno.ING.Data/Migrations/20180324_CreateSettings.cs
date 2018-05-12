using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sakuno.ING.Data.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20180324_CreateSettings")]
    public class CreateSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SettingEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingEntries", x => x.Id);
                });
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity("Sakuno.ING.Data.SettingDbEntry", b =>
            {
                b.Property<string>("Id");
                b.Property<string>("Value");
                b.HasKey("Id");
                b.ToTable("SettingEntries");
            });
        }
    }
}
