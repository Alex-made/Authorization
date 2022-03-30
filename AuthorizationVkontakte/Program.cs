using System;
using System.Security.Claims;
using AuthorizationVkontakte.Data;
using AuthorizationVkontakte.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthorizationVkontakte
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

	//������ ������������� ������������� ��
	internal class DatabaseInitializerOld
	{
		public static void Init(IServiceProvider scopeServiceProvider)
		{
			var context = scopeServiceProvider.GetService<ApplicationDbContext>();
			context.Users.Add(new ApplicationUser
			{
				UserName = "qwe",
				FirstName = "�������",
				LastName = "�������"
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
				FirstName = "�������",
				LastName = "�������"
			};
			var creationResult = userManager.CreateAsync(user, "qwe").GetAwaiter().GetResult();

			if (!creationResult.Succeeded)
			{
				throw new Exception("������ �������� ������������ � Identity");
			}

			userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
		}
	}
}
