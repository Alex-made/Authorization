using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyIdentityServer.Dal;

namespace MyIdentityServer
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
			services.AddDbContext<ApplicationDbContext>(options =>
															options.UseSqlServer(
																Configuration.GetConnectionString("DefaultConnection")));

			services.AddIdentity<IdentityUser, IdentityRole>(options =>
					{
						options.SignIn.RequireConfirmedAccount = false;   //����� �� ������ ��� ���������������� ������
						options.Password.RequiredLength = 3;   // ����������� �����
						options.Password.RequireNonAlphanumeric = false;   // ��������� �� �� ���������-�������� �������
						options.Password.RequireLowercase = false; // ��������� �� ������� � ������ ��������
						options.Password.RequireUppercase = false; // ��������� �� ������� � ������� ��������
						options.Password.RequireDigit = false; // ��������� �� �����
					})
					.AddEntityFrameworkStores<ApplicationDbContext>();

			//��� ����������� ���������� ��� ����������� ��� ��������� � ��������� ��������� ������� ����������� (������ �� ����� � ������ ������)
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.RequireHttpsMetadata = false;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							// ��������, ����� �� �������������� �������� ��� ��������� ������
							ValidateIssuer = true,
							// ������, �������������� ��������
							ValidIssuer = AuthOptions.ISSUER,

							// ����� �� �������������� ����������� ������
							ValidateAudience = true,
							// ��������� ����������� ������
							ValidAudience = AuthOptions.AUDIENCE,
							// ����� �� �������������� ����� �������������
							ValidateLifetime = true,

							// ��������� ����� ������������
							IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
							// ��������� ����� ������������
							ValidateIssuerSigningKey = true,
						};
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
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
