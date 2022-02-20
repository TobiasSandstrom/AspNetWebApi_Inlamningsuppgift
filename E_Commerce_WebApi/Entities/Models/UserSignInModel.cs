namespace E_Commerce_WebApi.Entities.Models
{
    public class UserSignInModel
    {
        public UserSignInModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }
}
