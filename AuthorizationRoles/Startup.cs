using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationRoles
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication("Cookie").AddCookie("Cookie",
														   config =>
														   {
															   config.LoginPath = "/admin/login";
															   config.AccessDeniedPath = "/Home/AccessDenied";
														   });
			//Авторизация на основе клаймов. К админу имеет доступ только админ, а к менеджеру админ и менеджер
			services.AddAuthorization(options =>
			{
				options.AddPolicy("Administrator",
								  builder =>
								  {
									  builder.RequireClaim(ClaimTypes.Role, "Administrator");
								  });
				options.AddPolicy("Manager",
								  builder =>
								  {
									  builder.RequireAssertion(x=>x.User.HasClaim(ClaimTypes.Role, "Manager") ||
																  x.User.HasClaim(ClaimTypes.Role, "Administrator"));
								  });
			});
			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
