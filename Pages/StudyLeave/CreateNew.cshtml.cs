using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;

namespace StudyLeaveAppraisals.Pages.StudyLeave
{
    public class CreateNewModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IAppointmentData _meta;
        private readonly IStaffUserData _staffData;
        private readonly DoSQL _sql;
        public CreateNewModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _meta = new AppointmentData(_context);
            _staffData = new StaffUserData(_context);
            _sql = new DoSQL(_config);
            _config = config;
        }

        public string staffName { get; set; }
        public string? Message;
        public bool isSuccess = false;

        [Authorize]
        public void OnGet()
        {
            try
            {
                staffName = _staffData.GetStaffName(User.Identity.Name);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string eventName, DateTime eventDate, int travelCost, int accomCost, int eventCost, int days, int totalReq, DateTime dateRequested)
        {
            try
            {
                staffName = _staffData.GetStaffName(User.Identity.Name);
                if (eventName != null & eventDate != DateTime.MinValue & travelCost != null & accomCost != null & eventCost != null & days != null &
                    totalReq != null & dateRequested != DateTime.MinValue)
                {
                    _sql.CreateNewStudyLeaveRequest(eventName.Replace("'", "''"), eventDate, travelCost, accomCost, eventCost, days, totalReq, dateRequested, _staffData.GetStaffCode(User.Identity.Name), _staffData.GetStaffName(User.Identity.Name));
                    isSuccess = true;
                    Message = "Saved!";
                }
                else
                {                    
                    isSuccess = false;
                    Message = "Missing data, please correct.";
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
