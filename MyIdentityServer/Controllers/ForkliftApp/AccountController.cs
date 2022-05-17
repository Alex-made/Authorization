using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyIdentityServer.Dal;
using MyIdentityServer.Domain;
using MyIdentityServer.Dto;

namespace MyIdentityServer.Controllers.ForkliftApp
{
	[ApiController]
	[Route("api/[controller]")]
    public class AccountController : Controller
    {
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly ApplicationDbContext _dbContext;

		public AccountController(UserManager<IdentityUser> userManager, 
								 RoleManager<IdentityRole> roleManager, 
								 SignInManager<IdentityUser> signInManager,
								 ApplicationDbContext dbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_dbContext = dbContext;
		}

		//Дай мне новую пару токен/refresh-токен по логину/паролю. Уже есть пользователь new@new.ru 1qAZ
		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh(string refreshToken)
		{
			//провалидировать

			//получить токен из БД токенов по юзеру
			var oldRRefreshToken = _dbContext.RefreshTokens.Single(token => token.Token == refreshToken);

			//получить пользователя из БД
			var user = await _userManager.FindByIdAsync(oldRRefreshToken.UserId);

			if (user == null)
			{
				// если пользователя не найдено
				return null;
			}

			var roles = await _userManager.GetRolesAsync(user);


			var roleClaims = new List<Claim>();
			foreach (var role in roles)
			{
				roleClaims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
			};
			claims.AddRange(roleClaims);

			ClaimsIdentity claimsIdentity =
				new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
								   ClaimsIdentity.DefaultRoleClaimType);

			//удалить токен
			//создать новую пару токен/refresh-токен
			var now = DateTime.UtcNow;
			// создаем JWT-токен
			var jwt = new JwtSecurityToken(
				issuer: AuthOptions.ISSUER,
				audience: AuthOptions.AUDIENCE,
				notBefore: now,
				claims: claimsIdentity.Claims,
				expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
				signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			var newToken = new JwtSecurityTokenHandler().WriteToken(jwt);

			var refreshJwt = new JwtSecurityToken(AuthOptions.ISSUER, 
													 AuthOptions.AUDIENCE,
													 null, 
													 DateTime.UtcNow, 
													 now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
													 new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			var newRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

			var response = new
			{
				access_token = newToken,
				refresh_token = newRefreshToken,
				username = claimsIdentity.Name
			};

			return Json(response);
		}

		//Дай мне пару токен/refresh-токен по логину/паролю. Уже есть пользователь new@new.ru 1qAZ
		//Этот endpoint оставлю для api. Для регистрации/логина клиента сделаю новый со схожим функционалом.
		[HttpPost("token")]
        public async Task<IActionResult> Token(LoginUserRequest loginRequest)
        {
			//Принцип создания ClaimsIdentity здесь тот же, что и при аутентификации с помощью кук: создается набор объектов Claim,
			//которые включают различные данные о пользователе, например, логин, роль и т.д.
			var user = await _userManager.FindByNameAsync(loginRequest.Email);

			if (user == null)
			{
				// если пользователя не найдено
				return null;
			}

			var identity = await GetIdentity(user, loginRequest.Password);
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
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

			// создаем refresh-токен и сохраняем в БД
			var refreshJwt = new JwtSecurityToken(AuthOptions.ISSUER,
												  AuthOptions.AUDIENCE,
												  null,
												  DateTime.UtcNow,
												  now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
												  new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			var refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);
			await _dbContext.RefreshTokens.AddAsync(new RefreshToken(user.Id, refreshToken));
			await _dbContext.SaveChangesAsync();

			var response = new
			{
				access_token = token,
				refresh_token = refreshToken,
				username = identity.Name
			};

			return Json(response);
        }

		private async Task<ClaimsIdentity> GetIdentity(IdentityUser user, string password)
        {
			var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
			if (!signInResult.Succeeded)
			{
				throw new Exception($"Не удалось аутентифицировать пользователя {user.UserName}");
			}

            var roles = await _userManager.GetRolesAsync(user);
			

			var roleClaims = new List<Claim>();
			foreach (var role in roles)
			{
				roleClaims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
			};
			claims.AddRange(roleClaims);

			ClaimsIdentity claimsIdentity =
				new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
								   ClaimsIdentity.DefaultRoleClaimType);
			return claimsIdentity;

            
        }
    }
}