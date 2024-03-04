using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Pages.Appraisals
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;
        private Metadata meta;
        public IndexModel(DataContext context)
        {
            _context = context;
            meta = new Metadata(_context);
        }

        public List<StaffMembers> staffMembers { get; set; }
        public List<Appointments> appointments { get; set; }
        public List<Appointments> mdcs { get; set; }
        public List<Appointments> totalappts { get; set; }
        public string staffCode { get; set; }
        public string staffName { get; set; }
        public bool isSupervisor { get; set; }
        public string sClinCode;
        public DateTime startDate;
        public DateTime endDate;
        public int iPatientsSeen;
        public int iPatientsSeenByAnother;
        public int iCancellations;
        public int iDNAs;
        public int iNotRecorded;
        public int iClinicsHeld;
        public int iTotalAppointments;

        [Authorize]
        public void OnGet(string? clinicianCode, DateTime? dStart, DateTime? dEnd)
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
                    staffName = meta.GetStaffName(User.Identity.Name);
                    staffCode = meta.GetStaffCode(User.Identity.Name);                    
                    isSupervisor = meta.GetIsConsSupervisor(staffCode);
                }

                if(clinicianCode == null)
                {
                    clinicianCode = staffCode;
                }

                staffMembers = meta.GetStaffMembers();
                
                sClinCode = clinicianCode;
                if (dStart == null)
                {
                    dStart = DateTime.Now.AddDays(-365);
                }
                if (dEnd == null)
                {
                    dEnd = DateTime.Now;
                }

                appointments = meta.GetAppointments(clinicianCode, dStart, dEnd);
                mdcs = meta.GetMDC(clinicianCode, dStart, dEnd);
                totalappts = appointments.Concat(mdcs).OrderBy(a => a.BOOKED_DATE).ThenBy(a => a.BOOKED_TIME).ToList();

                startDate = dStart.GetValueOrDefault();
                endDate = dEnd.GetValueOrDefault();
                iPatientsSeen = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy == a.STAFF_CODE_1).Count();
                iPatientsSeenByAnother = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy != a.STAFF_CODE_1).Count();
                iCancellations = totalappts.Where(a => a.Attendance.Contains("Canc")).Count();
                iDNAs = totalappts.Where(a => a.Attendance == "Did not attend").Count();
                iNotRecorded = totalappts.Where(a => a.Attendance == "NOT RECORDED").Count();
                iTotalAppointments = totalappts.Count();
                iClinicsHeld = totalappts.DistinctBy(a => a.BOOKED_DATE).Count();

                //totalappts = totalappts.GroupBy(a => a.BOOKED_DATE.Value.Month).SelectMany(gr => gr).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        public void OnPost(string? clinicianCode, DateTime? dStart, DateTime? dEnd)
        {
            try
            {
                staffName = meta.GetStaffName(User.Identity.Name);
                staffCode = meta.GetStaffCode(User.Identity.Name);                
                isSupervisor = meta.GetIsConsSupervisor(staffCode);
                staffMembers = meta.GetStaffMembers();
                appointments = meta.GetAppointments(clinicianCode, dStart, dEnd);
                mdcs = meta.GetMDC(clinicianCode, dStart, dEnd);
                totalappts = appointments.Concat(mdcs).OrderBy(a => a.BOOKED_DATE).ThenBy(a => a.BOOKED_TIME).ToList();

                sClinCode = clinicianCode;
                startDate = dStart.GetValueOrDefault();
                endDate = dEnd.GetValueOrDefault();
                iPatientsSeen = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy == a.STAFF_CODE_1).Count();
                iPatientsSeenByAnother = totalappts.Where(a => a.Attendance == "Attended" && a.SeenBy != a.STAFF_CODE_1).Count();
                iCancellations = totalappts.Where(a => a.Attendance.Contains("Canc")).Count();
                iDNAs = totalappts.Where(a => a.Attendance == "Did not attend").Count();
                iNotRecorded = totalappts.Where(a => a.Attendance == "NOT RECORDED").Count();
                iTotalAppointments = totalappts.Count();
                iClinicsHeld = totalappts.DistinctBy(a => a.BOOKED_DATE).Count();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
