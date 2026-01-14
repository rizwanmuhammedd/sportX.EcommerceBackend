using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sportex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WishlistUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_UserId_ProductId",
                table: "WishlistItems",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WishlistItems_UserId_ProductId",
                table: "WishlistItems");
        }
    }
}
