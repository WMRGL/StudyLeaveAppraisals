using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Pages.StudyLeave
{
    public class DetailsModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public Metadata meta;
        public DoSQL sql;
        public DetailsModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            meta = new Metadata(_context);
            sql = new DoSQL(_config);
            _config = config;
        }

        public StudyLeaveRequests Request {  get; set; }
        public IEnumerable<StudyLeaveFunds> Funds { get; set; }
        public string staffName { get; set; }
        public string staffCode { get; set; }
        public bool isSupervisor = false;
        public string? sMessage;
        public bool isSuccess = false;

        [Authorize]
        public void OnGet(int ID)
        {
            try
            {
                staffName = meta.GetStaffName(User.Identity.Name);
                staffCode = meta.GetStaffCode(User.Identity.Name);
                //if (meta.supervisorStaffCodes.Contains(staffCode))
                //{
                //    isSupervisor = true;
                //}
                isSupervisor = meta.GetIsGCSupervisor(staffCode);
                Request = meta.GetRequestDetails(ID);
                Funds = meta.GetFunds();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int ID, string sStaffCode, string sGranted, int iTotalGranted, string sFund, int iFundYear)
        {
            try
            {
                staffName = meta.GetStaffName(User.Identity.Name);
                staffCode = meta.GetStaffCode(User.Identity.Name);                
                isSupervisor = meta.GetIsGCSupervisor(staffCode);
                Request = meta.GetRequestDetails(ID);
                Funds = meta.GetFunds();

                if (sGranted != null & iTotalGranted != null & sFund != null & iFundYear != null)
                {
                    sql.ApproveStudyLeaveRequest(ID, sGranted, iTotalGranted, sFund, iFundYear, meta.GetStaffName(User.Identity.Name));
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
