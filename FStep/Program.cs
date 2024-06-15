using FStep.Data;
<<<<<<< HEAD
using FStep.Repostory.Interface;
using FStep.Repostory.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
=======
using FStep.Helpers;
using FStep.Repostory.Interface;
using FStep.Repostory.Service;
using FStep.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
>>>>>>> develop
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

public class Program
{
<<<<<<< HEAD
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

		// Register EmailSender service
		builder.Services.AddTransient<IEmailSender, EmailSender>();

		// Add session support
		builder.Services.AddSession(options =>
		{
			options.IdleTimeout = TimeSpan.FromSeconds(10);
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});

		// Add authentication
		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
=======
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<FstepDbContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("FStep"));
			});
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            //builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            //.AddEntityFrameworkStores<Fstep1Context>();
            //.AddDefaultTokenProviders();


			//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
			//.AddEntityFrameworkStores<Fstep1Context>();
			//.AddDefaultTokenProviders();

			builder.Services.AddSignalR();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(10);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			
			
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options
				=>
>>>>>>> develop
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

<<<<<<< HEAD
		// Add AutoMapper (if needed)
		builder.Services.AddAutoMapper(typeof(Program));
=======
			builder.Services.AddSingleton<IVnPayService, VnPayService>();
>>>>>>> develop

		var app = builder.Build();

<<<<<<< HEAD
		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
=======
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
			
>>>>>>> develop
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseSession();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		app.Run();
	}
}