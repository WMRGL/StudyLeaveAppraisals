using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;

namespace StudyLeaveAppraisals.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffUserData _staffData;
        private readonly NotificationData _notificationData;

        public IndexModel(ClinicalContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _staffData = new StaffUserData(_context);
            _notificationData = new NotificationData(_context);
        }

        public string staffCode { get; set; }
        public string staffName { get; set; }
        public string notificationMessage { get; set; }
        public bool isLive { get; set; }


        [Authorize]
        public void OnGet()
        {
            try
            {
                //isSupervisor = false;
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    notificationMessage = _notificationData.GetMessage("SLAOutage");
                    staffName = _staffData.GetStaffName(User.Identity.Name);
                    staffCode = _staffData.GetStaffCode(User.Identity.Name);                    
                    isLive = bool.Parse(_config.GetValue("IsLive", ""));
                }

            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
