using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlexAppSecAssign.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecAssign1.Pages
{
    public class ProfileModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public String testString { get; set; }
        public void OnGet()
        {
        }
    }
}
