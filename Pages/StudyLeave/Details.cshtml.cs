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
        private readonly Metadata _meta;
        private readonly DoSQL _sql;
        public DetailsModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _meta = new Metadata(_context);
            _sql = new DoSQL(_config);
            _config = config;
        }

        public StudyLeaveRequests Request {  get; set; }
        public IEnumerable<StudyLeaveFunds> Funds { get; set; }
        public string staffName { get; set; }
        public string staffCode { get; set; }
        public bool isSupervisor = false;
        public string? Message;
        public bool isSuccess = false;

        [Authorize]
        public void OnGet(int ID)
        {
            try
            {
                staffName = _meta.GetStaffName(User.Identity.Name);
                staffCode = _meta.GetStaffCode(User.Identity.Name);
                //if (_meta.supervisorStaffCodes.Contains(staffCode))
                //{
                //    isSupervisor = true;
                //}
                isSupervisor = _meta.GetIsGCSupervisor(staffCode);
                Request = _meta.GetRequestDetails(ID);
                Funds = _meta.GetFunds();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int id, string granted, int totalGranted, string fund, int fundYear)
        {
            try
            {
                staffName = _meta.GetStaffName(User.Identity.Name);
                staffCode = _meta.GetStaffCode(User.Identity.Name);                
                isSupervisor = _meta.GetIsGCSupervisor(staffCode);
                Request = _meta.GetRequestDetails(id);
                Funds = _meta.GetFunds();

                if (granted != null & totalGranted != null & fund != null & fundYear != null)
                {
                    _sql.ApproveStudyLeaveRequest(id, granted, totalGranted, fund, fundYear, _meta.GetStaffName(User.Identity.Name));
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
