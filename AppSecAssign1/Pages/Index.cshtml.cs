using AppSecAssign1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSecAssign1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IMailService _mailService;

        public IndexModel(ILogger<IndexModel> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        public async void OnGet()
        {
            // await _mailService.SendEmailAsync("alexchien09@gmail.com", "Mistah", "123456");
        }
    }
}
