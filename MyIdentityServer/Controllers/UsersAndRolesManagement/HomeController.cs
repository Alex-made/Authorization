using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyIdentityServer.Dal;
using MyIdentityServer.Models;

namespace MyIdentityServer.Controllers.UsersAndRolesManagement
{
	[Authorize(Roles = "Admin")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public HomeController(ILogger<HomeController> logger, 
							  ApplicationDbContext context, 
							  UserManager<IdentityUser> userManager,
							  RoleManager<IdentityRole> roleManager)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public IActionResult Index()
		{
			var users = _context.Users;
			var roles = _roleManager.Roles;
			return View(users);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
