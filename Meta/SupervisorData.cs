using StudyLeaveAppraisals.Data;

namespace StudyLeaveAppraisals.Meta
{
    public interface ISupervisorData
    {
        public bool GetIsGCSupervisor(string staffCode);
        public bool GetIsConsSupervisor(string staffCode);
    }
    public class SupervisorData : ISupervisorData
    {
        private readonly SLAContext _clinContext;

        public SupervisorData(SLAContext context)
        {
            _clinContext = context;
        }

        public bool GetIsGCSupervisor(string staffCode)
        {
            var sup = _clinContext.Supervisors.Where(s => s.StaffCode == staffCode && s.isGCSupervisor == true).ToList();
            if (sup.Count > 0) return true;

            return false;
        }

        public bool GetIsConsSupervisor(string staffCode)
        {
            var sup = _clinContext.Supervisors.Where(s => s.StaffCode == staffCode && s.isConsSupervisor == true).ToList();
            if (sup.Count > 0) return true;

            return false;
        }
    }
}
