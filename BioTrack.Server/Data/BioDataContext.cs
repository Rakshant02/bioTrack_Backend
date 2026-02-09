using BioTrack.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BioTrack.Server.Data
{
    public class BioDataContext(DbContextOptions<BioDataContext> options):DbContext
    {
        

        public DbSet<Participants> Participants { get; set; }
        public DbSet<TrialProtocols> TrialsProtocols { get; set; }
        public DbSet<TrialReports> TrialsReports { get; set; }
        public DbSet<Observations> Observations { get; set; }
        
        public DbSet<StudySites> StudySites { get; set; }
        public DbSet<ComplianceReports> ComplianceReports { get; set; }

        
    }
}
