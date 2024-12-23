using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;

namespace StudyLeaveAppraisals.Pages.Appraisals
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ClinicalContext _context;
        private readonly SLAContext _slaContext;
        private readonly IAppointmentData _appointmentData;
        private readonly ReferralData _referralData;
        private readonly IStaffUserData _staffData;
        private readonly ISupervisorData _supervisorData;
        private readonly PrintServices printer;
        private readonly DoSQL _sql;
        public IndexModel(ClinicalContext context, SLAContext slaContext, IConfiguration config)
        {
            _config = config;
            _slaContext = slaContext;
            _context = context;
            _appointmentData = new AppointmentData(_context);
            _referralData = new ReferralData(_context);
            _staffData = new StaffUserData(_context);
            _supervisorData = new SupervisorData(_context);
            printer = new PrintServices();
            _sql = new DoSQL(_config);
        }

        public List<StaffMember> staffMembers { get; set; }
        public List<Appointment> appointments { get; set; }
        public List<Appointment> mdcs { get; set; }
        public List<Appointment> totalappts { get; set; }
        public List<Appointment> apptsPerClinic { get; set; }
        public List<Referral> referrals { get; set; }
        public string staffCode { get; set; }
        public string staffName { get; set; }
        public StaffMember staffMember { get; set; }
        public bool isSupervisor { get; set; }
        public string clinCode;
        public DateTime startDate;
        public DateTime endDate;
        public int patientsSeen;
        public int patientsSeenByAnother;
        public int cancellations;
        public int dnas;
        public int notRecorded;
        public int clinicsHeld;
        public int totalAppointments;
        public int incompleteRefs;
        public int newRefs;
        public int reRefs;
        public int secondaryRefs;
        public int tempReg;
        public int notEntered;
        public int routine;
        public int fastTrack;
        public int urgent;

        public bool isSuccess;
        public string Message;

        [Authorize]
        public void OnGet(string? clinicianCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                isSupervisor = false;
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {                    
                    //staffName = _staffData.GetStaffName(User.Identity.Name);
                    staffMember = _staffData.GetStaffMemberDetails(User.Identity.Name);
                    //staffCode = _staffData.GetStaffCode(User.Identity.Name);
                    staffCode = staffMember.STAFF_CODE;
                    isSupervisor = _supervisorData.GetIsConsSupervisor(staffCode);
                    _sql.SqlWriteUsageAudit(staffCode, "", "Appraisals index");
                }

                if(clinicianCode == null)
                {
                    clinicianCode = staffCode;
                }

                staffMembers = _staffData.GetClinicalStaffList();
                
                clinCode = clinicianCode;
                if (startDate == null)
                {
                    startDate = DateTime.Now.AddDays(-365);
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }
                //Data
                appointments = _appointmentData.GetAppointmentsByClinicians(clinicianCode, startDate, endDate);
                mdcs = _appointmentData.GetMDC(clinicianCode, startDate, endDate);
                totalappts = appointments.Concat(mdcs).OrderBy(a => a.BOOKED_DATE).ThenBy(a => a.BOOKED_TIME).ToList();
                


                //Numbers
                this.startDate = startDate.GetValueOrDefault();
                this.endDate = endDate.GetValueOrDefault();
                patientsSeen = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy == a.STAFF_CODE_1).Count();
                patientsSeenByAnother = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy != a.STAFF_CODE_1).Count();
                cancellations = totalappts.Where(a => a.Attendance.Contains("Canc")).Count();
                dnas = totalappts.Where(a => a.Attendance == "Did not attend").Count();
                notRecorded = totalappts.Where(a => a.Attendance == "NOT RECORDED").Count();
                totalAppointments = totalappts.Count();
                clinicsHeld = totalappts.DistinctBy(a => a.BOOKED_DATE).Count();
                
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        public void OnPost(string? clinicianCode, DateTime? startDate, DateTime? endDate, bool? isPrintReq = false)
        {
            try
            {                
                staffName = _staffData.GetStaffName(User.Identity.Name);
                staffCode = _staffData.GetStaffCode(User.Identity.Name);                
                isSupervisor = _supervisorData.GetIsConsSupervisor(staffCode);
                _sql.SqlWriteUsageAudit(staffCode, $"Clinician={clinicianCode}", "Appraisals index");
                staffMembers = _staffData.GetStaffMemberList();
                
                if (clinicianCode != null)
                {
                    staffCode = clinicianCode;
                }
                
                appointments = _appointmentData.GetAppointmentsByClinicians(staffCode, startDate, endDate);
                mdcs = _appointmentData.GetMDC(staffCode, startDate, endDate);
                totalappts = appointments.Concat(mdcs).OrderBy(a => a.BOOKED_DATE).ThenBy(a => a.BOOKED_TIME).ToList();

                
                string clinName = _staffData.GetStaffNameFromStaffCode(staffCode);                
                
                startDate = startDate.GetValueOrDefault();
                endDate = endDate.GetValueOrDefault();
                patientsSeen = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy == a.STAFF_CODE_1).Count();
                patientsSeenByAnother = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy != a.STAFF_CODE_1).Count();
                cancellations = totalappts.Where(a => a.Attendance.Contains("Canc")).Count();
                dnas = totalappts.Where(a => a.Attendance == "Did not attend").Count();
                notRecorded = totalappts.Where(a => a.Attendance == "NOT RECORDED").Count();
                totalAppointments = totalappts.Count();
                clinicsHeld = totalappts.DistinctBy(a => a.BOOKED_DATE).Count();

                //referral data
                referrals = _referralData.GetReferralsByStaffMember(clinicianCode, startDate, endDate);
                


                if (isPrintReq.GetValueOrDefault())
                {
                    string clinNameFixed = clinName.Replace("'", "");

                    printer.PrintReport(totalappts, referrals, clinNameFixed, startDate, endDate);

                    //Response.Redirect("Download?sClin=" + clinNameFixed + "&startDate=" + startDate.Value.ToString("yyyy-MM-dd") + "&endDate=" + endDate.Value.ToString("yyyy-MM-dd"));
                    Response.Redirect("Download?sClin=" + clinNameFixed);

                }

            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        
    }
}
