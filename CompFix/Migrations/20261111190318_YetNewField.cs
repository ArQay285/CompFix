using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompFix.Migrations
{
    /// <inheritdoc />
    public partial class YetNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cart_Condition",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Cart_DiscountedPrice",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cart_Condition",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Cart_DiscountedPrice",
                table: "CartItems");
        }
    }
}
