namespace Uniqlo2.ViewModels.Basket
{
    public class BasketProductItemVM
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public BasketProductItemVM (int id)
        {
            Id = id;
            Count = 1;
        }
    }
}
