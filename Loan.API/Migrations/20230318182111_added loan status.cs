using Microsoft.EntityFrameworkCore.Migrations;

namespace Loan.API.Migrations
{
    public partial class addedloanstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanAprroved",
                table: "LoanAccount");

            migrationBuilder.AddColumn<int>(
                name: "LoanStatus",
                table: "LoanAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanStatus",
                table: "LoanAccount");

            migrationBuilder.AddColumn<bool>(
                name: "LoanAprroved",
                table: "LoanAccount",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
