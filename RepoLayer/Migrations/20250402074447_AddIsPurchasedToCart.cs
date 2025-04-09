using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepoLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPurchasedToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPurchased",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPurchased",
                table: "Carts");
        }
    }
}
