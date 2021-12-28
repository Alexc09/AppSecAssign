using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class ResetPasswordModel : PageModel
    {
        private readonly UserService _svc;

        [BindProperty(SupportsGet =true)]
        public string message { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
            ErrorMessage = "Password needs a combination of at least 12 lower-case, upper-case, numbers & special characters")]
        public string oldPw { get; set; }
        
        [BindProperty]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
            ErrorMessage = "Password needs a combination of at least 12 lower-case, upper-case, numbers & special characters")]
        public string newPw { get; set; }

        public ResetPasswordModel(UserService service)
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
                    return Page();
                }
            }

            return Redirect("Login");
        }

        public IActionResult OnPost()
        {
            var savedSessionAuth = HttpContext.Session.GetString("AuthToken");
            var savedUserId = HttpContext.Session.GetString("userId");
            var savedCookieAuth = Request.Cookies["AuthToken"];

            if (savedSessionAuth != null)
            {
                if (savedSessionAuth.Equals(savedCookieAuth))
                {
                    User user = _svc.GetUserById(savedUserId);
                    bool correctCurrentPw = _svc.ComparePassword(oldPw, user.Password);
                    // If oldPw is correct, we will allow them to update the password
                    if (correctCurrentPw)
                    {
                        // If the newPw is the same as the previous passwords (Whose hash is in user.PasswordHistory), we will not allow them to change password
                        bool reusedPassword = false;
                        List<String> passwordHistory = user.PasswordHistory.Split(";!;").ToList<String>();
                        // Remove all whitespace elements from list (So
                        passwordHistory.RemoveAll(item => string.IsNullOrWhiteSpace(item));
                        foreach (string hashedPw in passwordHistory)
                        {
                            // If newPw matches an old password, we won't allow them to add
                            if (_svc.ComparePassword(newPw, hashedPw))
                            {
                                reusedPassword = true;
                            }
                        }

                        // If newPw has never been used before, we allow them to change password
                        if (!reusedPassword)
                        {
                            var newHashedPassword = Security.Hash(newPw);

                            // If I already have 2 password histories, delete the earlier one (Index=0) and insert the newPasswordHash at the end of the List
                            if (passwordHistory.Count >= 2)
                            {
                                passwordHistory.RemoveAt(0);
                            }
                            passwordHistory.Add(newHashedPassword);


                            // Loop through each element in the list to create the passwordHistoryString, which is saved in the database (separator=;!;)
                            var newPasswordHistoryStr = "";
                            foreach (string elem in passwordHistory)
                            {
                                newPasswordHistoryStr += $"{elem};!;";
                            }

                            // Update the database entry
                            var updated = _svc.UpdateUserPassword(user.Id, newHashedPassword, newPasswordHistoryStr);
                            if (updated)
                            {
                                message = "Password successfully updated!";
                                return RedirectToPage("Login");
                            }
                            else
                            {
                                message = "Cannot change password within 1 minute of changing!";
                            }
                        }

                        else
                        {
                            message = "Previous passwords cannot be reused!";
                        }
                        
                    }

                    else
                    {
                        message = "Current Password is incorrect!";
                    }
                }

                return Page();
            }

            return Redirect("Login");
        }
    }
}
