namespace E_Commerce_WebApi.Entities.Models
{
    public class AddressModel
    {
        public AddressModel()
        {

        }

        public AddressModel(int id, string street, string zipcode, string city)
        {
            Id = id;
            Street = street;
            Zipcode = zipcode;
            City = city;
        }
        public int Id { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
    }
}
