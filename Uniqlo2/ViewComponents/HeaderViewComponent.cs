using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Uniqlo2.DataAccsess;
using Uniqlo2.ViewModels.Basket;

namespace Uniqlo2.ViewComponents
{
    public class HeaderViewComponent(UniqloDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketIds = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");
            var prods = await _context.Products
                .Where(x => basketIds.Select(y => y.Id).Any(y => y == x.Id))
                .Select(x => new ProductsBasketItemVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    SellPrice = x.SellPrice,
                    Discount = x.Discount,
                    ImageUrl = x.CoverImage
                }).ToListAsync();
            foreach(var item in prods)
            {
                item.Count=basketIds!.FirstOrDefault(x => x.Id == item.Id)!.Count;

            }
            return View(prods);
        }
    }
}
