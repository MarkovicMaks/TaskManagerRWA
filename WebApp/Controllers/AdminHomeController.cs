using TM.BL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace exercise_13.Controllers
{
    [Authorize]
    public class AdminHomeController : Controller
    {
        private readonly ILogger<AdminHomeController> _logger;

        public AdminHomeController(ILogger<AdminHomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
