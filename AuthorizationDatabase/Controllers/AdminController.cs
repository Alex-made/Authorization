using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationDatabase.Data;
using AuthorizationDatabase.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationDatabase.Controllers
{
	public class AdminController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[Authorize]
		public IActionResult Index()
		{
			return View();
		}

		//[Authorize(Roles = "Administrator")] - старый ролевой способ. если много пользователей, то нужно авторизовывать каждого с кучей указанных ролей.
		[Authorize(Policy = "Administrator")]
		public IActionResult Administrator()
		{
			return View();
		}

		//[Authorize(Roles = "Manager")]
		[Authorize(Policy = "Manager")]
		public IActionResult Manager()
		{
			return View();
		}

		public IActionResult Login(string returnUrl)
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			//протсо поиск пользователя в БД
			//var users = обычныйДБконтекст.Users.SingleOrDefault(поиск по model.Username и model.Password);
			
			// используем методы поиска пользователя из Microsoft.Identity
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user == null)
			{
				return View();
			}

			var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
			if (result.Succeeded)
			{
				return Redirect(model.ReturnUrl);
			}

			return View();

			////мы написали свой claim (утверждение), на основе которого можем давать доступ
			//var claims = new List<Claim>
			//{
			//	new Claim(ClaimTypes.Role, model.Username)
			//};
			//var claimIdentity = new ClaimsIdentity(claims, "Cookie");
			//var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
			//await HttpContext.SignInAsync("Cookie", claimsPrincipal);

			//return Redirect(model.ReturnUrl);
		}
	}

	public class LoginViewModel
	{
		public string Username
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string ReturnUrl
		{
			get;
			set;
		}
	}
}
