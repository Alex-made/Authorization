using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationVkontakte.Data;
using AuthorizationVkontakte.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthorizationVkontakte
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
						config.AppId = "с сайта developers.facebook.com";
						config.AppSecret = "с сайта developers.facebook.com";
					})
					.AddOAuth("VK", "Vkontakte", //тут ВК авторизация добавляется таким образом, чтобы сохранить и авторизацию FB. Т.к метод расширения AddVkontakte не работает после мтерда AddFacebook
												 // как настроить авторизацию ВК описано тут: https://vk.com/dev/openapi https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/other-logins?view=aspnetcore-5.0
							  config =>
							  {
								  config.ClientId = "8119776";
								  config.ClientSecret = "rjh8nRg3ca4C9LtwFRJB";
								  config.ClaimsIssuer = "VKontakte";
								  config.CallbackPath = new PathString("/signin-vkontakte-token");
								  config.AuthorizationEndpoint = "https://oauth.vk.com/authorize";
								  config.TokenEndpoint = "https://oauth.vk.com/access_token";
								  config.Scope.Add("email");
								  config.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
								  config.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
								  config.SaveTokens = true;
								  config.Events = new OAuthEvents
								  {
									  OnRemoteFailure = OnFailure
								  };
							  }); 

			//автоизация без использвоания Identity. В Identity уже это подключено
			//services.AddAuthentication("Cookie").AddCookie("Cookie",
			//											   config =>
			//											   {
			//												   config.LoginPath = "/admin/login";
			//												   config.AccessDeniedPath = "/Home/AccessDenied";
			//											   });
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
									  builder.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "Manager") ||
																	x.User.HasClaim(ClaimTypes.Role, "Administrator"));
								  });
			});
			services.AddControllersWithViews();
		}

		private Task OnFailure(RemoteFailureContext arg)
		{
			throw new System.NotImplementedException();
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
