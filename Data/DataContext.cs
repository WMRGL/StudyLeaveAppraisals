using Microsoft.EntityFrameworkCore;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<StaffMembers> StaffMembers { get; set; }
        public DbSet<StudyLeaveRequests> StudyLeaveRequests { get; set; }
        public DbSet<StudyLeaveFunds> StudyLeaveFunds { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<Supervisors> Supervisors { get; set; }
    }
}
