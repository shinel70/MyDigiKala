using Microsoft.EntityFrameworkCore.Migrations;

namespace DigiKala.DataAccessLayer.Migrations
{
    public partial class MigStoreBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCard",
                table: "Stores",
                maxLength: 24,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCard",
                table: "Stores");
        }
    }
}
