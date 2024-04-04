using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Meta
{
    public class Metadata
    {
        private readonly DataContext _context;        
        
        public Metadata(DataContext context) 
        {
            _context = context;            
        }        

        public string GetStaffCode(string userName)
        {
            string staffCode = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == userName).STAFF_CODE;
            return staffCode;                
        }

        public string GetStaffName(string userName)
        {
            string staffName = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == userName).NAME;
            return staffName;
        }

        public string GetStaffNameFromStaffCode(string staffCode)
        {
            string staffName = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == staffCode).NAME;
            return staffName;
        }

        public List<StudyLeaveRequests> GetMyRequests(string staffCode) 
        {
            var req = _context.StudyLeaveRequests.Where(r => r.StaffCode == staffCode);

            return req.ToList();             
        }

        public List<StudyLeaveRequests> GetOtherRequests(string staffCode)
        {
            var req = _context.StudyLeaveRequests.Where(r => r.StaffCode != staffCode & r.Granted == "Pending");

            return req.ToList();
        }

        public List<StudyLeaveFunds> GetFunds()
        {
            var req = _context.StudyLeaveFunds.Where(f => f.Active == true);

            return req.ToList();
        }

        public StudyLeaveRequests GetRequestDetails(int id)
        {
            var req = _context.StudyLeaveRequests.FirstOrDefault(r => r.ID == id);

            return req;
        }

        public List<StaffMembers> GetStaffMembers() 
        { 
            var staff = _context.StaffMembers.Where(s => s.InPost == true & s.CLINIC_SCHEDULER_GROUPS != "Admin").OrderBy(s => s.NAME);

            return staff.ToList();
        }

        public List<Appointments> GetAppointments(string staffCode, DateTime? startDate, DateTime? endDate)
        {            
            var apt = _context.Appointments.Where(a => (a.STAFF_CODE_1 == staffCode ||
                                                    a.STAFF_CODE_2 == staffCode ||
                                                    a.STAFF_CODE_3 == staffCode)
                                                    & a.AppType.Contains("app"));

            apt = apt.Where(a => a.BOOKED_DATE > startDate);
            apt = apt.Where(a => a.BOOKED_DATE < endDate);

            return apt.ToList();
        }

        public List<Appointments> GetMDC(string staffCode, DateTime? startDate, DateTime? endDate)
        {            
            var apt = _context.Appointments.Where(a => (a.STAFF_CODE_1 == staffCode ||
                                                    a.STAFF_CODE_2 == staffCode ||
                                                    a.STAFF_CODE_3 == staffCode)
                                                    & a.AppType.Contains("MD"));

            apt = apt.Where(a => a.BOOKED_DATE > startDate);
            apt = apt.Where(a => a.BOOKED_DATE < endDate);

            return apt.ToList();
        }

        public List<Appointments> GetAppointmentsByClinic(string staffCode, string clinic, DateTime? startDate, DateTime? endDate)
        {
            var apt = _context.Appointments.Where(a => (a.STAFF_CODE_1 == staffCode ||
                                                    a.STAFF_CODE_2 == staffCode ||
                                                    a.STAFF_CODE_3 == staffCode)
                                                    & (a.AppType.Contains("app") || a.AppType.Contains("MD"))
                                                    & a.Clinic == clinic);

            apt = apt.Where(a => a.BOOKED_DATE > startDate);
            apt = apt.Where(a => a.BOOKED_DATE < endDate);

            return apt.ToList();
        }

        public List<Appointments> GetAppointmentsByMonth(string staffCode, int month, int year)
        {
            var apt = _context.Appointments.Where(a => (a.STAFF_CODE_1 == staffCode ||
                                                    a.STAFF_CODE_2 == staffCode ||
                                                    a.STAFF_CODE_3 == staffCode)
                                                    & (a.AppType.Contains("app") || a.AppType.Contains("MD")));

            DateTime startDate = DateTime.Parse(year + "-" + month  + "-" + 1);
            DateTime endDate = DateTime.Parse(year + "-" + (month+1) + "-" + 1);

            apt = apt.Where(a => a.BOOKED_DATE >= startDate);
            apt = apt.Where(a => a.BOOKED_DATE < endDate);

            return apt.ToList();
        }


        public bool GetIsGCSupervisor(string staffCode)
        {
            var sup = _context.Supervisors.Where(s => s.StaffCode == staffCode && s.isGCSupervisor == true).ToList();
            if (sup.Count > 0) return true;

            return false;
        }

        public bool GetIsConsSupervisor(string staffCode)
        {
            var sup = _context.Supervisors.Where(s => s.StaffCode == staffCode && s.isConsSupervisor == true).ToList();
            if (sup.Count > 0) return true;

            return false;
        }

    }
}
