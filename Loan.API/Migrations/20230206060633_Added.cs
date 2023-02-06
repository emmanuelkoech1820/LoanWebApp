using Microsoft.EntityFrameworkCore.Migrations;

namespace Loan.API.Migrations
{
    public partial class Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BorrowedAmount",
                table: "LoanHistory",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RepaidAmount",
                table: "LoanHistory",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowedAmount",
                table: "LoanHistory");

            migrationBuilder.DropColumn(
                name: "RepaidAmount",
                table: "LoanHistory");
        }
    }
}
