using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceRepairComputer.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabaseDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "P_ID",
                table: "Divisions");

            migrationBuilder.AddColumn<string>(
                name: "DV_ID",
                table: "Positions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DV_ID",
                table: "Positions");

            migrationBuilder.AddColumn<string>(
                name: "P_ID",
                table: "Divisions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
