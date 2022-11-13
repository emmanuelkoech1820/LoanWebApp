﻿// <auto-generated />
using System;
using Apps.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebApi.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Apps.Data.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AcceptTerms")
                        .HasColumnType("bit");

                    b.Property<string>("ClientId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmployerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PasswordReset")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Verified")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Apps.Data.Entities.BankTransferRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("BankId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationBankCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoanRequestId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Narration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransferType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("BankTransferRequest");
                });

            modelBuilder.Entity("Apps.Data.Entities.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BankTransferRequestId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PerformedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BankTransferRequestId");

                    b.ToTable("History");
                });

            modelBuilder.Entity("Apps.Data.Entities.LoanAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationBankCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DisbursedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("DisbursmentStatus")
                        .HasColumnType("int");

                    b.Property<bool>("LoanAprroved")
                        .HasColumnType("bit");

                    b.Property<decimal>("LoanBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("LoanReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LoanRepaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Narration")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("RepaidAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("RepaymentPeriod")
                        .HasColumnType("int");

                    b.Property<string>("RepaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("RequestedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("VehicleReferenceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleRegistrationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LoanAccount");
                });

            modelBuilder.Entity("Apps.Data.Entities.LoanHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LoanRequest")
                        .HasColumnType("int");

                    b.Property<string>("PerformedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("LoanRequest");

                    b.ToTable("LoanHistory");
                });

            modelBuilder.Entity("Apps.Data.Entities.PropertyModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AgentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bathrooms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bedrooms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Kitchens")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PropertyType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("Apps.Data.Entities.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("InsturanceStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InsuranceCoverType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("VehicleCategory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleModel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("VehicleValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("YearOfManufacture")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("Apps.Data.Entities.Account", b =>
                {
                    b.OwnsMany("Apps.Data.Entities.RefreshToken", "RefreshTokens", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("AccountId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("Created")
                                .HasColumnType("datetime2");

                            b1.Property<string>("CreatedByIp")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime>("Expires")
                                .HasColumnType("datetime2");

                            b1.Property<string>("ReplacedByToken")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime?>("Revoked")
                                .HasColumnType("datetime2");

                            b1.Property<string>("RevokedByIp")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Token")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("Id");

                            b1.HasIndex("AccountId");

                            b1.ToTable("RefreshToken");

                            b1.WithOwner("Account")
                                .HasForeignKey("AccountId");

                            b1.Navigation("Account");
                        });

                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("Apps.Data.Entities.History", b =>
                {
                    b.HasOne("Apps.Data.Entities.BankTransferRequest", "BankTransferRequest")
                        .WithMany("Histories")
                        .HasForeignKey("BankTransferRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankTransferRequest");
                });

            modelBuilder.Entity("Apps.Data.Entities.LoanHistory", b =>
                {
                    b.HasOne("Apps.Data.Entities.LoanAccount", "LoanAccount")
                        .WithMany("LoanHistories")
                        .HasForeignKey("LoanRequest")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LoanAccount");
                });

            modelBuilder.Entity("Apps.Data.Entities.BankTransferRequest", b =>
                {
                    b.Navigation("Histories");
                });

            modelBuilder.Entity("Apps.Data.Entities.LoanAccount", b =>
                {
                    b.Navigation("LoanHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
