using Microsoft.AspNetCore.Mvc;

namespace FStep.ViewComponents
{
	public class CreatePostViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}

	}
}
