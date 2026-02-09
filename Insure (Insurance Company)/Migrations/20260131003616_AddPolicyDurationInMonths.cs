using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insure__Insurance_Company_.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyDurationInMonths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInMonths",
                table: "Policies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInMonths",
                table: "Policies");
        }
    }
}
