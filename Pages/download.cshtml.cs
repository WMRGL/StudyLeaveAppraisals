using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudyLeaveAppraisals.Pages
{
    public class downloadModel : PageModel
    {
        public string dlFileName {  get; set; }
     


        public void OnGet(string sClin, DateTime dStart, DateTime dEnd)
        {
            dlFileName = sClin.Replace(" ", "_") + "_" + dStart.ToString("yyyy-MM-dd") + "_" + dEnd.ToString("yyyy-MM-dd");
            //it just pure and simple straight up flat-out REFUSES to download it to the client!!!!
            //Response.WriteAsync("C:\\CGU_DB\\" + dlFileName).Wait();
        }
    }
}
