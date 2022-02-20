namespace E_Commerce_WebApi.Entities.Models
{
    public class OrderUpdateModel
    {
        public int OrderId { get; set; }
        public List<OrderrowCreateModel> Orderrows { get; set; }
        public string Orderstatus { get; set; }
        public int UserId { get; set; }
    }
}
