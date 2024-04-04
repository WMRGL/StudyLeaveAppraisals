using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;

namespace StudyLeaveAppraisals.Pages
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

        public string staffCode { get; set; }
        public string staffName { get; set; }
        //public bool isSupervisor { get; set; }


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
                    staffName = _meta.GetStaffName(User.Identity.Name);
                    staffCode = _meta.GetStaffCode(User.Identity.Name);                    
                    //isSupervisor = _meta.GetIsSupervisor(staffCode);
                }

            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
