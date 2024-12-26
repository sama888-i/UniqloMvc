using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uniqlo2.Migrations
{
    /// <inheritdoc />
    public partial class BasketProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBasket_Products_ProductId",
                table: "UsersBasket");

            migrationBuilder.DropIndex(
                name: "IX_UsersBasket_ProductId",
                table: "UsersBasket");

            migrationBuilder.DropIndex(
                name: "IX_UsersBasket_UserId",
                table: "UsersBasket");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "UsersBasket");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "UsersBasket");

            migrationBuilder.CreateTable(
                name: "BasketProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasketId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BasketProducts_UsersBasket_BasketId",
                        column: x => x.BasketId,
                        principalTable: "UsersBasket",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersBasket_UserId",
                table: "UsersBasket",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_ProductId",
                table: "BasketProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketProducts");

            migrationBuilder.DropIndex(
                name: "IX_UsersBasket_UserId",
                table: "UsersBasket");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "UsersBasket",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "UsersBasket",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersBasket_ProductId",
                table: "UsersBasket",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBasket_UserId",
                table: "UsersBasket",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBasket_Products_ProductId",
                table: "UsersBasket",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
