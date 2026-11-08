using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompFix.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "id", "Name" },
                values: new object[,]
                {
                    { 1, "Комп'ютери" },
                    { 2, "Ноутбуки" },
                    { 3, "Смартфони" },
                    { 4, "Планшети" },
                    { 5, "Аксесуари" },
                    { 6, "Ігрові приставки" },
                    { 7, "Телевізори" },
                    { 8, "Процесори" },
                    { 9, "Відеокарти" },
                    { 10, "Материнські плати" },
                    { 11, "Оперативна пам'ять" },
                    { 12, "Вібратори" },
                    { 13, "Люди" },
                    { 14, "Монітори" },
                    { 15, "Побутова техніка" },
                    { 16, "Ігрові комп'ютери" },
                    { 17, "Ігрові ноутбуки" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");
        }
    }
}
