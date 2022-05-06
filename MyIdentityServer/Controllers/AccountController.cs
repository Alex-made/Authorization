using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyIdentityServer.Models;

// класс Person

namespace MyIdentityServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
    public class AccountController : Controller
    {
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

        // тестовые данные вместо использования базы данных
        private List<Person> people = new List<Person>
        {
            new Person {Login="admin@gmail.com", Password="12345", Role = "admin" },
            new Person { Login="qwerty@gmail.com", Password="55555", Role = "user" }
        };

        //Дай мне токен по логину/паролю
        [HttpPost("token")]
        public async Task<IActionResult> Token(string username, string password)
        {
			//Принцип создания ClaimsIdentity здесь тот же, что и при аутентификации с помощью кук: создается набор объектов Claim,
            //которые включают различные данные о пользователе, например, логин, роль и т.д.
            var identity = await GetIdentity(username);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string username)
        {
			var person = await _userManager.FindByNameAsync(username);
			var roles = await _userManager.GetRolesAsync(person);
			if (person != null)
			{
				var roleClaims = new List<Claim>();
				foreach (var role in roles)
				{
					roleClaims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
				}

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.UserName),
				};
                claims.AddRange(roleClaims);

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}