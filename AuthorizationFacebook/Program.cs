using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationFacebook.Data;
using AuthorizationFacebook.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationFacebook
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args)
				.Build();
			using (var scope = host.Services.CreateScope())
			{
				DatabaseInitializer.Init(scope.ServiceProvider);
			}
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}

	//старый инициализатор пользователей БД
	internal class DatabaseInitializerOld
	{
		public static void Init(IServiceProvider scopeServiceProvider)
		{
			var context = scopeServiceProvider.GetService<ApplicationDbContext>();
			context.Users.Add(new ApplicationUser
			{
				UserName = "qwe",
				FirstName = "Алексей",
				LastName = "Смирнов"
			});
			context.SaveChanges();
		}
	}

	internal class DatabaseInitializer
	{
		public static void Init(IServiceProvider scopeServiceProvider)
		{
			var userManager = scopeServiceProvider.GetService<UserManager<ApplicationUser>>();
			var user = new ApplicationUser
			{
				UserName = "qwe",
				FirstName = "Алексей",
				LastName = "Смирнов"
			};
			var creationResult = userManager.CreateAsync(user, "qwe").GetAwaiter().GetResult();

			if (!creationResult.Succeeded)
			{
				throw new Exception("Ошибка создания пользователя в Identity");
			}

			userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
		}
	}
}
