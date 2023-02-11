using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicShop.Migrations
{
    public partial class BosungthemthuoctinhchoProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Products_Types_TypeId",
            //     table: "Products");

            // migrationBuilder.DropTable(
            //     name: "Types");

            // migrationBuilder.DropIndex(
            //     name: "IX_Products_TypeId",
            //     table: "Products");

            // migrationBuilder.RenameColumn(
            //     name: "TypeId",
            //     table: "Products",
            //     newName: "CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Products",
                type: "TEXT",
                nullable: true);

            // migrationBuilder.CreateTable(
            //     name: "Categories",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "INTEGER", nullable: false)
            //             .Annotation("Sqlite:Autoincrement", true),
            //         Name = table.Column<string>(type: "TEXT", nullable: false),
            //         ProductId = table.Column<int>(type: "INTEGER", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Categories", x => x.Id);
            //     });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

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
                name: "Quantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Products");

            // migrationBuilder.RenameColumn(
            //     name: "CategoryId",
            //     table: "Products",
            //     newName: "TypeId");

            // migrationBuilder.CreateTable(
            //     name: "Types",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "INTEGER", nullable: false)
            //             .Annotation("Sqlite:Autoincrement", true),
            //         Name = table.Column<string>(type: "TEXT", nullable: false),
            //         ProductId = table.Column<int>(type: "INTEGER", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Types", x => x.Id);
            //     });

            // migrationBuilder.CreateIndex(
            //     name: "IX_Products_TypeId",
            //     table: "Products",
            //     column: "TypeId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Products_Types_TypeId",
            //     table: "Products",
            //     column: "TypeId",
            //     principalTable: "Types",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }
    }
}
