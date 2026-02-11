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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure EF maps entities to the exact existing table names
            modelBuilder.Entity<TrialProtocols>().ToTable("TrialsProtocols");
            modelBuilder.Entity<Participants>().ToTable("Participants");
            modelBuilder.Entity<StudySites>().ToTable("StudySites");

            // TrialProtocol -> StudySites (1-n)
            modelBuilder.Entity<StudySites>()
                .HasOne(s => s.TrialProtocol)
                .WithMany(tp => tp.StudySites)
                .HasForeignKey(s => s.ProtocolID)
                .OnDelete(DeleteBehavior.Cascade);

            // TrialProtocol -> Participants (1-n)
            modelBuilder.Entity<Participants>()
                .HasOne(p => p.TrialProtocol)
                .WithMany(tp => tp.Participants)
                .HasForeignKey(p => p.ProtocolID)
                .OnDelete(DeleteBehavior.Restrict); // safer for clinical data; prevents accidental cascade deletes

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

            // Participant -> Observations (1-n)
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

            // TrialProtocol -> ComplianceReports (1-n)
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

            // Indexes
            modelBuilder.Entity<Participants>().HasIndex(p => p.ProtocolID);
            modelBuilder.Entity<Participants>().HasIndex(p => p.SiteID);
            modelBuilder.Entity<ConsentForm>().HasIndex(c => c.ParticipantID);
            modelBuilder.Entity<Observations>().HasIndex(o => o.ParticipantID);
            modelBuilder.Entity<AdverseEvents>().HasIndex(a => a.ParticipantID);
            modelBuilder.Entity<ComplianceReports>().HasIndex(r => r.ProtocolID);
            modelBuilder.Entity<TrialReports>().HasIndex(r => r.ProtocolID);
        }
    }
}