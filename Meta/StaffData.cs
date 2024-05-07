using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Meta
{
    public class StaffData
    {
        private readonly DataContext _context;        
        
        public StaffData(DataContext context) 
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


        public List<StaffMembers> GetStaffMembers() 
        { 
            var staff = _context.StaffMembers.Where(s => s.InPost == true & s.CLINIC_SCHEDULER_GROUPS != "Admin").OrderBy(s => s.NAME);

            return staff.ToList();
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
