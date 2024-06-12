using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace FStep.Controllers
{
	public class ProductController : Controller
	{
		private readonly FstepDBContext db;
		
		public ProductController(FstepDBContext context)
		{
			db = context;
		}
		public IActionResult Index(int? type)
		{
			var product = db.Products.AsQueryable();

			if (type.HasValue)
			{
				product = product.Where(p => p.IdProduct == type.Value);
			}
			var result = product.Select(p => new ProductVM
			{
				IdProduct = p.IdProduct,
				Name = p.Name,
				Price = p.Price ?? 0,
				Detail = p.Detail ?? "",
				RecieveImg = p.RecieveImg ?? "",

			});
			return View(result);
		}
		public IActionResult Detail(int id)
		{
			var data = db.Products.SingleOrDefault(p => p.IdProduct == id);

			return View(data);
		}

	}
}
