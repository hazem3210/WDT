using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suaah.Migrations
{
    public partial class addFlagPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlagPath",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagPath",
                table: "Country");
        }
    }
}
