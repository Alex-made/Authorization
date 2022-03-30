using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationRoles.Controllers
{
	public class AdminController : Controller
	{
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
			//мы написали свой claim (утверждение), на основе которого можем давать доступ
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Role, model.Username)
			};
			var claimIdentity = new ClaimsIdentity(claims, "Cookie");
			var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
			await HttpContext.SignInAsync("Cookie", claimsPrincipal);

			return Redirect(model.ReturnUrl);
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
