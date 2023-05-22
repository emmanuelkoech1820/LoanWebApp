using Microsoft.EntityFrameworkCore.Migrations;

namespace Loan.API.Migrations
{
    public partial class addedLatitudeandLongitude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Property");
        }
    }
}
