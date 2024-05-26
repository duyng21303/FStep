using FStep.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FStep
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<Fstep1Context>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("FStep"));
			});
			//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
		//.AddEntityFrameworkStores<Fstep1Context>();
		//.AddDefaultTokenProviders();

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromSeconds(10);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options
				=>
			{
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/AccessDenied";
			}).AddGoogle(googleOptions =>
			{
				// Đọc thông tin Authentication:Google từ appsettings.json
				IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

				// Thiết lập ClientID và ClientSecret để truy cập API google
				googleOptions.ClientId = googleAuthNSection["ClientId"];
				googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
				// Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
			});
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

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
