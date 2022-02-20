namespace E_Commerce_WebApi.Entities.Models
{
    public class ProductCreateModel
    {
        public ProductCreateModel()
        {

        }


        public ProductCreateModel(string articleNumber, string name, string description, decimal price, string category)
        {
            ArticleNumber = articleNumber;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
        }

        public string ArticleNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}
