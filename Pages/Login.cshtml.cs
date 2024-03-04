using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using StudyLeaveAppraisals.Models;

namespace StudyLeaveAppraisals.Pages
{
    public class LoginModel : PageModel
    {
        UserDataAccessLayer objUser = new UserDataAccessLayer();
        public UserDetails? UserDetails { get; set; }

        public async void OnGet(string sUsername, string sPassword)
        {
            //if (ModelState.IsValid)
            if (sUsername != null)
            {
                UserDetails user = new UserDetails();
                user.EMPLOYEE_NUMBER = sUsername;
                user.PASSWORD = sPassword;

                string LoginStatus = objUser.ValidateLogin(user);

                if (LoginStatus == "Success")
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.EMPLOYEE_NUMBER)
                    };
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync(principal);
                    Response.Redirect("Index");
                }
                else
                {

                    TempData["UserLoginFailed"] = "Login failed. Please try again.";
                    //return View();
                }
            }
        }
    }
}
