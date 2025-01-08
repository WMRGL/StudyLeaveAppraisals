using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Data;
using StudyLeaveAppraisals.Meta;
using StudyLeaveAppraisals.Models;
using ClinicalXPDataConnections.Meta;

namespace StudyLeaveAppraisals.Pages.StudyLeave
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ClinicalContext _context;
        private readonly SLAContext _slaContext;
        private readonly IAppointmentData _meta;
        private readonly IStaffUserData _staffData;
        private readonly ISupervisorData _supervisorData;
        private readonly StudyLeaveRequestsData _studyLeaveRequestsData;
        private readonly DoSQL _sql;
        public IndexModel(ClinicalContext context, SLAContext slaContext, IConfiguration config)
        {
            _config = config;
            _slaContext = slaContext;
            _context = context;
            _meta = new AppointmentData(_context);
            _staffData = new StaffUserData(_context);
            _supervisorData = new SupervisorData(_context);
            _studyLeaveRequestsData = new StudyLeaveRequestsData(_slaContext);
            _sql = new DoSQL(_config);
        }

        public List<StudyLeaveRequests> MyRequests { get; set; }
        public List<StudyLeaveRequests> AllRequests { get; set; }
        public List<StudyLeaveFunds> ListFunds { get; set; }
        public List<StaffMember> ListStaffMembers { get; set; }
        public string staffCode;
        public string staffName;
        public bool isSupervisor = false;
        public bool isShowAllSelected {  get; set; }
        public string staffCodeSelected { get; set; }

        [Authorize]
        public void OnGet(bool? isShowAll=false, string? staffMember="")
        {
            try
            {                
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                    staffName = _staffData.GetStaffName(User.Identity.Name);
                    staffCode = _staffData.GetStaffCode(User.Identity.Name);
                    _sql.SqlWriteUsageAudit(staffCode, "", "Study Leave Index", _ip.GetIPAddress());
                    isSupervisor = _supervisorData.GetIsGCSupervisor(staffCode);
                    ListFunds = _studyLeaveRequestsData.GetFunds();
                    ListStaffMembers = _staffData.GetStaffMemberList().Where(s => s.CLINIC_SCHEDULER_GROUPS == "GC").OrderBy(s => s.NAME).ToList();
                    MyRequests = _studyLeaveRequestsData.GetMyRequests(staffCode);
                    if (isSupervisor)
                    {
                        AllRequests = _studyLeaveRequestsData.GetOtherRequests(staffCode);
                        if (!isShowAll.GetValueOrDefault())
                        {
                            AllRequests = AllRequests.Where(r => r.Granted == "Pending").ToList();
                        }
                        if(staffMember != null && staffMember != "") 
                        {
                            AllRequests = AllRequests.Where(r => r.StaffCode == staffMember).ToList();
                        }

                        isShowAllSelected = isShowAll.GetValueOrDefault();
                        staffCodeSelected = staffMember;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Home/Error?sError=" + ex.Message);
            }

        }

        public void OnPost(bool? isShowAll = false, string? staffMember="")
        {
            IPAddressFinder _ip = new IPAddressFinder(HttpContext);
            staffName = _staffData.GetStaffName(User.Identity.Name);
            staffCode = _staffData.GetStaffCode(User.Identity.Name);
            _sql.SqlWriteUsageAudit(staffCode, $"Staffmember={staffMember}", "Study Leave Index", _ip.GetIPAddress());
            isSupervisor = _supervisorData.GetIsGCSupervisor(staffCode);
            ListFunds = _studyLeaveRequestsData.GetFunds();
            ListStaffMembers = _staffData.GetStaffMemberList();
            MyRequests = _studyLeaveRequestsData.GetMyRequests(staffCode);
            AllRequests = _studyLeaveRequestsData.GetOtherRequests(staffCode);            

            Response.Redirect($"StudyLeave?isShowAll={isShowAll}&staffMember={staffMember}");
        }
    }
}
