using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddProtocolDeviation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResearcherCredentials",
                columns: table => new
                {
                    ResearcherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearcherCredentials", x => x.ResearcherId);
                });

            migrationBuilder.CreateTable(
                name: "TrialsProtocols",
                columns: table => new
                {
                    ProtocolID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnrollmentRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrialsProtocols", x => x.ProtocolID);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceReports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtocolID = table.Column<int>(type: "int", nullable: false),
                    DeviationCount = table.Column<int>(type: "int", nullable: false),
                    AdherenceRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceReports", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_ComplianceReports_TrialsProtocols_ProtocolID",
                        column: x => x.ProtocolID,
                        principalTable: "TrialsProtocols",
                        principalColumn: "ProtocolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySites",
                columns: table => new
                {
                    SiteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtocolID = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvestigatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrincipalInvestigatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySites", x => x.SiteID);
                    table.ForeignKey(
                        name: "FK_StudySites_ResearcherCredentials_PrincipalInvestigatorId",
                        column: x => x.PrincipalInvestigatorId,
                        principalTable: "ResearcherCredentials",
                        principalColumn: "ResearcherId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudySites_TrialsProtocols_ProtocolID",
                        column: x => x.ProtocolID,
                        principalTable: "TrialsProtocols",
                        principalColumn: "ProtocolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrialsReports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtocolID = table.Column<int>(type: "int", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrialsReports", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_TrialsReports_TrialsProtocols_ProtocolID",
                        column: x => x.ProtocolID,
                        principalTable: "TrialsProtocols",
                        principalColumn: "ProtocolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    ParticipantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtocolID = table.Column<int>(type: "int", nullable: false),
                    SiteID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Age = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BloodPressure = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    HeartRate = table.Column<int>(type: "int", nullable: false),
                    EligibilityStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.ParticipantID);
                    table.ForeignKey(
                        name: "FK_Participants_StudySites_SiteID",
                        column: x => x.SiteID,
                        principalTable: "StudySites",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Participants_TrialsProtocols_ProtocolID",
                        column: x => x.ProtocolID,
                        principalTable: "TrialsProtocols",
                        principalColumn: "ProtocolID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudySiteResearchers",
                columns: table => new
                {
                    SiteID = table.Column<int>(type: "int", nullable: false),
                    ResearcherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySiteResearchers", x => new { x.SiteID, x.ResearcherId });
                    table.ForeignKey(
                        name: "FK_StudySiteResearchers_ResearcherCredentials_ResearcherId",
                        column: x => x.ResearcherId,
                        principalTable: "ResearcherCredentials",
                        principalColumn: "ResearcherId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySiteResearchers_StudySites_SiteID",
                        column: x => x.SiteID,
                        principalTable: "StudySites",
                        principalColumn: "SiteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdverseEvents",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantID = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    ReportedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdverseEvents", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_AdverseEvents_Participants_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Participants",
                        principalColumn: "ParticipantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsentForms",
                columns: table => new
                {
                    ConsentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantID = table.Column<int>(type: "int", nullable: false),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    FileUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentForms", x => x.ConsentID);
                    table.ForeignKey(
                        name: "FK_ConsentForms_Participants_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Participants",
                        principalColumn: "ParticipantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    ObservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantID = table.Column<int>(type: "int", nullable: false),
                    ProtocolID = table.Column<int>(type: "int", nullable: true),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BodyTemperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HeartRate = table.Column<int>(type: "int", nullable: false),
                    BloodPressure = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    OxygenSaturation = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Hemoglobin = table.Column<int>(type: "int", nullable: false),
                    Creatinine = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.ObservationID);
                    table.ForeignKey(
                        name: "FK_Observations_Participants_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Participants",
                        principalColumn: "ParticipantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observations_TrialsProtocols_ProtocolID",
                        column: x => x.ProtocolID,
                        principalTable: "TrialsProtocols",
                        principalColumn: "ProtocolID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProtocolDeviations",
                columns: table => new
                {
                    DeviationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProtocolID = table.Column<int>(type: "int", nullable: false),
                    ParticipantID = table.Column<int>(type: "int", nullable: false),
                    ObservationID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    ReportedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParticipantsParticipantID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolDeviations", x => x.DeviationId);
                    table.ForeignKey(
                        name: "FK_ProtocolDeviations_Observations_ObservationID",
                        column: x => x.ObservationID,
                        principalTable: "Observations",
                        principalColumn: "ObservationID");
                    table.ForeignKey(
                        name: "FK_ProtocolDeviations_Participants_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Participants",
                        principalColumn: "ParticipantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProtocolDeviations_Participants_ParticipantsParticipantID",
                        column: x => x.ParticipantsParticipantID,
                        principalTable: "Participants",
                        principalColumn: "ParticipantID");
                    table.ForeignKey(
                        name: "FK_ProtocolDeviations_TrialsProtocols_ProtocolID",
                        column: x => x.ProtocolID,
                        principalTable: "TrialsProtocols",
                        principalColumn: "ProtocolID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdverseEvents_ParticipantID",
                table: "AdverseEvents",
                column: "ParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceReports_ProtocolID",
                table: "ComplianceReports",
                column: "ProtocolID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentForms_ParticipantID",
                table: "ConsentForms",
                column: "ParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ParticipantID",
                table: "Observations",
                column: "ParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ProtocolID",
                table: "Observations",
                column: "ProtocolID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ProtocolID",
                table: "Participants",
                column: "ProtocolID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_SiteID",
                table: "Participants",
                column: "SiteID");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_ObservationID",
                table: "ProtocolDeviations",
                column: "ObservationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_ParticipantID",
                table: "ProtocolDeviations",
                column: "ParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_ParticipantsParticipantID",
                table: "ProtocolDeviations",
                column: "ParticipantsParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_ProtocolID",
                table: "ProtocolDeviations",
                column: "ProtocolID");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_ReportedDate",
                table: "ProtocolDeviations",
                column: "ReportedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProtocolDeviations_Severity",
                table: "ProtocolDeviations",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_StudySiteResearchers_ResearcherId",
                table: "StudySiteResearchers",
                column: "ResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySites_PrincipalInvestigatorId",
                table: "StudySites",
                column: "PrincipalInvestigatorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySites_ProtocolID",
                table: "StudySites",
                column: "ProtocolID");

            migrationBuilder.CreateIndex(
                name: "IX_TrialsReports_ProtocolID",
                table: "TrialsReports",
                column: "ProtocolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdverseEvents");

            migrationBuilder.DropTable(
                name: "ComplianceReports");

            migrationBuilder.DropTable(
                name: "ConsentForms");

            migrationBuilder.DropTable(
                name: "ProtocolDeviations");

            migrationBuilder.DropTable(
                name: "StudySiteResearchers");

            migrationBuilder.DropTable(
                name: "TrialsReports");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "StudySites");

            migrationBuilder.DropTable(
                name: "ResearcherCredentials");

            migrationBuilder.DropTable(
                name: "TrialsProtocols");
        }
    }
}
