using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AppSecAssign1.Models;
using AppSecAssign1.Services;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecAssign1.Pages
{
    [ValidateReCaptcha]
    public class OTPModel : PageModel
    {
        private IMailService _mailService;
        private UserService _svc;

        public string userEmail { get; set; }

        [BindProperty]
        [Required]
        public string OTP { get; set; }

        public string message { get; set; }




        public OTPModel(IMailService mailService, UserService service)
        {
            _mailService = mailService;
            _svc = service;
        }

        public IActionResult OnGet()
        {
            userEmail = HttpContext.Session.GetString("userEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("Login");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            userEmail = HttpContext.Session.GetString("userEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("Login");
            }

            if (string.IsNullOrEmpty(OTP))
            {
                message = "Fields cannot be empty. Please fill up fields";
                return Page();
            }

            // If all validation passes (OTP & Session input is valid)
            else
            {
                // OTP must be reissued after x mins
                var reissueAfter = 30;

                var savedUser = _svc.GetUserByEmail(userEmail);
                var timeDiff = 0.0;
                // If OTPIssuedTime exists, we will udpate it. Else, timeDiff remains = 0
                if (savedUser.OTPIssuedTime != null)
                {
                    timeDiff = (DateTime.Now - savedUser.OTPIssuedTime).TotalMinutes;
                }

                // Reissue OTP here
                if (timeDiff > reissueAfter)
                {
                    var OTP = Security.GenerateRandomString(6);
                    _svc.UpdateOTP(userEmail, OTP);
                    _mailService.SendEmailAsync(userEmail, savedUser.FirstName, OTP);
                    return Page();
                }

                // If OTP timeout is not reached
                else
                {
                    // If OTP is correct
                    if (OTP == savedUser.OTP)
                    {
                        // Reset the FailedLoginAttempts to 0
                        _svc.resetFailedLoginAttempts(userEmail);

                        // If verified, redirect them to homepage. Else, +1 to their login failure (To the database)
                        HttpContext.Session.SetString("username", savedUser.FirstName);
                        string guid = Guid.NewGuid().ToString();
                        HttpContext.Session.SetString("AuthToken", guid);
                        HttpContext.Session.SetString("userId", savedUser.Id);
                        CookieOptions option = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(30),
                            Path = "/"
                        };
                        Response.Cookies.Append("AuthToken", guid, option);
                        return RedirectToPage("Logout");
                    }
                    // If OTP is wrong
                    else 
                    {
                        message = "OTP is incorrect!";
                        return Page();
                    }
                }
            }
        }
    }
}
