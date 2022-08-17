using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class _vehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    EmployerName = table.Column<string>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: false),
                    AcceptTerms = table.Column<bool>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    VerificationToken = table.Column<string>(nullable: true),
                    Verified = table.Column<DateTime>(nullable: true),
                    ResetToken = table.Column<string>(nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(nullable: true),
                    PasswordReset = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankTransferRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    ProfileId = table.Column<string>(nullable: true),
                    LoanRequestId = table.Column<string>(nullable: true),
                    SourceAccount = table.Column<string>(nullable: true),
                    DestinationAccount = table.Column<string>(nullable: true),
                    TransferType = table.Column<string>(nullable: true),
                    DestinationBankCode = table.Column<string>(nullable: true),
                    DestinationName = table.Column<string>(nullable: true),
                    Narration = table.Column<string>(nullable: true),
                    PaymentReason = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    BankId = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransferRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    ProfileId = table.Column<string>(nullable: true),
                    DisbursmentStatus = table.Column<int>(nullable: false),
                    LoanAprroved = table.Column<bool>(nullable: false),
                    DisbursedAmount = table.Column<decimal>(nullable: false),
                    RepaidAmount = table.Column<decimal>(nullable: false),
                    RequestedAmount = table.Column<decimal>(nullable: false),
                    LoanBalance = table.Column<decimal>(nullable: false),
                    RepaymentStatus = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    DestinationAccount = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    LoanReason = table.Column<string>(nullable: true),
                    DestinationBankCode = table.Column<string>(nullable: true),
                    RepaymentPeriod = table.Column<int>(nullable: false),
                    DestinationName = table.Column<string>(nullable: true),
                    Narration = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    ProfileId = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    VehicleCategory = table.Column<string>(nullable: true),
                    VehicleType = table.Column<string>(nullable: true),
                    InsuranceCoverType = table.Column<string>(nullable: true),
                    VehicleModel = table.Column<string>(nullable: true),
                    YearOfManufacture = table.Column<string>(nullable: true),
                    VehicleValue = table.Column<decimal>(nullable: false),
                    RegistrationNumber = table.Column<string>(nullable: true),
                    InsturanceStartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedByIp = table.Column<string>(nullable: true),
                    Revoked = table.Column<DateTime>(nullable: true),
                    RevokedByIp = table.Column<string>(nullable: true),
                    ReplacedByToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PerformedBy = table.Column<string>(nullable: true),
                    BankTransferRequestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_BankTransferRequest_BankTransferRequestId",
                        column: x => x.BankTransferRequestId,
                        principalTable: "BankTransferRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PerformedBy = table.Column<string>(nullable: true),
                    LoanRequest = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanHistory_LoanAccount_LoanRequest",
                        column: x => x.LoanRequest,
                        principalTable: "LoanAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_BankTransferRequestId",
                table: "History",
                column: "BankTransferRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanHistory_LoanRequest",
                table: "LoanHistory",
                column: "LoanRequest");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_AccountId",
                table: "RefreshToken",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "LoanHistory");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "BankTransferRequest");

            migrationBuilder.DropTable(
                name: "LoanAccount");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
