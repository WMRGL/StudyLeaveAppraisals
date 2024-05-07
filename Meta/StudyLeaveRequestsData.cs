using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Meta
{
    public class StudyLeaveRequestsData
    {
        private readonly DataContext _context;        
        
        public StudyLeaveRequestsData(DataContext context) 
        {
            _context = context;            
        }        

        

        public List<StudyLeaveRequests> GetMyRequests(string staffCode) 
        {
            var req = _context.StudyLeaveRequests.Where(r => r.StaffCode == staffCode).OrderByDescending(r => r.DateRequested);

            return req.ToList();             
        }

        public List<StudyLeaveRequests> GetOtherRequests(string staffCode)
        {
            var req = _context.StudyLeaveRequests.Where(r => r.StaffCode != staffCode).OrderByDescending(r => r.DateRequested);

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



    }
}
