using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceRepairComputer.Migrations
{
    /// <inheritdoc />
    public partial class addupdateIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "Issues");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndJobAt",
                table: "Issues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiveAt",
                table: "Issues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SendJobAt",
                table: "Issues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartJobAt",
                table: "Issues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndJobAt",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ReceiveAt",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "SendJobAt",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "StartJobAt",
                table: "Issues");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "Issues",
                type: "datetime2",
                nullable: true);
        }
    }
}
