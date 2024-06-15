using FStep.Data;
using Microsoft.AspNetCore.Mvc;

namespace FStep.ViewComponents
{
	public class CheckoutInfoComponent : ViewComponent
	{
		private readonly FstepDBContext db;

		public CheckoutInfoComponent(FstepDBContext context) => db = context;

		//public IViewComponentResult Invoke()
		//{
		//	var data = db.Loais.Select(lo => new MenuLoaiVM
		//	{
		//		MaLoai = lo.MaLoai,
		//		TenLoai = lo.TenLoai,
		//		SoLuong = lo.HangHoas.Count()
		//	});
		//	return View(data); //Default.cshtml
		//					   //return View("Default", data);
		//}
	}
}
