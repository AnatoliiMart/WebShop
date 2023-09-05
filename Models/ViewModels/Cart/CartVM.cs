namespace WebShop.Models.ViewModels.Cart
{
    public class CartVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get => Quantity * Price; }

        public string Image { get; set; }  

        public CartVM() { }
    }
}