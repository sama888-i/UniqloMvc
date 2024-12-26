﻿namespace Uniqlo2.ViewModels.Basket
{
    public class ProductsBasketItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Discount { get; set; }
        public decimal SellPrice { get; set; }
        public string ImageUrl { get; set; }

    }
}
