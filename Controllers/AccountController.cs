using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Services;
using ProjectMVC.DTO;
using ProjectMVC.Data;
using ProjectMVC.Services.Interfaces;

namespace ProjectMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly EmployeeContext _context;

        public AccountController(IAuthService authService, EmployeeContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

       

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
                return View(model);
            }

            try
            {
                var result = await _authService.RegisterAsync(model);
                if (result.success)
                {
                    // Auto login after successful registration
                    var loginModel = new LoginDTO
                    {
                        Username = model.Username,
                        Password = model.Password
                    };
                    await _authService.LoginAsync(loginModel);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View(model);
        }
    }
}