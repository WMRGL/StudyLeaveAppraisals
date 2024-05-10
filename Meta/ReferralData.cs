using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Meta
{
    public class ReferralData
    {
        private readonly DataContext _context;        
        
        public ReferralData(DataContext context) 
        {
            _context = context;            
        }        

               
        public List<Referrals> GetReferrals(string staffCode, DateTime? startDate, DateTime? endDate)
        {
            var refs = _context.Referrals.Where(r => r.PATIENT_TYPE_CODE == staffCode ||
                                                    r.GC_CODE == staffCode);

            refs = refs.Where(a => a.RefDate > startDate);
            refs = refs.Where(a => a.RefDate < endDate);

            return refs.ToList();
        }
          

    }
}
