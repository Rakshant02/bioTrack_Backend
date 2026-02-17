using BioTrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Data
{
    public class BioDataContext : DbContext
    {
        public BioDataContext(DbContextOptions<BioDataContext> options) : base(options) { }

        public DbSet<Participants> Participants { get; set; } = default!;
        public DbSet<TrialProtocols> TrialsProtocols { get; set; } = default!; // maps to table "TrialsProtocols"
        public DbSet<TrialReports> TrialsReports { get; set; } = default!;
        public DbSet<Observations> Observations { get; set; } = default!;
        public DbSet<StudySites> StudySites { get; set; } = default!;
        public DbSet<ComplianceReports> ComplianceReports { get; set; } = default!;
        public DbSet<ConsentForm> ConsentForms { get; set; } = default!;
        public DbSet<AdverseEvents> AdverseEvents { get; set; } = default!;
        public DbSet<ProtocolDeviation> ProtocolDeviations { get; set; } = default!;
        public DbSet<ResearcherCredentials> ResearcherCredentials { get; set; } = default!;
        public DbSet<AuditLogs> AuditLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TrialProtocols>().ToTable("TrialsProtocols");
            modelBuilder.Entity<Participants>().ToTable("Participants");
            modelBuilder.Entity<StudySites>().ToTable("StudySites");
            modelBuilder.Entity<ProtocolDeviation>().ToTable("ProtocolDeviations");

            // TrialProtocol -> StudySites (1-n)
            modelBuilder.Entity<StudySites>()
                .HasOne(s => s.TrialProtocol)
                .WithMany(tp => tp.StudySites)
                .HasForeignKey(s => s.ProtocolID)
                .OnDelete(DeleteBehavior.SetNull);

            // TrialProtocol -> Participants (1-n)
            modelBuilder.Entity<Participants>()
                .HasOne(p => p.TrialProtocol)
                .WithMany(tp => tp.Participants)
                .HasForeignKey(p => p.ProtocolID)
                .OnDelete(DeleteBehavior.Restrict);

            // StudySite -> Participants (1-n, optional)
            modelBuilder.Entity<Participants>()
                .HasOne(p => p.StudySite)
                .WithMany(s => s.Participants)
                .HasForeignKey(p => p.SiteID)
                .OnDelete(DeleteBehavior.SetNull);

            // Participant -> ConsentForm (1-n)
            modelBuilder.Entity<ConsentForm>()
                .HasOne(c => c.Participant)
                .WithMany(p => p.Consents)
                .HasForeignKey(c => c.ParticipantID)
                .OnDelete(DeleteBehavior.Cascade);

            // Participant -> Observations (1-n)  (CASCADE)
            modelBuilder.Entity<Observations>()
                .HasOne(o => o.Participant)
                .WithMany(p => p.Observations)
                .HasForeignKey(o => o.ParticipantID)
                .OnDelete(DeleteBehavior.Cascade);

            // Participant -> AdverseEvents (1-n)
            modelBuilder.Entity<AdverseEvents>()
                .HasOne(a => a.Participant)
                .WithMany(p => p.AdverseEvents)
                .HasForeignKey(a => a.ParticipantID)
                .OnDelete(DeleteBehavior.Cascade);

            // TrialProtocol -> ComplianceReports (1-n) (CASCADE)
            modelBuilder.Entity<ComplianceReports>()
                .HasOne(cr => cr.TrialProtocol)
                .WithMany(tp => tp.ComplianceReports)
                .HasForeignKey(cr => cr.ProtocolID)
                .OnDelete(DeleteBehavior.Cascade);

            // TrialProtocol -> TrialReports (1-n)
            modelBuilder.Entity<TrialReports>()
                .HasOne(tr => tr.TrialProtocol)
                .WithMany(tp => tp.TrialReports)
                .HasForeignKey(tr => tr.ProtocolID)
                .OnDelete(DeleteBehavior.Cascade);

            // Observations -> TrialProtocols (nullable) (SET NULL)
            modelBuilder.Entity<Observations>(entity =>
            {
                entity.HasOne(o => o.Protocol)
                      .WithMany()
                      .HasForeignKey(o => o.ProtocolID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // StudySites -> PrincipalInvestigator (Restrict)
            //modelBuilder.Entity<StudySites>()
            //    .HasOne(s => s.PrincipalInvestigator)
            //    .WithMany(r => r.PrincipalInvestigatorSites)
            //    .HasForeignKey(s => s.PrincipalInvestigatorId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many: StudySites <-> ResearcherCredentials
            //modelBuilder.Entity<StudySites>()
            //    .HasMany(s => s.StudySiteResearchers)
            //    .WithMany(r => r.CollaboratingSites)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "StudySiteResearchers",
            //        right => right
            //            .HasOne<ResearcherCredentials>()
            //            .WithMany()
            //            .HasForeignKey("ResearcherId")
            //            .HasConstraintName("FK_StudySiteResearchers_ResearcherCredentials_ResearcherId")
            //            .OnDelete(DeleteBehavior.Cascade),
            //        left => left
            //            .HasOne<StudySites>()
            //            .WithMany()
            //            .HasForeignKey("SiteID")
            //            .HasConstraintName("FK_StudySiteResearchers_StudySites_SiteID")
            //            .OnDelete(DeleteBehavior.Cascade),
            //        join =>
            //        {
            //            join.ToTable("StudySiteResearchers");
            //            join.HasKey("SiteID", "ResearcherId");
            //            join.HasIndex("ResearcherId");
            //        });

            // ProtocolDeviation enum conversion
            modelBuilder.Entity<ProtocolDeviation>()
                .Property(d => d.Severity)
                .HasConversion<int>();

            // ProtocolDeviation -> TrialProtocol (NO ACTION)
            modelBuilder.Entity<ProtocolDeviation>()
                .HasOne(d => d.TrialProtocol)
                .WithMany(tp => tp.ProtocolDeviations)
                .HasForeignKey(d => d.ProtocolID)
                .OnDelete(DeleteBehavior.NoAction);

            // ProtocolDeviation -> Participant (CASCADE)  (Keep only this once)
            modelBuilder.Entity<ProtocolDeviation>()
                .HasOne(d => d.Participant)
                .WithMany(p => p.ProtocolDeviations)
                .HasForeignKey(d => d.ParticipantID)
                .OnDelete(DeleteBehavior.Cascade);

            // ProtocolDeviation -> Observation (NO ACTION to avoid multiple cascade paths)
            modelBuilder.Entity<ProtocolDeviation>()
                .HasOne(d => d.Observation)
                .WithMany()
                .HasForeignKey(d => d.ObservationID)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            modelBuilder.Entity<Observations>().HasIndex(o => o.ProtocolID);
            modelBuilder.Entity<Participants>().HasIndex(p => p.ProtocolID);
            modelBuilder.Entity<Participants>().HasIndex(p => p.SiteID);
            modelBuilder.Entity<ConsentForm>().HasIndex(c => c.ParticipantID);
            modelBuilder.Entity<Observations>().HasIndex(o => o.ParticipantID);
            modelBuilder.Entity<AdverseEvents>().HasIndex(a => a.ParticipantID);
            modelBuilder.Entity<ComplianceReports>().HasIndex(r => r.ProtocolID);
            modelBuilder.Entity<TrialReports>().HasIndex(r => r.ProtocolID);

            modelBuilder.Entity<ProtocolDeviation>().HasIndex(d => d.ProtocolID);
            modelBuilder.Entity<ProtocolDeviation>().HasIndex(d => d.ParticipantID);
            modelBuilder.Entity<ProtocolDeviation>().HasIndex(d => d.ObservationID);
            modelBuilder.Entity<ProtocolDeviation>().HasIndex(d => d.ReportedDate);
            modelBuilder.Entity<ProtocolDeviation>().HasIndex(d => d.Severity);

            // Decimal precisions to remove warnings
            modelBuilder.Entity<ComplianceReports>()
                .Property(c => c.AdherenceRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TrialProtocols>()
                .Property(tp => tp.EnrollmentRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TrialProtocols>()
                .Property(tp => tp.CompletionRate)
                .HasPrecision(18, 2);

            // Participant -> ConsentForm (1-n)
            modelBuilder.Entity<ConsentForm>()
                .HasOne(c => c.Participant)
                .WithMany(p => p.Consents)
                .HasForeignKey(c => c.ParticipantID)
                .OnDelete(DeleteBehavior.Cascade);

            // Index to speed up history queries by participant
            modelBuilder.Entity<ConsentForm>()
                .HasIndex(c => c.ParticipantID);
            // TrialProtocol -> Lead Researcher (optional), do not cascade delete protocols
            modelBuilder.Entity<TrialProtocols>()
                .HasOne(tp => tp.LeadResearcher)
                .WithMany(r => r.LeadProtocols)
                .HasForeignKey(tp => tp.LeadResearcherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure existing 1:n TrialProtocol -> StudySites stays
            //modelBuilder.Entity<StudySites>()
            //    .HasOne(s => s.TrialProtocol)
            //    .WithMany(tp => tp.StudySites)
            //    .HasForeignKey(s => s.ProtocolID)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Helpful index
            modelBuilder.Entity<TrialProtocols>()
                .HasIndex(tp => tp.LeadResearcherId);

            modelBuilder.Entity<AuditLogs>(b =>
            {
                b.ToTable("AuditLogs");
                b.HasKey(x => x.LogId);
                b.Property(x => x.Action).IsRequired().HasMaxLength(50);
                b.Property(x => x.User).HasMaxLength(256);
                b.Property(x => x.Timestamp);
            });

            modelBuilder.Entity<ResearcherCredentials>(b =>
            {
                b.ToTable("ResearcherCredentials");
                b.HasKey(r => r.ResearcherId);
                b.Property(r => r.FullName).IsRequired().HasMaxLength(150);
                b.Property(r => r.Email).IsRequired().HasMaxLength(150);
                b.Property(r => r.PasswordHash).IsRequired();
                b.HasIndex(r => r.Email).IsUnique(); // recommended for login
            });



        }
    }
}