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
        private readonly AppointmentData _appointmentData;
        private readonly StaffData _staffData;
        private readonly PrintServices printer;
        public IndexModel(DataContext context)
        {
            _context = context;
            _appointmentData = new AppointmentData(_context);
            _staffData = new StaffData(_context);
            printer = new PrintServices();
        }

        public List<StaffMembers> staffMembers { get; set; }
        public List<Appointments> appointments { get; set; }
        public List<Appointments> mdcs { get; set; }
        public List<Appointments> totalappts { get; set; }
        public List<Appointments> apptsPerClinic { get; set; }        
        public string staffCode { get; set; }
        public string staffName { get; set; }
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
                    staffName = _staffData.GetStaffName(User.Identity.Name);
                    staffCode = _staffData.GetStaffCode(User.Identity.Name);                    
                    isSupervisor = _staffData.GetIsConsSupervisor(staffCode);
                }

                if(clinicianCode == null)
                {
                    clinicianCode = staffCode;
                }

                staffMembers = _staffData.GetStaffMembers();
                
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
                appointments = _appointmentData.GetAppointments(clinicianCode, startDate, endDate);
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
                isSupervisor = _staffData.GetIsConsSupervisor(staffCode);
                staffMembers = _staffData.GetStaffMembers();
                
                if (clinicianCode != null)
                {
                    staffCode = clinicianCode;
                }
                
                appointments = _appointmentData.GetAppointments(staffCode, startDate, endDate);
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

                if (isPrintReq.GetValueOrDefault())
                {
                    printer.PrintReport(totalappts, clinName, startDate, endDate);
                                       
                    Response.Redirect("Download?sClin=" + clinName + "&startDate=" + startDate.Value.ToString("yyyy-MM-dd") + "&endDate=" + endDate.Value.ToString("yyyy-MM-dd"));
                    
                }

            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        
    }
}
