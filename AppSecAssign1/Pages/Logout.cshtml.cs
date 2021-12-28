using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlexAppSecAssign.Models;
using AppSecAssign1.Models;
using AppSecAssign1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecAssign1.Pages
{
    public class LogoutModel : PageModel
    {
        public string name { get; set; }
        public string encCardNum { get; set; }
        public string decCardNum { get; set; }
        public string imgName { get; set; }

        public User user { get; set; }
        private readonly UserService _svc;

        public LogoutModel(UserService service)
        {
            _svc = service;
        }

        public IActionResult OnGet()
        {
            var savedSessionAuth = HttpContext.Session.GetString("AuthToken");
            var savedUserId = HttpContext.Session.GetString("userId");
            var savedCookieAuth = Request.Cookies["AuthToken"];

            if (savedSessionAuth != null)
            {
                if (savedSessionAuth.Equals(savedCookieAuth))
                {
                    user = _svc.GetUserById(savedUserId);
                    // Password must be changed after x minutes of not changing password
                    var mustChangeAfter = 30;

                    var timeDiff = (DateTime.Now - user.PasswordChangeTime).TotalMinutes;
                    // If the user has not changed password for x minutes, we force them to change password
                    if (timeDiff > mustChangeAfter)
                    {
                        return RedirectToPage("ResetPassword", new { message = "Please change password to continue" });
                    }
                    encCardNum = user.CardNumber;

                    imgName = user.Image;

                    decCardNum = Security.Decrypt(user.CardNumber, user.encKey, user.encIV);
                    return Page();
                }
            }

            return Redirect("Login");
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("AuthToken");
            return RedirectToPage("Login");
        }
    }
}
