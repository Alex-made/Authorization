using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AuthorizationJwtBearer.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Secret()
		{
			return View();
		}

		public IActionResult Authenticate()
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, "Alex"),
				new (JwtRegisteredClaimNames.Email, "mail")
			};

			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret_key_for_jwt"));

			var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken("http://localhost", "http://localhost", claims, DateTime.Now, DateTime.Now.AddDays(1), signingCredentials);

			var value = new JwtSecurityTokenHandler().WriteToken(token);

			ViewBag.Token = value;
			return View();
		}
	}
}
