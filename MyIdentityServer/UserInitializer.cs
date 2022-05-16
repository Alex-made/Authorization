using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyIdentityServer
{
	public static class UserInitializer
	{
		//private readonly UserManager<IdentityUser> _userManager;
		//private readonly RoleManager<IdentityRole> _roleManager;

		//public UserInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		//{
		//	_userManager = userManager;
		//	_roleManager = roleManager;
		//}

		public static async Task InitializeAsync(UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
		{
			var roleName = "InitialRole";
			var userName = "InitialUser";

			// если пользователей у роли нет или нет самой роли, то вернется пустой список
			var adminUsers = await _userManager.GetUsersInRoleAsync("InitialRole");

			//тогда создадим нового пользователя с админской ролью (сейчас исп. InitialRole)
			if (!adminUsers.Any())
			{
				var adminUser = new IdentityUser(userName);

				var createUserResult = await _userManager.CreateAsync(adminUser, "StrongPassword");
				if (createUserResult.Succeeded)
				{
					var adminRole = new IdentityRole(roleName);

					var createRoleResult = await _roleManager.CreateAsync(adminRole);

					if (createRoleResult.Succeeded)
					{
						await _userManager.AddToRoleAsync(adminUser, roleName);
					}
				}
			}
		}
	}
}
