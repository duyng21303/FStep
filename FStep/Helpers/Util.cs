using FStep.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;

namespace FStep.Helpers
{
	public class Util
	{
		public static ClaimsPrincipal ClaimsHelper(User user)
		{
			var claims = new List<Claim>
			{
					new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
					new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
					new Claim("UserID", user.IdUser ?? string.Empty),
					new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
					new Claim("IMG",  user.AvatarImg != null ? "userAvar/" + user.AvatarImg : "nullAvar/User-avatar.svg.png")
			};
			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			return claimsPrincipal;
		}
		public static string UpLoadImg(IFormFile img, string folder)
		{
			try
			{
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folder, img.FileName);
				using (var myfile = new FileStream(fullPath, FileMode.CreateNew ))
				{
					img.CopyTo(myfile);
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
			return img.FileName;
		}
		public static string GenerateRandomKey(int length = 5)
		{
			var pattern = @"ksfjsdkfjhskfnskdfnskdfskvbkxcjvnkcvnosfoxcvnxcivnkjnLSKDLKNGLKFNVLCXNVKCBKJDNGDKOLJVNXCLJVNXLCVN!LSKDFX";
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}
			return sb.ToString();
		}
	}
}
