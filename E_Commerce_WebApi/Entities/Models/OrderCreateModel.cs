namespace E_Commerce_WebApi.Entities.Models
{
    public class OrderCreateModel
    {
        public OrderCreateModel()
        {

        }
        public OrderCreateModel(int customerId, List<OrderrowCreateModel> orderRows, string orderstatus)
        {
            CustomerId = customerId;
            OrderRows = orderRows;
            Orderstatus = orderstatus;
        }

        public int CustomerId { get; set; }
        public List<OrderrowCreateModel> OrderRows { get; set; }
        public string Orderstatus { get; set; }

    }
}
