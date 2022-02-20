using E_Commerce_WebApi.Data;
using E_Commerce_WebApi.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSignInController : ControllerBase
    {


        private readonly SqlContext _context;
        private readonly IConfiguration _configuration;

        public UserSignInController (SqlContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("SignIn")]
        public async Task<ActionResult> SignIn(UserSignInModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            { return BadRequest("Email and password must be provided"); }

            var user = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Email == model.Email);
            
            if (user == null) 
            { return BadRequest("Incorrect email or password"); }
                


            var _hash = await _context.Hashes.FirstOrDefaultAsync(x => x.Id == user.HashId);
            var hash = new HashUpdateModel()
            {
                Pass = _hash.Pass,
                Salt = _hash.Salt,
            };

            if (!hash.CompareSecurePassword(model.Password)) 
            { return BadRequest("Incorrect email or password"); }
                



            if (user.Role.Rolename == "Admin")
            { 
                string adminkey = _configuration.GetValue<string>("AdminApiKey");

                return Ok($"Welcome {user.Firstname} {user.Lastname}, your key is: {adminkey}"); 
            }


            string key = _configuration.GetValue<string>("ApiKey");
            return Ok($"Welcome {user.Firstname} {user.Lastname}, your key is: {key}");
            
        }



    }
}
