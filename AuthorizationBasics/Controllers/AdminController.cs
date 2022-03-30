using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationBasics.Controllers
{
	public class AdminController : Controller
	{
		[Authorize]
		public IActionResult Index()
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
			//мы написали свой claim (утверждение), на основе которого омжем давать доступ
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Role, "ClaimValue")
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
