using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace StudyLeaveAppraisals.Models 
{
    [Table("STAFF", Schema = "dbo")]
    public class StaffMembers
    {
        [Key]
        public string STAFF_CODE { get; set; }
        public string? EMPLOYEE_NUMBER { get; set; }
        public string? NAME { get; set; }
        public string? POSITION { get; set; }
        public bool InPost { get; set; }
        public string CLINIC_SCHEDULER_GROUPS { get; set; }
    }

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

    [Table("ViewPatientAppointmentDetails", Schema = "dbo")]
    public class Appointments
    {
        [Key]
        public int RefID { get; set; }
        public DateTime? BOOKED_DATE { get; set; }
        public DateTime? BOOKED_TIME { get; set; }
        public string? STAFF_CODE_1 { get; set; }
        public string? STAFF_CODE_2 { get; set; }
        public string? STAFF_CODE_3 { get; set; }
        public string? AppType { get; set; }
        public string? SeenBy { get; set; }
        public string? SeenBy2 { get; set; }
        public string? SeenBy3 { get; set; }
        public string? Attendance { get; set; }
        public string? Clinic { get; set; }
        public string? Clinician { get; set; }
        public string? Clinician2 { get; set; }
        public string? Clinician3 { get; set; }
        public string? SeenByClinician { get; set; }        
    }

    [Table("ViewPatientReferralDetails", Schema = "dbo")]
    public class Referrals
    {
        [Key]
        public int RefID { get; set; }
        public DateTime RefDate { get; set; }
        public string RefType { get; set; }
        public string? RefClass { get; set; }
        public string? PATIENT_TYPE_CODE { get; set; }
        public string? GC_CODE {  get; set; }
    }

        [Table("ClinicSlotsAll", Schema ="dbo")]
    public class Slots
    {
        [Key]
        public int SlotID { get; set; }
        public DateTime SlotDate { get; set; }
        public DateTime SlotTime { get; set; }
        public string ClinicianID { get; set; }
        public string ClinicID { get; set; }
        public string SlotStatus { get; set; }
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

    [Table("Notifications", Schema = "dbo")]
    public class Notifications
    {
        [Key]
        public int ID { get; set; }
        public string MessageCode { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }
}
