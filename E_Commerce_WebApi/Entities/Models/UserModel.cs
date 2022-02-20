namespace E_Commerce_WebApi.Entities.Models
{
    public class UserModel
    {
        public UserModel()
        {

        }

        public UserModel(int id)
        {
            Id = id;
        }

        public UserModel(int id, string firstname, string lastname, string email, string role, string street, string zipcode, string city)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Role = role;
            Street = street;
            Zipcode = zipcode;
            City = city;
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }

    }
}
