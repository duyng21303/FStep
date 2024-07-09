using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FStep.Repostory.Interface;
using FStep.Data; // Sử dụng entity framework core lưu trữ data.

namespace FStep.Repostory.Service
{
	public class PostExpirationService : IHostedService, IDisposable  //IHostedService: dv hoạt động chạy ngầm 
	{
		private readonly IServiceProvider _services;  // tạo scope để lấy dbContext
		private readonly IEmailSender _emailSender;  // sendMail
		private Timer _timer; //

		private readonly ILogger<PostExpirationService> _logger;

		public PostExpirationService(IServiceProvider services, IEmailSender emailSender, ILogger<PostExpirationService> logger)
		{
			_services = services;
			_emailSender = emailSender;
			_logger = logger;
		}


		public Task StartAsync(CancellationToken cancellationToken)
		{
			//DateTime now = DateTime.Now;
			//DateTime nextEightAM = now.Date.AddHours(8);

			//if (now >= nextEightAM)
			//{
			//	nextEightAM = nextEightAM.AddDays(1); // Move to 8:00 AM tomorrow
			//}
			//TimeSpan initialDueTime = nextEightAM - now;

			_timer = new Timer(CheckPostExpiration, null, TimeSpan.Zero, TimeSpan.FromDays(1)); // Kiểm tra nếu post hết hạn sau mỗi ngày
			_logger.LogInformation($"Found :{_timer}");
			return Task.CompletedTask;
			//triển khia từ ihostedservice được gọi ngay khi dịch vụ bắt đầu . _timer được khởi tạo để chạy hàm checkpost mỗi ngày 1 lần

		}

		private async void CheckPostExpiration(object state)
		{
			using (var scope = _services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<FstepDbContext>();
				// Lấy các bài post đã duyệt sắp hết hạn
				var approvedPosts = await dbContext.Posts
					.Include(p => p.IdUserNavigation)
					.Where(p => p.Status == "True" && p.Date.HasValue && EF.Functions.DateDiffDay(DateTime.Now, p.Date.Value.AddDays(30)) == 0)
					.ToListAsync();

				foreach (var post in approvedPosts)
				{
					if (post.IdUserNavigation != null && !string.IsNullOrEmpty(post.IdUserNavigation.Email))
					{
						try
						{
							post.Status = "False";
							string subject = $"Your post has expired";
							string body = $@"Hello {post.IdUserNavigation.Name},<br/><br/>
                                    Your post: '{post.Content}' with code: {post.IdPost} has expired. Please create a new post or you can pay to have us review again '{post.Content}' for you.<br/>
                                     Information: ABC Bank - Account: 123456789 (fee 5,000 VND) <br/>
                                     <span style='color: red;'>
                                      Transfer content includes: <br/>
                                      Your name:........ <br/>
                                     IdPost:........... <br/>
                                     NamePost:.......... <br/>
                                    (idPost and namePost are the posts you want to review. We sent it via email)
                                     </span> <br/>
                                     Sincerely,<br/>
                                     Admin Team";
							await dbContext.SaveChangesAsync();
							await _emailSender.EmailSendAsync(post.IdUserNavigation.Email, subject, body);
							_logger.LogInformation($"The email has been sent successfully to {post.IdUserNavigation.Email}");
						}
						catch (Exception ex)
						{
							_logger.LogError($"Error processing post ID {post.IdProduct}: {ex.Message}");
							throw;
						}
					}
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}

// checklog
//if (approvedPosts.Any())
//{
//	_logger.LogInformation($"Found : {approvedPosts.Count} approvedPosts:");
//	foreach (var post in approvedPosts)
//	{
//		_logger.LogInformation($"- Post ID: {post.IdProduct}, Title: {post.Content}");
//		// Thêm thông tin khác
//	}
//}
//else
//{
//	_logger.LogInformation("Không có bài viết nào thỏa điều kiện chờ duyệt.");
//}
