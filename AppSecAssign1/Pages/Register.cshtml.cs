using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using AlexAppSecAssign.Models;
using AppSecAssign1.Models;
using AppSecAssign1.Services;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AppSecAssign1.Pages
{
    [AutoValidateAntiforgeryToken]
    [ValidateReCaptcha]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile image { get; set; }

        public UserService _svc;
        private IHostingEnvironment _environment;

        public void OnGet()
        {
        }

        public RegisterModel(UserService service, IHostingEnvironment environment)
        {

            _svc = service;
            _environment = environment;
        }
        
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // IF account already exists, dont allow user to create account
                if (_svc.CheckAccountExists(user.Email))
                {
                    // Render error message that account already exists
                    return Page();
                }
                else
                {
                    var imgExt = image.FileName.Split('.').Last();
                    var genImgName = Security.GenerateRandomString(32) + "." + imgExt;
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads", genImgName);
                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }    

                    var password = Security.Hash(user.Password);
                    byte[] cipherKey;
                    byte[] cipherIV;
                    user.Password = password;
                    user.Id = Security.GenerateRandomString(32);
                    (user.CardNumber, cipherKey, cipherIV) = Security.Encrypt(user.CardNumber);
                    user.encKey = cipherKey;
                    user.encIV = cipherIV;
                    user.Image = genImgName;

                    user.PasswordHistory = $"{password};!;";
                    user.AccLockOut = false;
                    user.PasswordChangeTime = DateTime.Now;
                    _svc.AddUser(user);
                }
                return RedirectToPage("Login");
                // return RedirectToPage("Profile", new { password = password, birth = user.DOB });
            }

            return Page();
        }
    }
}
