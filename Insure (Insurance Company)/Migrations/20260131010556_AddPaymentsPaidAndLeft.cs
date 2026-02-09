using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insure__Insurance_Company_.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsPaidAndLeft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentsLeft",
                table: "UserPolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentsPaid",
                table: "UserPolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentsLeft",
                table: "UserPolicies");

            migrationBuilder.DropColumn(
                name: "PaymentsPaid",
                table: "UserPolicies");
        }
    }
}
