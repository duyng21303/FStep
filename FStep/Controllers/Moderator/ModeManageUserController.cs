using AutoMapper;
using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FStep.Controllers.ManagePost
{
    public class ModeManageUserController : Controller
    {

        private readonly FstepDBContext _context;
        private readonly IMapper _mapper;
        private static readonly string[] defaultRole = new[] { "Customer", "Moderator", "Administrator" };

        public ModeManageUserController(FstepDBContext context, IMapper mapper)


        {
            _context = context;
            _mapper = mapper;
        }
		[Authorize(Roles = "Moderator")]
		public IActionResult UserManager(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _context.Users.AsQueryable();
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => x.Email.ToLower().Contains(search.ToLower()) || x.StudentId.ToLower().Contains(search.ToLower()));

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

		[Authorize(Roles = "Moderator")]
		public IActionResult ProcessPoint(String? id)
		{
			var user = _context.Users.FirstOrDefault(user => user.IdUser == id);
			if(user == null)
			{
				TempData["SuccessMessage"] = $"Người dùng {user.Name} không tồn tại.";
			}
			return View();
		}

	}
}
