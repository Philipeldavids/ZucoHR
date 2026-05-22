using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZucoHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onboardingmodelredone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnboardingTasks_Onboardings_OnboardingId",
                table: "OnboardingTasks");

            migrationBuilder.DropTable(
                name: "Onboardings");

            migrationBuilder.DropIndex(
                name: "IX_OnboardingTasks_OnboardingId",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "OnboardingTasks");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "OnboardingTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "OnboardingTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "OnboardingTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OnboardingTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingTasks_EmployeeId",
                table: "OnboardingTasks",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnboardingTasks_Employees_EmployeeId",
                table: "OnboardingTasks",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnboardingTasks_Employees_EmployeeId",
                table: "OnboardingTasks");

            migrationBuilder.DropIndex(
                name: "IX_OnboardingTasks_EmployeeId",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OnboardingTasks");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "OnboardingTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Onboardings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Onboardings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingTasks_OnboardingId",
                table: "OnboardingTasks",
                column: "OnboardingId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnboardingTasks_Onboardings_OnboardingId",
                table: "OnboardingTasks",
                column: "OnboardingId",
                principalTable: "Onboardings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
