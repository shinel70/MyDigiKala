using Microsoft.EntityFrameworkCore.Migrations;

namespace DigiKala.DataAccessLayer.Migrations
{
    public partial class MigSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    DefaultImg = table.Column<string>(maxLength: 100, nullable: true),
                    Size = table.Column<string>(maxLength: 50, nullable: true),
                    Desc = table.Column<string>(nullable: true),
                    Pirce = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Img = table.Column<string>(maxLength: 100, nullable: true),
                    NotShow = table.Column<bool>(nullable: false),
                    OrderShow = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "Sliders");
        }
    }
}
