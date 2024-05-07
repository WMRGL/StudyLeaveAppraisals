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
        private readonly AppointmentData _meta;
        private readonly StaffData _staffData;
        private readonly StudyLeaveRequestsData _studyLeaveRequestsData;
        public IndexModel(DataContext context)
        {
            _context = context;
            _meta = new AppointmentData(_context);
            _staffData = new StaffData(_context);
            _studyLeaveRequestsData = new StudyLeaveRequestsData(_context);
        }

        public List<StudyLeaveRequests> MyRequests { get; set; }
        public List<StudyLeaveRequests> AllRequests { get; set; }
        public List<StudyLeaveFunds> ListFunds { get; set; }
        public List<StaffMembers> ListStaffMembers { get; set; }
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
                    staffName = _staffData.GetStaffName(User.Identity.Name);
                    staffCode = _staffData.GetStaffCode(User.Identity.Name);                    
                    isSupervisor = _staffData.GetIsGCSupervisor(staffCode);
                    ListFunds = _studyLeaveRequestsData.GetFunds();
                    ListStaffMembers = _staffData.GetStaffMembers().Where(s => s.CLINIC_SCHEDULER_GROUPS == "GC").OrderBy(s => s.NAME).ToList();
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
            staffName = _staffData.GetStaffName(User.Identity.Name);
            staffCode = _staffData.GetStaffCode(User.Identity.Name);
            isSupervisor = _staffData.GetIsGCSupervisor(staffCode);
            ListFunds = _studyLeaveRequestsData.GetFunds();
            ListStaffMembers = _staffData.GetStaffMembers();
            MyRequests = _studyLeaveRequestsData.GetMyRequests(staffCode);
            AllRequests = _studyLeaveRequestsData.GetOtherRequests(staffCode);            

            Response.Redirect($"StudyLeave?isShowAll={isShowAll}&staffMember={staffMember}");
        }
    }
}
