using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZucoHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class payrollrentrelief : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RentRelief",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_OrganizationId",
                table: "JobPosts",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_Organizations_OrganizationId",
                table: "JobPosts",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_Organizations_OrganizationId",
                table: "JobPosts");

            migrationBuilder.DropIndex(
                name: "IX_JobPosts_OrganizationId",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "RentRelief",
                table: "Payslips");
        }
    }
}
