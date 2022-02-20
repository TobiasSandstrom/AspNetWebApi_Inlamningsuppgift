namespace E_Commerce_WebApi.Entities.Models
{
    public class AddressCreateModel
    {
        public AddressCreateModel()
        {

        }

        public AddressCreateModel(string street, string zipcode, string city)
        {
            Street = street;
            Zipcode = zipcode;
            City = city;
        }

        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
    }
}
