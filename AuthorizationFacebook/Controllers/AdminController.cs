using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationFacebook.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationFacebook.Controllers
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

		public async Task<IActionResult> Login(string returnUrl)
		{
			var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();

			return View(new LoginViewModel
			{
				ReturnUrl = returnUrl,
				ExternalProviders = externalProviders
			});
		}
		
		public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
		{
			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Admin", new {returnUrl});
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

			return Challenge(properties, provider);
		}

		public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				return RedirectToAction("Login");
			}

			// Succeeded будет в том случае, когда логин на сайте будет сопоставлен с местным логином в БД
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false);
			if (result.Succeeded)
			{
				return RedirectToAction("Administrator");
			}

			return RedirectToAction("RegisterExternal", new ExternalLoginViewModel
			{
				ReturnUrl = returnUrl,
				Username = info.Principal.FindFirstValue(ClaimTypes.Name)
			});
		}

		public IActionResult RegisterExternal(ExternalLoginViewModel model)
		{
			return View(model);
		}

		[HttpPost]
		[ActionName("RegisterExternal")]
		public async Task<IActionResult> RegisterExternalConfirm(ExternalLoginViewModel model)
		{
			//проверим, что пользователь есть на фейсбуке
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				return RedirectToAction("Login");
			}

			var user = new ApplicationUser(model.Username);
			var result = await _userManager.CreateAsync(user);
			if (result.Succeeded)
			{
				// добавить клаймы после сохранения пользовтеля
				var claimAddingResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator"));
				if (claimAddingResult.Succeeded)
				{
					var identityResult = await _userManager.AddLoginAsync(user, info);
					if (identityResult.Succeeded)
					{
						await _signInManager.SignInAsync(user, false);
						return RedirectToAction("Administrator");
					}
				}
			}

			return View(model);
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

		public IEnumerable<AuthenticationScheme> ExternalProviders
		{
			get;
			set;
		}
	}

	public class ExternalLoginViewModel
	{
		public string Username
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
