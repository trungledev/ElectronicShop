using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicShop.Migrations
{
    public partial class addadressandphonenumberinproducerProducer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Producers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProducerId",
                table: "Addresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProducerProductId",
                table: "Addresses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProducerProductId",
                table: "Addresses",
                column: "ProducerProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Producers_ProducerProductId",
                table: "Addresses",
                column: "ProducerProductId",
                principalTable: "Producers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Producers_ProducerProductId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ProducerProductId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Producers");

            migrationBuilder.DropColumn(
                name: "ProducerId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ProducerProductId",
                table: "Addresses");
        }
    }
}
