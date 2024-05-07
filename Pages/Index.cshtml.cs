using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;

namespace StudyLeaveAppraisals.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly AppointmentData _meta;
        private readonly StaffData _staffData;

        public IndexModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _meta = new AppointmentData(_context);
            _staffData = new StaffData(_context);
        }

        public string staffCode { get; set; }
        public string staffName { get; set; }
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
