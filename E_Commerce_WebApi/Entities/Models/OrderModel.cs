namespace E_Commerce_WebApi.Entities.Models
{
    public class OrderModel
    {
        public OrderModel()
        {

        }

        public OrderModel(int id, List<OrderrowModel> orderrows, UserModel user, DateTime createdDate, string status)
        {
            Id = id;
            Orderrows = orderrows;
            User = user;
            CreatedDate = createdDate;
            Status = status;
        }

        public int Id { get; set; }
        public List<OrderrowModel> Orderrows { get; set; }
        public UserModel User { get; set; }
        public DateTime CreatedDate { get; set;}
        public string Status { get; set; }
        public decimal TotalPrice { get; private set; }

        public void SetTotalPrice(List<OrderrowModel> orderrows)
        {
            decimal totalPrice = 0;
            foreach (var p in orderrows)
            {
                totalPrice += p.TotalPrice;
            }
            TotalPrice = totalPrice;
        }
    }
}
