using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Meta
{
    public interface ITotalTriageData
    {
        public List<TriageTotal> GetAllTriages(string staffCode, DateTime? startDate, DateTime? endDate);
    }

    public class TotalTriageData : ITotalTriageData
    {
        private readonly SLAContext _context;

        public TotalTriageData(SLAContext context)
        {
            _context = context;
        }

        public List<TriageTotal> GetAllTriages(string staffCode, DateTime? startDate, DateTime? endDate)
        {
            var triages = _context.TriageTotal.Where(t => t.TriagedBy == staffCode && t.LogicalDelete == false);

            if (startDate != null)
            {
                triages = triages.Where(t => t.TriagedDate >= startDate);
            }

            if (endDate != null)
            {
                triages = triages.Where(t => t.TriagedDate <= endDate);
            }

            triages = triages.OrderBy(t => t.TriagedDate);

            return triages.ToList();
        }
    }
}
