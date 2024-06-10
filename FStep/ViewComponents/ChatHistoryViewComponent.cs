using FStep.Data;
using Microsoft.AspNetCore.Mvc;

namespace FStep.ViewComponents
{
	public class ChatHistoryViewComponent : ViewComponent
	{
		private readonly FstepDbContext db;

		public ChatHistoryViewComponent(FstepDbContext context)
		{
			db = context;
		}
		public async Task<IViewComponentResult> InvokeAsync(string userID)
		{

			return View();
		}
	}
}
