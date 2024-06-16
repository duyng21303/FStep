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
			_timer = new Timer(CheckPostExpiration, null, TimeSpan.Zero, TimeSpan.FromDays(1)); // Kiểm tra nếu post hết hạn sau mỗi ngày
			return Task.CompletedTask;
			//triển khia từ ihostedservice được gọi ngay khi dịch vụ bắt đầu . _timer được khởi tạo để chạy hàm checkpost mỗi ngày 1 lần
		}

		private async void CheckPostExpiration(object state)
		{
			using (var scope = _services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<FstepDbContext>();

				// Lấy các bài post sắp hết hạn
				var pendingPosts = await dbContext.Posts
					.Include(p => p.IdUserNavigation)
					.Where(p => p.Status == "false" && p.Date.HasValue && EF.Functions.DateDiffDay(DateTime.Now, p.Date.Value.AddDays(7)) == 2)
					.ToListAsync();

				if (pendingPosts.Any())
				{
					_logger.LogInformation($"Found : {pendingPosts.Count} pendingpost:");
					foreach (var post in pendingPosts)
					{
						_logger.LogInformation($"- Post ID: {post.IdProduct}, Title: {post.Content}, email: {post.IdUserNavigation.Email}");
						// Thêm thông tin khác nếu cần
					}
				}
				else
				{
					_logger.LogInformation("Không có bài viết nào thỏa điều kiện chờ duyệt.");
				}
				foreach (var post in pendingPosts)
				{
					if (post.IdUserNavigation != null && !string.IsNullOrEmpty(post.IdUserNavigation.Email))
					{
						try
						{
							string subject = $"Bài đăng <span style=\"color:red;\">(post.Content)</span> sắp hết hạn";
							string body = $@"Xin chào {post.IdUserNavigation.Name},<br/><br/>
                                             Bài đăng '(post.Content)' - ngày tạo '{post.Date}' của bạn sẽ hết hạn trong 2 ngày. 
                                             Vui lòng mang sản phẩm đến phòng P. 123 để chúng tôi kiểm tra. 
                                             Các bài đăng quá hạn sẽ bị xóa khỏi hệ thống.<br/><br/>
                                             Trân trọng,<br/>
                                             Nhóm Admin";
							// Gửi email
							await _emailSender.EmailSendAsync(post.IdUserNavigation.Email, subject, body);
							_logger.LogInformation($"The email has been sent successfully  {post.IdUserNavigation.Email}");
						}
						catch (Exception ex)
						{
							_logger.LogError($"Error sending email to {post.IdUserNavigation.Email}: {ex.Message}");
							throw; // Ném lỗi để xử lý ở nơi gọi
						}
					}
				}

				// Lấy các bài post đã duyệt sắp hết hạn
				var approvedPosts = await dbContext.Posts
					.Where(p => p.Status == "true" && p.Date.HasValue && EF.Functions.DateDiffDay(DateTime.Now, p.Date.Value.AddDays(30)) == 2)
					.ToListAsync();

				if (pendingPosts.Any())
				{
					_logger.LogInformation($"Found : {approvedPosts.Count} approvedPosts:");
					foreach (var post in approvedPosts)
					{
						_logger.LogInformation($"- Post ID: {post.IdProduct}, Title: {post.Content}");
						// Thêm thông tin khác nếu cần
					}
				}
				else
				{
					_logger.LogInformation("Không có bài viết nào thỏa điều kiện chờ duyệt.");
				}

				foreach (var post in approvedPosts)
				{
					if (post.IdUserNavigation != null && !string.IsNullOrEmpty(post.IdUserNavigation.Email))
					{
						await _emailSender.EmailSendAsync(post.IdUserNavigation.Email, $"Bài đăng {post.Content} sắp hết hạn",
						$"Hello,<br/><br/>Your post titled '{post.Content}' will expire in 2 days. If you want to continue using it, please pay.<br/ ><br/>Sincerely,<br/>Admin Team");
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
