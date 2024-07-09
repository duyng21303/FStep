using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.Models;
using FStep.Services;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FStep.Controllers.Admin
{
	public class AdminController : Controller
	{
		private readonly FstepDbContext _context;
		private readonly IMapper _mapper;
		private static readonly string[] defaultRole = new[] { "Customer", "Moderator", "Administrator" };
		private readonly IConfiguration _configuration;
		private readonly NotificationServices notificationServices;

		public AdminController(FstepDbContext context, IMapper mapper, IConfiguration configuration)
		{
			_context = context;
			_mapper = mapper;
			_configuration = configuration;
			notificationServices = new NotificationServices(_context);
		}
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index(string codeTransaction, int? page)
		{
			var totalPost = _context.Posts.Count(p => p.Status != "Rejected");
			var totalUser = _context.Users.Count(u => u.Status != false);
			var totalTransaction = _context.Transactions.Count(t => t.Status == "Completed" || t.Status == "Processing" || t.Status == "Canceled");
			var totalGMV = _context.Transactions
				.Where(t => t.Status == "Completed")
				.Sum(t => t.Amount);
			IQueryable<Transaction> query = _context.Transactions;
			if (!string.IsNullOrEmpty(codeTransaction))
			{
				query = query.Where(t => t.CodeTransaction.Contains(codeTransaction));
			}

			var transactions = query.OrderByDescending(t => t.Date).ToList();
			List<TransactionVM> transactionVMs = new List<TransactionVM>();
			foreach (var item in transactions)
			{
				transactionVMs.Add(new TransactionVM()
				{
					Transaction = item
				});
			}

			// Calculate revenues and assign to each transaction
			foreach (var transaction in transactionVMs)
			{
				transaction.Revenues = CalculateDiscount(transaction.Transaction.Amount);
			}

			float totalRevenues = 0;
			foreach (var transaction in transactionVMs)
			{
				if (transaction.Transaction.Status == "Completed")
				{
					totalRevenues += transaction.Revenues;
				}
			}

			// Paginate transactions
			int pageSize = 20;
			int pageNumber = (page ?? 1);
			var pagedTransactions = transactionVMs.ToPagedList(pageNumber, pageSize);

			var transactionVM = new TransactionVM
			{
				PagedTransactions = pagedTransactions
			};
			//----------------------------------------------------------------------------------------------------Dasboard
			var sevenMonthsAgo = DateTime.Now.AddMonths(-6);

			// Tạo danh sách các tháng trong 7 tháng gần nhất
			var months = Enumerable.Range(0, 5)
					.Select(i => DateTime.Now.AddMonths(-i))
					.OrderBy(d => d.Year).ThenBy(d => d.Month)
					.ToList();


			// Phân loại giao dịch theo tháng
			var resultListTotalPost = new List<int>();
			var resultListTotal = new List<int>();
			var resultListTotalCompleted = new List<int>();

			var resultListPostCountExchange = new List<int>();
			var resultListPostCountSale = new List<int>();
			var amount = _configuration.GetValue<float>("Amount");
			foreach (var month in months)
			{
				// Kiểm tra xem tháng hiện tại có giao dịch hay không
				var totalPostCount = await _context.Posts
							.Where(o => o.Date != null && o.Date.Value.Month == month.Month && o.Date.Value.Year == month.Year)
							.CountAsync();

				var totalTransactionCount = await _context.Transactions
							.Where(o => o.Date != null && o.Date.Value.Month == month.Month && o.Date.Value.Year == month.Year)
							.CountAsync();

				var totalCompletedTransactionCount = await _context.Transactions
							.Where(o => o.Date != null && o.Date.Value.Month == month.Month && o.Date.Value.Year == month.Year && o.Status == "Completed")
							.CountAsync();
				var totalPostCountExchange = await _context.Posts
							.Where(o => o.Date != null && o.Date.Value.Month == month.Month && o.Date.Value.Year == month.Year && o.Type == "Exchange" && o.Status == "Completed")
							.CountAsync();
				var totalPostCountSale = await _context.Posts
							.Where(o => o.Date != null && o.Date.Value.Month == month.Month && o.Date.Value.Year == month.Year && o.Type == "Sale" && o.Status == "Completed")
							.CountAsync();
				// Thêm số lượng giao dịch hoặc giá trị 0 vào danh sách kết quả
				resultListTotal.Add(totalTransactionCount);
				resultListTotalCompleted.Add(totalCompletedTransactionCount);
				resultListTotalPost.Add(totalPostCount);

				resultListPostCountExchange.Add((int)(totalPostCountSale * amount));
				resultListPostCountSale.Add((int)(totalPostCountSale * amount));
			}
			// Chuẩn bị dữ liệu cho biểu đồ
			var labels = months.Select(g => "Tháng " + g.Month).ToList();
			ViewBag.Labels = labels;
			ViewBag.TotalTransactionDash = resultListTotal;
			ViewBag.TotalPostDash = resultListTotalPost;
			ViewBag.TotalCompleted = resultListTotalCompleted;
			ViewBag.TotalPostExchange = resultListPostCountExchange;
			ViewBag.TotalPostSale = resultListPostCountSale;
			//----------------------------------------------------------------------------------------------------Dasboard

			ViewBag.TotalPost = totalPost;
			ViewBag.TotalTransaction = totalTransaction;
			ViewBag.TotalUser = totalUser;
			ViewBag.TotalGMV = totalGMV;
			ViewBag.TotalRevenues = totalRevenues;
			ViewBag.CodeTransaction = codeTransaction;
			return View(transactionVM);
		}

		public float CalculateDiscount(float? amount)
		{
			if (amount.HasValue && amount > 0)
			{

				return _configuration.GetValue<float>("Amount");
				// 10% discount
			}
			else
			{
				return _configuration.GetValue<float>("Amount"); // Default discount if amount is null or <= 0
			}
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult ReferTransaction(int id)
		{
			var transaction = _context.Transactions.FirstOrDefault(t => t.IdTransaction == id);
			if (transaction != null)
			{
				transaction.Status = "Processing";
				_context.Transactions.Update(transaction);
				_context.SaveChanges();
				TempData["SuccessMessage"] = $"Giao dịch đã được hoàn trả thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Giao dịch hoàn trả thất bại.";
			}
			return RedirectToAction("Index", "Admin");
		}
		[Authorize(Roles = "Admin, Moderator")]
		public IActionResult UserManager(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _context.Users.AsQueryable();
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Email.ToLower().Contains(search.ToLower()));
			}

			var users = query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => _mapper.Map<ProfileVM>(x)).ToList();
			PagingModel<ProfileVM> pagingModel = new()
			{
				Items = users,
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = pageSize,
					TotalItems = query.Count(),
					Search = search
				}
			};

			return View("UserManager", pagingModel);
		}
		[Authorize(Roles = "Admin, Moderator")]
		public IActionResult UserDetail(string id)
		{
			// Lấy thông tin người dùng
			var user = _context.Users.FirstOrDefault(user => user.IdUser == id);

			if (user != null)
			{
				// Đếm số bài đăng của người dùng có type là Sale
				var saleCount = _context.Posts.Count(post => post.IdUser == id && post.Type == "Sale");

				// Đếm số bài đăng của người dùng có type là Exchange
				var exchangeCount = _context.Posts.Count(post => post.IdUser == id && post.Type == "Exchange");

				var totalPost = saleCount + exchangeCount;

				var totalTransaction = _context.Transactions.Count(t => t.IdUserBuyer == id && t.Status != "Processing");
				// Map thông tin người dùng sang ProfileVM
				var profileVM = _mapper.Map<ProfileVM>(user);

				// Truyền số bài đăng vào ViewModel hoặc ViewData
				ViewBag.TotalPost = totalPost;
				ViewBag.TotalTransaction = totalTransaction;

				return View("UserDetail", profileVM);
			}

			return View("UserDetail");
		}


		[Authorize(Roles = "Admin, Moderator")]
		public IActionResult ReportManager(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _context.Reports
				.Include(x => x.IdUserNavigation) // Join với bảng người dùng
				.Include(x => x.IdPostNavigation) // join với bảng bài đăng

				.Include(x => x.IdTransactionNavigation) // join với bảng transaction
				.ThenInclude(x => x.IdPostNavigation)
				.ThenInclude(x => x.IdUserNavigation)

				.Include(x => x.IdCommentNavigation) // join với bảng comment
				.ThenInclude(x => x.IdUserNavigation) // Join với bảng người dùng
				.AsQueryable();

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => x.Content.Contains(search));
			}

			var comments = query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new ReportVM
			{
				IdReport = x.IdReport,
				IdPost = x.IdPost,
				PostName = x.IdPostNavigation.Content,

				IdUser = x.IdUser,
				Content = x.Content,
				UserAvatar = x.IdUserNavigation.AvatarImg,
				UserFullName = x.IdUserNavigation.Name,

				IdComment = x.IdComment,
				CommentContent = x.IdCommentNavigation.Content,
				UserCommentFullName = x.IdCommentNavigation.IdUserNavigation.Name ?? x.IdTransactionNavigation.IdPostNavigation.IdUserNavigation.Name,
				IdUserComment = x.IdCommentNavigation.IdUser ?? x.IdTransactionNavigation.IdPostNavigation.IdUserNavigation.IdUser,
				UserCommentAvatar = x.IdCommentNavigation.IdUserNavigation.AvatarImg ?? x.IdTransactionNavigation.IdPostNavigation.IdUserNavigation.AvatarImg,

				Date = x.Date,
				IdTransaction = x.IdTransaction,
				PointRating = x.IdCommentNavigation.IdUserNavigation.PointRating ?? x.IdTransactionNavigation.IdPostNavigation.IdUserNavigation.PointRating
			}).ToList();

			PagingModel<ReportVM> pagingModel = new()
			{
				Items = comments,
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = pageSize,
					TotalItems = query.Count(),
					Search = search
				}
			};

			return View("ReportManager", pagingModel);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult EditAccount(string id)
		{
			var user = _context.Users.FirstOrDefault(p => p.IdUser == id);

			if (user == null)
			{
				return NotFound();
			}

			var viewModel = new ProfileVM
			{
				IdUser = user.IdUser,
				Role = user.Role,
				Status = user.Status ?? true, // Ensure status defaults to true if null
				BankName = user.BankName,
				BankAccountNumber = user.BankAccountNumber
			};

			return View(viewModel);
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult UpdateAccount(ProfileVM viewModel)
		{
			try
			{
				// Lưu thông tin bài đăng
				var user = _context.Users
					.FirstOrDefault(p => p.IdUser == viewModel.IdUser);
				if (user == null)
				{
					return NotFound();
				}
				else
				{
					user.Role = viewModel.Role;
					user.Status = viewModel.Status;
					user.BankName = viewModel.BankName;
					user.BankAccountNumber = viewModel.BankAccountNumber;
					_context.SaveChanges();
					TempData["SuccessMessage"] = $"Tài khoản {user.Name} đã được cập nhật lại thành công";
					return RedirectToAction("UserManager", "Admin");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật lại tài khoản {user.Name}.";
			}
			return View(viewModel);
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult ChangeRole([FromBody] ProfileVM user)
		{
			var userFound = _context.Users.FirstOrDefault(x => x.IdUser == user.IdUser);
			if (userFound != null && defaultRole.Contains(user.Role))
			{
				userFound.Role = user.Role;
				_context.Users.Update(userFound);
				_context.SaveChanges();
				return Ok();
			}
			return BadRequest();
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public IActionResult DeleteComment([FromBody] CommentVM comment)
		{
			var commentExisted = _context.Comments.FirstOrDefault(x => x.IdComment == comment.IdComment);
			if (commentExisted != null)
			{
				_context.Comments.Remove(commentExisted);
				_context.SaveChanges();
				return Ok();
			}
			return BadRequest();
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> DeleteReport([FromBody] ReportVM report)
		{
			if (report.IdTransaction != null) return BadRequest();
			var reportExisted = _context.Reports.FirstOrDefault(x => x.IdReport == report.IdReport);
			if (reportExisted != null)
			{
				await _context.Database.BeginTransactionAsync();
				try
				{
					_context.Reports.Remove(reportExisted);

					var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.IdComment == report.IdComment);
					if (notification != null)
					{
						_context.Notifications.Remove(notification);
					}

					var comment = await _context.Comments.FirstOrDefaultAsync(x => x.IdComment == report.IdComment);
					if (comment != null)
					{
						_context.Comments.Remove(comment);

					}
					await _context.SaveChangesAsync();
					await _context.Database.CommitTransactionAsync();
				}
				catch (Exception ex)
				{
					await _context.Database.RollbackTransactionAsync();
				}
				return Ok();
			}
			return BadRequest();
		}
		[Authorize(Roles = "Admin")]
		public IActionResult Payment(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _context.Payments.AsQueryable();
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => x.IdTransaction.ToString().ToLower().Contains(search.ToLower()));
			}

			var payment = query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => _mapper.Map<PaymentVM>(x)).ToList();

			var link = _configuration.GetValue<string>("VNPayMerchant");

			PagingModel<PaymentVM> pagingModel = new()
			{
				Items = payment,
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = pageSize,
					TotalItems = query.Count(),
					Search = search
				}
			};
			ViewBag.link = link;
			return View("Payment", pagingModel);
		}

		/// <summary>
		/// Admin xử lý trừ point người dùng bình báo cáo đánh giá
		/// </summary>
		/// <param name="report"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> PointHandler([FromBody] ReportVM report)
		{
			var reportExists = _context.Reports.FirstOrDefault(x => x.IdReport == report.IdReport);
			User user = null;
			Comment comment = null;
			Transaction transaction = null;
			int pointRating = 0;
			if (reportExists != null)
			{
				// trừ 10 điểm của người bình luận nếu người khác báo cáo
				if (report.IdComment != null)
				{
					comment = await _context.Comments.FirstOrDefaultAsync(x => x.IdComment == report.IdComment);
					if (comment != null)
					{
						user = await _context.Users.FirstOrDefaultAsync(x => x.IdUser == comment.IdUser);
						if (user != null)
						{
							user.PointRating -= 10;
							pointRating += 10;
						}
					}
				}

				// trừ 20 điểm của người đăng bài nếu người dùng báo cáo mua hàng
				if (report.IdTransaction != null)
				{
					transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.IdTransaction == report.IdTransaction);
					if (transaction != null)
					{
						user = await _context.Users.FirstOrDefaultAsync(x => x.IdUser == transaction.IdUserSeller);
						if (user != null)
						{
							user.PointRating -= 20;
							pointRating += 20;
						}
					}
				}

				if (user != null)
				{
					await _context.Database.BeginTransactionAsync();
					try
					{
						_context.Users.Update(user);
						await _context.SaveChangesAsync();
						if (transaction != null)
						{
							await notificationServices.CreateNotification(user.IdUser, "ReportTransaction", "Transaction", pointRating.ToString(), transaction.IdTransaction);
						}

						if (comment != null)
						{
							await notificationServices.CreateNotification(user.IdUser, "ReportComment", "Comment", pointRating.ToString(), comment.IdComment);
						}
						await _context.Database.CommitTransactionAsync();
					}
					catch (Exception ex)
					{
						await _context.Database.RollbackTransactionAsync();
					}
				}
				return Ok();
			}
			return BadRequest();
		}
	}
}
