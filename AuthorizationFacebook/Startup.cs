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
using AuthorizationFacebook.Data;
using AuthorizationFacebook.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationFacebook
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
			services.AddDbContext<ApplicationDbContext>(config =>
			{
				config.UseInMemoryDatabase("DB");
			}).AddIdentity<ApplicationUser, ApplicationRole>(config =>
			{
				config.Password.RequireUppercase = false;
				config.Password.RequireDigit = false;
				config.Password.RequireLowercase = false;
				config.Password.RequireNonAlphanumeric = false;
				config.Password.RequiredLength = 3;
			})
					.AddEntityFrameworkStores<ApplicationDbContext>();

			services.ConfigureApplicationCookie(config =>
			{
				config.LoginPath = "/admin/login";
				config.AccessDeniedPath = "/Home/AccessDenied";
			});

			services.AddAuthentication()
					.AddFacebook(config =>
					{
						config.AppId = "? ????? developers.facebook.com";
						config.AppSecret = "? ????? developers.facebook.com";
					});

			//?????????? ??? ????????????? Identity. ? Identity ??? ??? ??????????
			//services.AddAuthentication("Cookie").AddCookie("Cookie",
			//											   config =>
			//											   {
			//												   config.LoginPath = "/admin/login";
			//												   config.AccessDeniedPath = "/Home/AccessDenied";
			//											   });
			//??????????? ?? ?????? ???????. ? ?????? ????? ?????? ?????? ?????, ? ? ????????? ????? ? ????????
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
									  builder.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "Manager") ||
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
