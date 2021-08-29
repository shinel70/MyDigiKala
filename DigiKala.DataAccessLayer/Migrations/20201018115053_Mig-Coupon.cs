using Microsoft.EntityFrameworkCore.Migrations;

namespace DigiKala.DataAccessLayer.Migrations
{
    public partial class MigCoupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Code = table.Column<string>(maxLength: 40, nullable: true),
                    Desc = table.Column<string>(nullable: true),
                    Percent = table.Column<int>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    StartDate = table.Column<string>(maxLength: 10, nullable: true),
                    EndDate = table.Column<string>(maxLength: 10, nullable: true),
                    IsExpire = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_StoreId",
                table: "Coupons",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
