using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Meta
{
    public class AppointmentData
    {
        private readonly DataContext _context;        
        
        public AppointmentData(DataContext context) 
        {
            _context = context;            
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



    }
}
