using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;
using DigiKala.Core.Classes;

using DigiKala.DataAccessLayer.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Parbad.Builder;
using Parbad.Storage.EntityFrameworkCore.Builder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.ParbadVirtual;

namespace DigiKala
{
	public class Startup
	{
		public IConfiguration Configuration { get; set; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(options =>
		   {
			   options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			   options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			   options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		   }).AddCookie(options =>
		   {
			   options.LoginPath = "/Accounts/Login";
			   options.LogoutPath = "/Accounts/LogOut";
			   options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
		   });

			services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddTransient<IUser, UserService>();
			services.AddTransient<IAccount, AccountService>();
			services.AddTransient<IAdmin, AdminService>();
			services.AddTransient<IStore, StoreService>();
			services.AddTransient<ITemp, TempService>();
			services.AddTransient<IViewRenderService, RenderToString>();

			services.AddAutoMapper(opt => { opt.AddMaps(typeof(AutoMapperProfile).Assembly); });
			services.AddScoped<PanelLayoutScope>();
			services.AddScoped<TemplateScope>();
			services.AddScoped<MessageSender>();
			services.AddSession(option => { option.IdleTimeout = TimeSpan.FromDays(7); option.Cookie.HttpOnly = true; option.Cookie.IsEssential = true; });

			services.AddControllersWithViews().AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
			services.AddParbad()
				.ConfigureGateways(gateways =>
				{
					gateways.AddMellat().WithAccounts(source => source.Add<GatewaysAccounts>(ServiceLifetime.Transient));
					gateways.AddParbadVirtual().WithOptions(options => options.GatewayPath = "/MyVirtualGateway");
				})
				.ConfigureHttpContext(builder => builder.UseDefaultAspNetCore())
				.ConfigureStorage(builder =>
				{
					builder.UseEfCore(option =>
					{
						option.ConfigureDbContext = db => db.UseSqlServer(
							Configuration.GetConnectionString("DefaultConnection"),
							sql =>
							{
								sql.MigrationsAssembly("DigiKala.DataAccessLayer");
								sql.MigrationsHistoryTable("__ParbadEFMigrationHistory");
							}
						);
					});
				})
				.ConfigureAutoIncrementTrackingNumber(option => { option.MinimumValue = 1000;option.Increment = 1; });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseHsts();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseSession();

			app.UseAuthorization();
			app.UseAuthentication();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
			app.UseParbadVirtualGateway();
		}
	}
}
