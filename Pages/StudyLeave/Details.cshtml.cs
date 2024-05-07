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
        private readonly AppointmentData _meta;
        private readonly StaffData _staffData;
        private readonly StudyLeaveRequestsData _studyLeaveRequestsData;
        private readonly DoSQL _sql;
        public DetailsModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _meta = new AppointmentData(_context);
            _staffData = new StaffData(_context);
            _studyLeaveRequestsData = new StudyLeaveRequestsData(_context);
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
                staffName = _staffData.GetStaffName(User.Identity.Name);
                staffCode = _staffData.GetStaffCode(User.Identity.Name);             
                isSupervisor = _staffData.GetIsGCSupervisor(staffCode);
                Request = _studyLeaveRequestsData.GetRequestDetails(ID);
                Funds = _studyLeaveRequestsData.GetFunds();
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
                staffName = _staffData.GetStaffName(User.Identity.Name);
                staffCode = _staffData.GetStaffCode(User.Identity.Name);                
                isSupervisor = _staffData.GetIsGCSupervisor(staffCode);
                Request = _studyLeaveRequestsData.GetRequestDetails(id);
                Funds = _studyLeaveRequestsData.GetFunds();

                if (granted != null & totalGranted != null & fund != null & fundYear != null)
                {
                    _sql.ApproveStudyLeaveRequest(id, granted, totalGranted, fund, fundYear, _staffData.GetStaffName(User.Identity.Name));
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
