using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Mig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<DateTime>(
            //    name: "LoanRepaymentDate",
            //    table: "LoanAccount",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<string>(
            //    name: "VehicleReferenceNumber",
            //    table: "LoanAccount",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "VehicleRegistrationNumber",
            //    table: "LoanAccount",
            //    type: "nvarchar(max)",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bedrooms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bathrooms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kitchens = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Property");

            //migrationBuilder.DropColumn(
            //    name: "LoanRepaymentDate",
            //    table: "LoanAccount");

            //migrationBuilder.DropColumn(
            //    name: "VehicleReferenceNumber",
            //    table: "LoanAccount");

            //migrationBuilder.DropColumn(
            //    name: "VehicleRegistrationNumber",
            //    table: "LoanAccount");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Accounts");
        }
    }
}
