namespace E_Commerce_WebApi.Entities.Models
{
    public class ProductModel
    {
        public ProductModel()
        {
            
        }


        public ProductModel(int id, string articleNumber, string name, string description, decimal price, string category)
        {
            Id = id;
            ArticleNumber = articleNumber;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
        }

        public int Id { get; set; }
        public string ArticleNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }


    }
}
