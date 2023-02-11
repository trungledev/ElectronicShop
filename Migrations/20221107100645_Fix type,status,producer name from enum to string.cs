using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicShop.Migrations
{
    public partial class Fixtypestatusproducernamefromenumtostring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Types",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Statuses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Producers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Types");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Producers");
        }
    }
}
