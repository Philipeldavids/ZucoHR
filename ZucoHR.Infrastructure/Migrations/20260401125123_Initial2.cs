using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZucoHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BaseSalary",
                table: "Employees",
                newName: "BasicSalary");

            migrationBuilder.AddColumn<decimal>(
                name: "Allowances",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BasicSalary",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Payslips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "GrossPay",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NHF",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherDeductions",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Pension",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeductions",
                table: "Payslips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PayRuns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeductions",
                table: "PayRuns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalGross",
                table: "PayRuns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Allowances",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allowances",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "BasicSalary",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "GrossPay",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "NHF",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "OtherDeductions",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "Pension",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "TotalDeductions",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PayRuns");

            migrationBuilder.DropColumn(
                name: "TotalDeductions",
                table: "PayRuns");

            migrationBuilder.DropColumn(
                name: "TotalGross",
                table: "PayRuns");

            migrationBuilder.DropColumn(
                name: "Allowances",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "BasicSalary",
                table: "Employees",
                newName: "BaseSalary");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
