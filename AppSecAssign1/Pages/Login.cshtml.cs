using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlexAppSecAssign.Models;
using AppSecAssign1.Models;
using AppSecAssign1.Services;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecAssign1.Pages
{
    [ValidateReCaptcha]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string email { get; set; }

        [BindProperty]
        public string password { get; set; }

        public bool verified { get; set; }

        public string message { get; set; }

        private UserService _svc;
        private IMailService _mailService;

        public LoginModel(UserService service, IMailService mailService)
        {
            _svc = service;
            _mailService = mailService;
        }

        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("AuthToken") != null)
            {
                Response.Redirect("Logout");
            }
            // If user already logged in, redirect them to the logout page
            //if (HttpContext.Session.GetString("username") != null && HttpContext.Session.GetString("AuthToken") != null && Request.Cookies["AuthToken"] != null)
            //{
            //    var savedSessionAuth = HttpContext.Session.GetString("AuthToken").ToString();
            //    // Set cookies & retrieve it in the same page (Login page)
            //    var savedCookieAuth = Request.Cookies["AuthToken"].ToString();
            //    if (savedSessionAuth.Equals(savedCookieAuth))
            //    {
            //        RedirectToPage("Logout");
            //    }
            //}
        }

        
        public IActionResult OnPost()
        {
            // Check if email exists in the database first, then check if the login failure count exeeed 3 (If exceed, don't allow login)
            // If all ok, check if password corresponds
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                message = "Fields cannot be empty. Please fill up fields";
                return Page();
            }
            else
            {
                if (!_svc.CheckAccountExists(email))
                {
                    message = "Provided Email Address & Password does not match records";
                    return Page();
                }
                var verified = _svc.CheckPassword(email, password);
                if (verified)
                {
                    var savedUser = _svc.GetUserByEmail(email);
                    var OTP = Security.GenerateRandomString(6);
                    _svc.UpdateOTP(email, OTP);
                    _mailService.SendEmailAsync(email, savedUser.FirstName, OTP);
                    HttpContext.Session.SetString("userEmail", savedUser.Email);
                    return RedirectToPage("OTP");
                    //// Reset the FailedLoginAttempts to 0
                    //_svc.resetFailedLoginAttempts(email);

                    //var savedUser = _svc.GetUserByEmail(email);
                    //// If verified, redirect them to homepage. Else, +1 to their login failure (To the database)
                    //HttpContext.Session.SetString("username", savedUser.FirstName);
                    //string guid = Guid.NewGuid().ToString();
                    //HttpContext.Session.SetString("AuthToken", guid);
                    //HttpContext.Session.SetString("userId", savedUser.Id);
                    //CookieOptions option = new CookieOptions
                    //{
                    //    Expires = DateTime.Now.AddMinutes(30),
                    //    Path = "/"
                    //};
                    //Response.Cookies.Append("AuthToken", guid, option);
                    //return RedirectToPage("Logout");
                }

                else
                {
                    message = "Provided Email Address & Password does not match records";
                    return Page();
                }
            }
        }
    }
}
