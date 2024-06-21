using FStep.Data;
using FStep.Repostory.Interface;
using FStep.Repostory.Service;
using FStep.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using FStep.Helpers;
using FStep.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using FStep;
using FStep.Hubs;

namespace FStep
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

		// Register DbContext
		builder.Services.AddDbContext<FstepDbContext>(options =>
		{
			options.UseSqlServer(builder.Configuration.GetConnectionString("FStep"));
		});

		// Add services to the container.
		builder.Services.AddControllersWithViews();
		builder.Services.AddDbContext<FstepDbContext>(option =>
		{
			option.UseSqlServer(builder.Configuration.GetConnectionString("FStep"));
		});
		builder.Services.AddTransient<IEmailSender, EmailSender>();
		builder.Services.AddHostedService<PostExpirationService>();

			builder.Services.AddSignalR();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(10);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			builder.Services.AddHttpContextAccessor();

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options
				=>
			{
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/AccessDenied";
			});


			// Configure Google authentication (if needed)
			builder.Services.AddAuthentication().AddGoogle(googleOptions =>
			{
				IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
				googleOptions.ClientId = googleAuthNSection["ClientId"];
				googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
				googleOptions.ClaimActions.MapJsonKey("UserID", "sub", "string");
				googleOptions.ClaimActions.MapJsonKey("IMG_RAW", "picture", "string");
				googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name", "givenName");
				googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", "string");
			});
			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

			builder.Services.AddDistributedMemoryCache();




			// Add AutoMapper (if needed)
			builder.Services.AddAutoMapper(typeof(Program));
			builder.Services.AddSingleton<IVnPayService, VnPayService>();
			var app = builder.Build();
			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseSession();


		app.UseAuthentication();
		app.UseAuthorization();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapHub<ChatHub>("chatHub");
		});
		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();

		}
	}

}