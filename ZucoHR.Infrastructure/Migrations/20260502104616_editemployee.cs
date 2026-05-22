using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZucoHR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editemployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Allowances",
                table: "Employees",
                newName: "Allowance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Allowance",
                table: "Employees",
                newName: "Allowances");

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
