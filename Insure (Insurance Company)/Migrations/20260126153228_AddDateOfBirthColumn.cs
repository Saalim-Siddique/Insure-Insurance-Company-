using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insure__Insurance_Company_.Migrations
{
    /// <inheritdoc />
    public partial class AddDateOfBirthColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1900, 1, 1));  // default date to avoid null issues
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");
        }

    }
}
