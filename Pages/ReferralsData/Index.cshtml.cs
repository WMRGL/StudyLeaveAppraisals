using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudyLeaveAppraisals.Meta;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;
using StudyLeaveAppraisals.Models;
using StudyLeaveAppraisals.Data;

namespace StudyLeaveAppraisals.Pages.ReferralsData
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ClinicalContext _context;
        private readonly IReferralData _referralData;
        private readonly IStaffUserData _staffData;        
        private readonly IDiseaseData _diseaseData;
        private readonly DoSQL _sql;
        public IndexModel(ClinicalContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
            _referralData = new ReferralData(_context);
            _staffData = new StaffUserData(_context);           
            _diseaseData = new DiseaseData(_context);
            _sql = new DoSQL(_config);
        }


        public List<StaffMember> staffMembers { get; set; }
        //public List<StaffMember> consultants { get; set; }
        //public List<StaffMember> gcs { get; set; }
        public List<Referral> referrals { get; set; }
        public List<Disease> diseases { get; set; }
        public string staffCode { get; set; }
        public string staffName { get; set; }
        public StaffMember staffMember { get; set; }
        public string clinician;        
        public string disCode;
        public DateTime startDate;
        public DateTime endDate;
        

        public bool isSuccess;
        public string Message;


        [Authorize]
        public void OnGet(string? clinicianCode, string? diseaseCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {                    
                    staffMember = _staffData.GetStaffMemberDetails(User.Identity.Name);
                    staffCode = staffMember.STAFF_CODE;
                    _sql.SqlWriteUsageAudit(staffCode, "", "Clinic Data index", _ip.GetIPAddress());
                }

                if(clinicianCode == null)
                {
                    clinicianCode = staffCode;
                }

                //consultants = _staffData.GetConsultantsList();
                //gcs = _staffData.GetGCList();
                staffMembers = _staffData.GetClinicalStaffList();

                diseases = _diseaseData.GetDiseaseList().OrderBy(d => d.DISEASE_CODE).ToList();
                                
                disCode = diseaseCode;

                if (startDate == null)
                {
                    startDate = DateTime.Now.AddDays(-365);
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                referrals = _referralData.GetReferralsByStaffMember(clinicianCode, startDate, endDate);

                this.startDate = startDate.GetValueOrDefault();
                this.endDate = endDate.GetValueOrDefault();
                                
                clinician = clinicianCode;

            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        public void OnPost(string? clinicianCode, string? venueCode, string? outcome, string? seenby, string? diseaseCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                staffName = _staffData.GetStaffName(User.Identity.Name);
                staffCode = _staffData.GetStaffCode(User.Identity.Name);  
                staffMember = _staffData.GetStaffMemberDetails(User.Identity.Name);
                _sql.SqlWriteUsageAudit(staffCode, $"Clinician={clinicianCode}", "Clinic Data index", _ip.GetIPAddress());                

                diseases = _diseaseData.GetDiseaseList().OrderBy(d => d.DISEASE_CODE).ToList();

                if (clinicianCode != null)
                {
                    staffCode = clinicianCode;
                }

                if (clinicianCode == null)
                {
                    clinicianCode = staffCode;
                }

                staffMembers = _staffData.GetClinicalStaffList();

                diseases = _diseaseData.GetDiseaseList().OrderBy(d => d.DISEASE_CODE).ToList();


                disCode = diseaseCode;

                if (startDate == null)
                {
                    startDate = DateTime.Now.AddDays(-365);
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                referrals = _referralData.GetReferralsByStaffMember(clinicianCode, startDate, endDate);
                                
                clinician = clinicianCode;

                this.startDate = startDate.GetValueOrDefault();
                this.endDate = endDate.GetValueOrDefault();              
                
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        
    }
}
