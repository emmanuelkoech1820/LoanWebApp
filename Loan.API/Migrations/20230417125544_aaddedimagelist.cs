using Microsoft.EntityFrameworkCore.Migrations;

namespace Loan.API.Migrations
{
    public partial class aaddedimagelist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyModelId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_PropertyModelId",
                table: "Images",
                column: "PropertyModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Property_PropertyModelId",
                table: "Images",
                column: "PropertyModelId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Property_PropertyModelId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_PropertyModelId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PropertyModelId",
                table: "Images");
        }
    }
}
