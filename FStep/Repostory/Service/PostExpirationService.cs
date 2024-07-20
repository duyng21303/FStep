using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FStep.Repostory.Interface;
using FStep.Data;
using NuGet.Protocol.Plugins; // Sử dụng entity framework core lưu trữ data.

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
				var dbContext = scope.ServiceProvider.GetRequiredService<FstepDBContext>();
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
							dbContext.Update(post);
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
				// Các transaction exchange bị quá hạn 3 ngày (1 trong 2 không đem hàng tới, huỷ tự động)
				var exchange = await dbContext.Transactions.Where(p => p.Type == "Exchange" && p.Status == "Processing" && p.Date.Value.AddDays(3).CompareTo(DateTime.Now) < 0 && (p.SentImg == null || p.SentBuyerImg == null))?.ToListAsync();
				foreach (var x in exchange)
				{
					//update status transaction, post, product, payment
					x.Status = "Canceled";
					dbContext.Update(x);

					var seller = await dbContext.Users.SingleOrDefaultAsync(p => p.IdUser == x.IdUserSeller);
					var buyer = await dbContext.Users.SingleOrDefaultAsync(p => p.IdUser == x.IdUserBuyer);

					var post = dbContext.Posts.SingleOrDefault(p => p.IdPost == x.IdPost);
					post.Status = "True";
					dbContext.Update(post);

					var product = dbContext.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct);
					product.Status = "True";
					dbContext.Update(product);

					var payment = dbContext.Payments.FirstOrDefault(p => p.IdTransaction == x.IdTransaction);
					payment.Status = "False";
					payment.CancelDate = DateTime.Now;
					dbContext.Update(payment);

					//Update point rating user
					if (x.SentImg == null)
					{
						if (x.SentBuyerImg == null)
						{
							seller.PointRating -= 15;
							buyer.PointRating -= 15;
						}
						else
						{
							seller.PointRating -= 15;
						}
					}
					else
					{
						buyer.PointRating -= 15;
					}
					//send email inform for user
					string body = $"Đơn hàng mới mã số <span style=\"color:red\"> {x.CodeTransaction} </span> đã bị huỷ tự động vì quá hạn giao hàng, vui lòng tới nhận lại hàng của bạn!!!";
					await _emailSender.EmailSendAsync(seller.Email, "Đơn hàng bị huỷ tự động vì quá hạn", body);
					await _emailSender.EmailSendAsync(buyer.Email, "Đơn hàng bị huỷ tự động vì quá hạn", body);

					//save change db
					await dbContext.SaveChangesAsync();
				}
				// Các transaction sale bị quá hạn 3 ngày (người bán không đem hàng tới, huỷ tự động)
				var sale = await dbContext.Transactions.Where(p => p.Type == "Sale" && p.Status == "Processing" && p.Date.Value.AddDays(3).CompareTo(DateTime.Now) < 0 && p.SentImg == null)?.ToListAsync();
				foreach (var x in sale)
				{
					//update status transaction, post, product, payment
					x.Status = "Canceled";
					dbContext.Update(x);

					var seller = await dbContext.Users.SingleOrDefaultAsync(p => p.IdUser == x.IdUserSeller);
					var buyer = await dbContext.Users.SingleOrDefaultAsync(p => p.IdUser == x.IdUserBuyer);

					var post = dbContext.Posts.SingleOrDefault(p => p.IdPost == x.IdPost);
					post.Status = "True";
					dbContext.Update(post);

					var product = dbContext.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct);
					product.Status = "True";
					dbContext.Update(product);

					var payment = dbContext.Payments.FirstOrDefault(p => p.IdTransaction == x.IdTransaction);
					payment.Status = "False";
					payment.CancelDate = DateTime.Now;
					dbContext.Update(payment);

					//Update point rating user
					seller.PointRating -= 15;

					// refund cho người mua
					//
					// renfund end

					//send email inform for user
					await _emailSender.EmailSendAsync(seller.Email, "Đơn hàng bị huỷ tự động vì quá hạn", $"Đơn hàng mới mã số <span style=\"color:red\"> {x.CodeTransaction} </span> đã bị huỷ tự động vì quá hạn giao hàng. <span style=\"color:red\">Bạn sẽ bị trừ điểm uy tín</span>, xin hãy lưu ý!!!");
					await _emailSender.EmailSendAsync(buyer.Email, "Đơn hàng bị huỷ tự động vì quá hạn", $"Đơn hàng mới mã số <span style=\"color:red\"> {x.CodeTransaction} </span> đã bị huỷ tự động vì quá hạn giao hàng, tiền của bạn sẽ được hoàn lại trong thời gian sớm nhất!!!");

					//save change db
					await dbContext.SaveChangesAsync();
				}
				// Tất cả Các transaction processing bị quá hạn 7 ngày (hoàn thành giao dịch)
				var transaction = await dbContext.Transactions.Where(p => p.Status == "Processing" && p.Date.Value.AddDays(7).CompareTo(DateTime.Now) < 0)?.ToListAsync();
				foreach (var x in transaction)
				{
					//update status transaction, post, product, payment
					x.Status = "Completed";
					dbContext.Update(x);

					var seller = await dbContext.Users.SingleOrDefaultAsync(p => p.IdUser == x.IdUserSeller);
					var buyer = await dbContext.Users.SingleOrDefaultAsync(p => p.IdUser == x.IdUserBuyer);

					var post = dbContext.Posts.SingleOrDefault(p => p.IdPost == x.IdPost);
					post.Status = "False";
					dbContext.Update(post);

					var product = dbContext.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct);
					product.Status = "False";
					dbContext.Update(product);

					var payment = new Payment();
					payment.Status = "True";
					payment.PayTime = DateTime.Now;
					payment.IdTransaction = x.IdTransaction;
					payment.Amount = x.Amount ?? null;
					payment.Type = "Seller";
					dbContext.Add(payment);

					//Chuyển tiền cho người bán
					//
					//Pay end

					//send email inform for user
					string body = $"Đơn hàng mới mã số <span style=\"color:green\"> {x.CodeTransaction} </span> đã được giao thành công, vui lòng kiểm tra đơn hàng trên lịch sử giao dịch của bạn!!!";
					await _emailSender.EmailSendAsync(seller.Email, "Đơn hàng của bạn đã được giao thành công", body);
					await _emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được giao thành công", body);
					dbContext.Update(x);

					//save change db
					await dbContext.SaveChangesAsync();
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
