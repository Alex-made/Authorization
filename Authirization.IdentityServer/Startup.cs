using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Authirization.IdentityServer
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
			services.AddHttpsRedirection(options =>
			{
				//options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
				options.HttpsPort = 10001;
			});
			services.AddHttpClient();
			services.AddControllersWithViews();
			services.AddIdentityServer()
					.AddInMemoryClients(IdentityServer.Configuration.GetClients())
					.AddInMemoryApiResources(IdentityServer.Configuration.GetApiResources())
					.AddInMemoryIdentityResources(IdentityServer.Configuration.GetIdentityResources())
					.AddInMemoryApiScopes(IdentityServer.Configuration.GetScopes())
					.AddDeveloperSigningCredential();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseIdentityServer();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
