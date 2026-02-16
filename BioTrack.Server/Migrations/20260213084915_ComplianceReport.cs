using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class ComplianceReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProtocolDeviations_Participants_ParticipantsParticipantID",
                table: "ProtocolDeviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProtocolDeviations_TrialsProtocols_ProtocolID",
                table: "ProtocolDeviations");

            migrationBuilder.DropIndex(
                name: "IX_ProtocolDeviations_ParticipantsParticipantID",
                table: "ProtocolDeviations");

            migrationBuilder.DropColumn(
                name: "ParticipantsParticipantID",
                table: "ProtocolDeviations");

            migrationBuilder.AddForeignKey(
                name: "FK_ProtocolDeviations_TrialsProtocols_ProtocolID",
                table: "ProtocolDeviations",
                column: "ProtocolID",
                principalTable: "TrialsProtocols",
                principalColumn: "ProtocolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProtocolDeviations_TrialsProtocols_ProtocolID",
                table: "ProtocolDeviations");

            migrationBuilder.AddColumn<int>(
                name: "ParticipantsParticipantID",
                table: "ProtocolDeviations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_ParticipantsParticipantID",
                table: "ProtocolDeviations",
                column: "ParticipantsParticipantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProtocolDeviations_Participants_ParticipantsParticipantID",
                table: "ProtocolDeviations",
                column: "ParticipantsParticipantID",
                principalTable: "Participants",
                principalColumn: "ParticipantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProtocolDeviations_TrialsProtocols_ProtocolID",
                table: "ProtocolDeviations",
                column: "ProtocolID",
                principalTable: "TrialsProtocols",
                principalColumn: "ProtocolID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
