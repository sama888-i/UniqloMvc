using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using Uniqlo2.DataAccsess;
using Uniqlo2.Models;
using Uniqlo2.ViewModels.ProductDetails;

namespace Uniqlo2.Controllers
{
    public class ProductController(UniqloDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult>Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();
            var data = await _context.Products
                .Where(x => x.Id == id.Value && !x.IsDeleted)
                .Include(x => x.Images)
                .Include(x=>x.Ratings)
                .Include(x=>x.Comments)
                .FirstOrDefaultAsync();
            if (data is null) return NotFound();
            ViewBag.Rating = 5;
            if(User.Identity?.IsAuthenticated ?? false)
            {
                string userId = User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)!.Value;
                int rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id).Select(x => x.Rating).FirstOrDefaultAsync();
                ViewBag.Rating = rating == 0 ? 5 :rating;
            }
            return View(data);


        }
        public async Task<IActionResult> Rating(int productId,int rating)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();
            if(data is null)
            {
                await _context.ProductRatings.AddAsync(new Models.ProductRating
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating
                });
            }
            else
            {
                data.Rating = rating;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new {Id=productId});
        }
        
        public async Task<IActionResult> AddComment(ProductCommentVM vm,int productId)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var data = await _context.ProductComments.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();


            ProductComment comment = new ProductComment
            {
                UserId=userId,
                ProductId=productId,
                Comment = vm.Comment,
            };


            await _context.ProductComments.AddAsync(comment);

             await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details),new {Id=productId});
        }
    }
}
