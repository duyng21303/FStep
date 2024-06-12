using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FStep.Controllers.Customer
{
    public class BuyNowController : Controller
    {
        private readonly FstepDBContext _db;
        private readonly IMapper _mapper;
        public  BuyNowController(FstepDBContext context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> BuyNow(int id, int Quantity) 
        {
            var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post == null)
            {
                return NotFound();
            }

            var product = _db.Products.FirstOrDefault(p => p.IdProduct == post.IdProduct);
            if(product == null)
            {
                return NotFound();
            }
            String userID = User.FindFirstValue("userID");
            var payment = new TransactionVM
            {
                IdPost = id,
                IdUserSeler = post.IdUser,
                IdUserBuyer = userID,
                NameProduct =post.Content ?? String.Empty,
                Image = post.Img,
                Price = (float)product.Price,
                Quantity = Quantity,
                Amount = (float)(product.Price * Quantity)
            };
            return View("Payment", payment);
        }
    }
}
