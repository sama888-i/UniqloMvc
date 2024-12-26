using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using Uniqlo2.DataAccsess;
using Uniqlo2.Models;
using Uniqlo2.ViewModels.Basket;
using Uniqlo2.ViewModels.Product;

namespace Uniqlo2.Controllers
{
    public class BasketController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var basketItems = await _context.Products
            .Include(y=>y.BasketProducts)
            .Include(x => x.Images)
            .Select(x => new UsersBasketItemVM
            {
                Id = x.Id,
                Name = x.Name,
                SellPrice = x.SellPrice,
                ImageUrl = x.CoverImage,
                Quantity = x.Quantity
            }).ToListAsync();

            return View(basketItems);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id))
                return NotFound();
            var basketItems = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");
            var item = basketItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                item = new BasketProductItemVM(id);
                basketItems.Add(item);
            }
            else
            {
                item.Count++;
            }
            Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));
            if (User.Identity?.IsAuthenticated ?? true)
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
                       
                var Item = await _context.UsersBasket.Include(x=>x.BasketProducts).ThenInclude(x=>x.Product).FirstOrDefaultAsync(x => x.UserId == userId);
                if (Item is null)
                {
                    Item= new Basket
                    {
                        UserId = userId,
                        Id = id,
                        
                    };
                    _context.UsersBasket.Add(Item);

                }
               /* else
                {
                    Item.Quantity++;
                }*/
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> GetBasket()
        {
            BasketVM vm = new();
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
            decimal sum = 0;
            foreach (var item in prods)
            {
                item.Count = basketIds!.FirstOrDefault(x => x.Id == item.Id)!.Count;
                sum += (100 - item.Discount) / 100m * item.SellPrice * item.Count;
            }
            vm.Products = prods;
            vm.Subtotal = sum;
            return PartialView("_BasketPartial", vm);
        }
    }

}
