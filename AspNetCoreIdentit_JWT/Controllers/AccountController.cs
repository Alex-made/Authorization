using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentit_JWT;
using AspNetCoreIdentit_JWT.Dto;
using AspNetCoreIdentit_JWT.Models;
using AspNetCoreIdentity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IJwtGenerator _jwtGenerator;

		public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJwtGenerator jwtGenerator)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_jwtGenerator = jwtGenerator;
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterUserViewModel registerUserViewModel)
		{
			var user = new IdentityUser
			{
				UserName = registerUserViewModel.Email,
				Email = registerUserViewModel.Email
			};
			var result = await _userManager.CreateAsync(user, registerUserViewModel.Password);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);
				return RedirectToAction("Index", "Home");
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}

				return View(registerUserViewModel);
			}
		}

		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			return View(new LoginUserViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<User> Login(LoginUserViewModel model)
		{
			User userWithToken = null;

			
				var user = await _userManager.FindByNameAsync(model.Email);
				
				var result =
					await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

				if (result.Succeeded)
				{
					
						userWithToken = new User
						{
							DisplayName = user.UserName,
							Token = _jwtGenerator.CreateToken(user),
							UserName = user.UserName,
							Image = null
						};
					
				}
				else
				{
					throw new Exception();
				}
			

			return userWithToken;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			// удаляем аутентификационные куки
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
