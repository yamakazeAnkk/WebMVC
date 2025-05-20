using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectMVC.DTO;

namespace ProjectMVC.Controllers
{
    [Route("register")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger)
        {
            _logger = logger;
        }

        // Explicit route for Index
        [Route("")]
        public IActionResult Index()
        {
            var model = new RegisterDTO();
            return View(model);
        }

        // Explicit route for Error (Error page)
        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // Post action for registration
        [HttpPost]
        [Route("register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                TempData["Message"] = "Registration successful";
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }

}