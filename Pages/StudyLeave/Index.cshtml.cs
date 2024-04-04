using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Pages.StudyLeave
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;
        private readonly Metadata _meta;

        public IndexModel(DataContext context)
        {
            _context = context;
            _meta = new Metadata(_context);
        }

        public List<StudyLeaveRequests> MyRequests { get; set; }
        public List<StudyLeaveRequests> AllRequests { get; set; }
        public List<StudyLeaveFunds> ListFunds { get; set; }
        public string staffCode;
        public string staffName;
        public bool isSupervisor = false;

        [Authorize]
        public void OnGet()
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    staffName = _meta.GetStaffName(User.Identity.Name);
                    staffCode = _meta.GetStaffCode(User.Identity.Name);                    
                    isSupervisor = _meta.GetIsGCSupervisor(staffCode);
                    ListFunds = _meta.GetFunds();
                    MyRequests = _meta.GetMyRequests(staffCode);
                    if (isSupervisor)
                    {
                        AllRequests = _meta.GetOtherRequests(staffCode);
                    }

                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Home/Error?sError=" + ex.Message);
            }

        }
    }
}
