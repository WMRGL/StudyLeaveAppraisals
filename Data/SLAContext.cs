using Microsoft.EntityFrameworkCore;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Data
{
    public class SLAContext : DbContext
    {
        public SLAContext(DbContextOptions<SLAContext> options) : base(options) { }

        public DbSet<StudyLeaveRequests> StudyLeaveRequests { get; set; }
        public DbSet<StudyLeaveFunds> StudyLeaveFunds { get; set; }
        public DbSet<Supervisors> Supervisors { get; set; }
        public DbSet<TriageTotal> TriageTotal { get; set; }
    }
}
