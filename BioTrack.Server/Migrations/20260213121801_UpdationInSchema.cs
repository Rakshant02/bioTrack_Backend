using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdationInSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeadResearcherId",
                table: "TrialsProtocols",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Objectives",
                table: "TrialsProtocols",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrialsProtocols_LeadResearcherId",
                table: "TrialsProtocols",
                column: "LeadResearcherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrialsProtocols_ResearcherCredentials_LeadResearcherId",
                table: "TrialsProtocols",
                column: "LeadResearcherId",
                principalTable: "ResearcherCredentials",
                principalColumn: "ResearcherId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrialsProtocols_ResearcherCredentials_LeadResearcherId",
                table: "TrialsProtocols");

            migrationBuilder.DropIndex(
                name: "IX_TrialsProtocols_LeadResearcherId",
                table: "TrialsProtocols");

            migrationBuilder.DropColumn(
                name: "LeadResearcherId",
                table: "TrialsProtocols");

            migrationBuilder.DropColumn(
                name: "Objectives",
                table: "TrialsProtocols");
        }
    }
}
