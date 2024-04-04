using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudyLeaveAppraisals.Pages
{
    public class downloadModel : PageModel
    {
        public string dlFileName {  get; set; }
     
        public void OnGet(string sClin, DateTime startDate, DateTime endDate)
        {
            dlFileName = sClin.Replace(" ", "_") + "_" + startDate.ToString("yyyy-MM-dd") + "_" + endDate.ToString("yyyy-MM-dd");
            //it just pure and simple straight up flat-out REFUSES to download it to the client!!!!
            //Response.WriteAsync("C:\\CGU_DB\\" + dlFileName).Wait();
        }
    }
}
