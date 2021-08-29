using Microsoft.EntityFrameworkCore.Migrations;

namespace DigiKala.DataAccessLayer.Migrations
{
    public partial class MigBanner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pirce",
                table: "Banners");

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Banners",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Banners");

            migrationBuilder.AddColumn<int>(
                name: "Pirce",
                table: "Banners",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
