namespace Uniqlo2.ViewModels.Basket
{
    public class BasketVM
    {
        public IEnumerable<ProductsBasketItemVM> Products { get; set; }
        public decimal Subtotal { get; set; }
    }
}
