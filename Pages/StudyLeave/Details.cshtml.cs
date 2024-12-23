using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using StudyLeaveAppraisals.Models;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;

namespace StudyLeaveAppraisals.Pages.StudyLeave
{
    public class DetailsModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly SLAContext _slaContext;
        private readonly IConfiguration _config;
        private readonly AppointmentData _meta;
        private readonly IStaffUserData _staffData;
        private readonly ISupervisorData _supervisorData;
        private readonly StudyLeaveRequestsData _studyLeaveRequestsData;
        private readonly DoSQL _sql;
        public DetailsModel(ClinicalContext context, SLAContext slaContext, IConfiguration config)
        {
            _context = context;
            _slaContext = slaContext;
            _config = config;
            _meta = new AppointmentData(_context);
            _staffData = new StaffUserData(_context);
            _supervisorData = new SupervisorData(_context);
            _studyLeaveRequestsData = new StudyLeaveRequestsData(_slaContext);
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
                _sql.SqlWriteUsageAudit(staffCode, $"ID={ID}", "Study Leave Details");
                isSupervisor = _supervisorData.GetIsGCSupervisor(staffCode);
                Request = _studyLeaveRequestsData.GetRequestDetails(ID);
                Funds = _studyLeaveRequestsData.GetFunds();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int id, string granted, int totalGranted, string fund, int fundYear, bool isCancel)
        {
            try
            {
                staffName = _staffData.GetStaffName(User.Identity.Name);
                staffCode = _staffData.GetStaffCode(User.Identity.Name);                
                isSupervisor = _supervisorData.GetIsGCSupervisor(staffCode);
                Request = _studyLeaveRequestsData.GetRequestDetails(id);
                Funds = _studyLeaveRequestsData.GetFunds();

                if (isCancel)
                {
                    _sql.CancelStudyLeaveRequest(id);
                    isSuccess = true;
                    Message = "Request cancelled.";
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
