namespace E_Commerce_WebApi.Entities.Models
{
    public class OrderrowModel
    {
        public OrderrowModel(int id, ProductModel product, int quantity)
        {
            Id = id;
            Product = product;
            Quantity = quantity;
        }

        public int Id { get; set; }
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public void SetTotalPrice(Orderrow orderrow)
        {
            TotalPrice = orderrow.Product.Price * orderrow.Quantity; 
        }
    }
}
