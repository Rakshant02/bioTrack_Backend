using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class ConsentForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUri",
                table: "ConsentForms");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ConsentForms");

            migrationBuilder.DropColumn(
                name: "SignedDate",
                table: "ConsentForms");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ConsentForms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUri",
                table: "ConsentForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ConsentForms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedDate",
                table: "ConsentForms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ConsentForms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
