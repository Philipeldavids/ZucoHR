using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZucoHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CareersRecruitment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Applicants");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "JobPosts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Applicants",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SalaryMax",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalaryMin",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverLetter",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobPostTitle",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedinUrl",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioUrl",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stage",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Applicants",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "SalaryMax",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "SalaryMin",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "CoverLetter",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "JobPostTitle",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "LinkedinUrl",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PortfolioUrl",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Applicants");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "JobPosts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Applicants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
