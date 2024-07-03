using FStep.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FStep.Controllers.Admin
{
    public class AdminPostController : Controller
    {
        private readonly FstepDbContext _db;

        public AdminPostController(FstepDbContext context) {
            _db = context;
        }
        [Authorize]

        public IActionResult ManagePost()
        {
            return View();
        }
    }
}
