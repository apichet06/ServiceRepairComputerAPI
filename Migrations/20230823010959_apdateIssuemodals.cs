using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceRepairComputer.Migrations
{
    /// <inheritdoc />
    public partial class apdateIssuemodals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Issues");

            migrationBuilder.AddColumn<string>(
                name: "C_ID",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "C_ID",
                table: "Issues");

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "Issues",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
