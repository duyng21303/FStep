using FStep.Data;
using FStep.Repostory.Interface;
using FStep.Repostory.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

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

		// Register EmailSender service
		builder.Services.AddTransient<IEmailSender, EmailSender>();
		//builder.Services.AddHostedService<PostExpirationService>();
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

		// Add AutoMapper (if needed)
		builder.Services.AddAutoMapper(typeof(Program));

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
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