using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class addedreqprotocol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExclusionCriteria",
                table: "TrialsProtocols",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InclusionCriteria",
                table: "TrialsProtocols",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Objective",
                table: "TrialsProtocols",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExclusionCriteria",
                table: "TrialsProtocols");

            migrationBuilder.DropColumn(
                name: "InclusionCriteria",
                table: "TrialsProtocols");

            migrationBuilder.DropColumn(
                name: "Objective",
                table: "TrialsProtocols");
        }
    }
}
