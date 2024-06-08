using FStep.Data;
using Microsoft.AspNetCore.Mvc;

namespace FStep.ViewComponents
{
	public class ChatHistoryViewComponent : ViewComponent
	{
		private readonly FstepDBContext db;

		public ChatHistoryViewComponent(FstepDBContext context)
		{
			db = context;
		}
		public async Task<IViewComponentResult> InvokeAsync(string userID)
		{

			return View();
		}
	}
}
