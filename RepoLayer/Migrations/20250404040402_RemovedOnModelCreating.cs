using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepoLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemovedOnModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AdminUsers_AdminUserId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "WishlistID",
                table: "Wishlists",
                newName: "WishlistId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CartID",
                table: "Carts",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "BookID",
                table: "Books",
                newName: "BookId");

            migrationBuilder.RenameColumn(
                name: "AdminUserID",
                table: "AdminUsers",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AdminUsers_AdminUserId",
                table: "Books",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AdminUsers_AdminUserId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "WishlistId",
                table: "Wishlists",
                newName: "WishlistID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "Carts",
                newName: "CartID");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Books",
                newName: "BookID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminUsers",
                newName: "AdminUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AdminUsers_AdminUserId",
                table: "Books",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "AdminUserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
