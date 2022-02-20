using System.Security.Cryptography;
using System.Text;

namespace E_Commerce_WebApi.Entities.Models
{
    public class HashCreateModel
    {
        public byte[] Pass { get; set; } = null!;
        public byte[] Salt { get; set; } = null!;

        public void CreateSecurePassword(string password)
        {
            using var hmac = new HMACSHA512();
            Salt = hmac.Key;
            Pass = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }


        public bool CompareSecurePassword(string password)
        {
            using (var hmac = new HMACSHA512(Salt))
            {
                var _pass = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < _pass.Length; i++)
                {
                    if (_pass[i] != Pass[i])
                        return false;
                }
            }

            return true;
        }



    }
}
