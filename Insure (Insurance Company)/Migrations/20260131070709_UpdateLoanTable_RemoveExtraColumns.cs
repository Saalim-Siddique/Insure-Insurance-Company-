using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insure__Insurance_Company_.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoanTable_RemoveExtraColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "OutstandingBalance",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "RepaymentAmount",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "TermMonths",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Loans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DueDate",
                table: "Loans",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandingBalance",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RepaymentAmount",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TermMonths",
                table: "Loans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
