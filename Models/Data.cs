using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace StudyLeaveAppraisals.Models 
{
    

    [Table("StudyLeaveRequests", Schema = "dbo")]
    public class StudyLeaveRequests
    {
        [Key]
        public int ID { get; set; }
        public string Event { get; set; }
        public DateTime DateOfEvent { get; set; }
        public int? CostTravel { get; set; }
        public int? CostAccomodation { get; set; }
        public int? CostEvent { get; set; }
        public int? Duration { get; set; }
        public string? Fund { get; set; }
        public int? FundYear { get; set; }
        public int? TotalGranted { get; set; }
        public int? TotalRequested { get; set; }
        public int? TotalCosts { get; set; }
        public string? Granted { get; set; }
        public string? GrantedBy { get; set; }
        public DateTime? GrantedDate { get; set; }
        public string? StaffCode { get; set; }
        public bool? Attended { get; set; }
        public string RequesterName { get; set; }
        public DateTime DateRequested { get; set; }
        public bool LogicalDelete { get; set; }
    }

    [Table("StudyLeaveFunds", Schema = "dbo")]
    public class StudyLeaveFunds
    {
        [Key]
        public int ID { get; set; }
        public string Fund { get; set; }
        public bool Active { get; set; }

    }    

    [Table("ListSupervisors", Schema = "dbo")]
    public class  Supervisors
    {
        [Key]
        public int ID { get; set; }
        public string StaffCode { get; set; }        
        public bool isGCSupervisor { get; set; }
        public bool isConsSupervisor { get; set; }
    }

    [Keyless]
    [Table("ViewPatientTriagesAll", Schema = "dbo")]
    public class TriageTotal
    {
        //[Key]
        public int ICPID { get; set; }
        public int RefID { get; set; }
        public int MPI {  get; set; }
        public string CGU_No { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public bool Triaged { get; set; }
        public string? TriagedBy { get; set; }
        public string? TriagedByClinician { get; set; }
        public DateTime? TriagedDate { get; set; }
        public bool LogicalDelete { get; set; }
        public string? Type { get; set; }
        public string PATHWAY { get; set; }
        public string? WaitingListClinician { get; set; }
        public string? WaitingListClinicianName { get; set; }
        public string? WaitingListClinic { get; set; }
        public string? WaitingListClinicName { get; set; }
    }
}
