using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;

namespace StudyLeaveAppraisals.Pages.StudyLeave
{
    public class CreateNewModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public Metadata meta;
        public DoSQL sql;
        public CreateNewModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            meta = new Metadata(_context);
            sql = new DoSQL(_config);
            _config = config;
        }

        public string staffName { get; set; }
        public string? sMessage;
        public bool isSuccess = false;

        [Authorize]
        public void OnGet()
        {
            try
            {
                staffName = meta.GetStaffName(User.Identity.Name);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(string sEvent, DateTime dEventDate, int iTravelCost, int iAccomCost, int iEventCost, int iDays, int iTotalReq, DateTime dDateRequested)
        {
            try
            {
                staffName = meta.GetStaffName(User.Identity.Name);
                if (sEvent != null & dEventDate != DateTime.MinValue & iTravelCost != null & iAccomCost != null & iEventCost != null & iDays != null &
                    iTotalReq != null & dDateRequested != DateTime.MinValue)
                {
                    sql.CreateNewStudyLeaveRequest(sEvent, dEventDate, iTravelCost, iAccomCost, iEventCost, iDays, iTotalReq, dDateRequested, meta.GetStaffCode(User.Identity.Name), meta.GetStaffName(User.Identity.Name));
                    isSuccess = true;
                    sMessage = "Saved!";
                }
                else
                {                    
                    isSuccess = false;
                    sMessage = "Missing data, please correct.";
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
