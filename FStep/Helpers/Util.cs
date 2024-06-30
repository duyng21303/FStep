using FStep.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Mono.TextTemplating;
using NuGet.DependencyResolver;
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
				if (img != null)
				{
					var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folder, img.FileName);
					bool isExisted = Path.Exists(fullPath);
					if (isExisted)
					{
						string strDate = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss");
						string fileExtension = Path.GetExtension(img.FileName).Replace(".", "");
						string fileName = img.FileName.Substring(img.FileName.LastIndexOf("\\\\") + 1);
						fileName = fileName.Substring(0, fileName.LastIndexOf(fileExtension)) + strDate + "." + fileExtension;

						fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folder, fileName);

						using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
						{
							img.CopyTo(myfile);
						}
						return fileName;
					}
					else
					{
						using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
						{
							img.CopyTo(myfile);
						}
					}
				}
				else
				{
					return "";
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
			return img.FileName;
		}
		public static string ConvertImgUser(User user)
		{
			var img = "";
			if (user.AvatarImg != null)
			{
				img = "userAvar/" + user.AvatarImg;
			}
			else
			{
				img = "nullAvar/149071.png";
			}
			return img;
		}
		public static string GenerateRandomKey(int length)
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
		public async static Task<string> DownloadImgGoogle(string imgURL, string userID, string downloadDirectory)
		{ // Ensure the directory exists
			if (!Directory.Exists(downloadDirectory))
			{
				Directory.CreateDirectory(downloadDirectory);
			}

			// Define the image filename based on the userID and the directory path
			string imgFilename = $"{userID}.jpg";

			using HttpClient client = new HttpClient();
			try
			{
				// Send a GET request to the image URL
				HttpResponseMessage response = await client.GetAsync(imgURL);
				// Ensure the request was successful
				response.EnsureSuccessStatusCode();

				// Read the image bytes from the response
				byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
				// Save the image bytes to a local file
				string filePath = Path.Combine(downloadDirectory, imgFilename);
				await File.WriteAllBytesAsync(filePath, imageBytes);

				// Return the filename of the saved image
				return imgFilename;
			}
			catch (Exception ex)
			{
				// Log the exception (could also rethrow or handle accordingly)
				Console.WriteLine($"Failed to download image: {ex.Message}");
				return null; // Return null or throw an exception as needed
			}
		}
	}
}
