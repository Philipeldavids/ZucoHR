using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZucoHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removedomainsubsc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "Organizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPlan",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
