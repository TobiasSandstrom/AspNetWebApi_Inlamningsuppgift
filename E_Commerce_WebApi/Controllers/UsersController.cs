#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_WebApi.Data;
using E_Commerce_WebApi.Entities;
using E_Commerce_WebApi.Entities.Models;
using E_Commerce_WebApi.Filters;

namespace E_Commerce_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SqlContext _context;

        public UsersController(SqlContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [UseAdminApiKey]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            var userModelList = new List<UserModel>();
            var userList = await _context.Users.Include(x => x.Address).Include(x => x.Role).ToListAsync();
            foreach (var user in userList)
            {
                 if(user.Firstname != null)
                {
                    var model = new UserModel(user.Id, user.Firstname, user.Lastname, user.Email, user.Role.Rolename, user.Address.Street, user.Address.Zipcode, user.Address.City);
                    userModelList.Add(model);
                    
                }
                else
                {
                    var model = new UserModel(user.Id);
                    userModelList.Add(model);

                }
            }
            return userModelList;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [UseAdminApiKey]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var user = await _context.Users.Include(x => x.Address).Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            if (user.Firstname != null)
            {
                var userModel = new UserModel(user.Id, user.Firstname, user.Lastname, user.Email, user.Role.Rolename, user.Address.Street, user.Address.Zipcode, user.Address.City);
                return userModel;
            }
            else
            {
                var userModel = new UserModel(user.Id);
                return userModel;
            }

        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [UseApiKey]
        public async Task<IActionResult> PutUser(int id, UserUpdateModel model)
        {
            var user = await _context.Users.Include(x => x.Address).Include(x => x.Role).Include(x => x.Hash).FirstOrDefaultAsync(x => x.Id == model.Id);
            
            if (user != null)
            {
                if (user.Id != id && user.Email != model.Email)
                {
                    return BadRequest("User with that email already exists");
                }
            }
            if (id != model.Id)
            {
                return BadRequest();
            }

            

            var newAddress = await _context.Addresses.Where(x => x.Street == model.Street && x.Zipcode == model.Zipcode).FirstOrDefaultAsync();

            if (newAddress == null)
            {
                var _address = new Address()
                {
                    Street = model.Street,
                    Zipcode = model.Zipcode,
                    City = model.City
                };
                newAddress = _address;
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

            }
            

            var newHash = new HashCreateModel();
            newHash.CreateSecurePassword(model.Password);
            var _hash = new Hash()
            {
                Salt = newHash.Salt,
                Pass = newHash.Pass
            };
            var oldHash = await _context.Hashes.FirstOrDefaultAsync(x => x.Id == user.HashId);
            
            _context.Hashes.Remove(oldHash);
            _context.Hashes.Add(_hash);
            await _context.SaveChangesAsync();
            

            var newRole = await _context.Roles.FirstOrDefaultAsync(x => x.Rolename == model.Role);
            if (newRole == null)
            {
                var _newRole = new Role()
                {
                    Rolename = model.Role
                };
                newRole = _newRole;
                
                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();
            }
            



            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.AddressId = newAddress.Id;
            user.HashId = _hash.Id;
            user.RoleId = newRole.Id;

            _context.Entry(user).State = EntityState.Modified;

            
           

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [UseApiKey]
        public async Task<ActionResult<UserModel>> CreateUser(UserCreateModel model)
        {

            var _user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (_user != null)
            {
                return BadRequest("User with that email already exists");

            }
            if (_user != null && _user.Firstname == null)
            {
                return BadRequest("An old user with that email already exists, contact administratior if you want to signup again");
                
            }


            
            var address = await _context.Addresses
                .FirstOrDefaultAsync(x => x.Street == model.Street && x.Zipcode == model.Zipcode);

            if(address == null)
            {

                var _address = new Address()
                {
                    Street = model.Street,
                    Zipcode = model.Zipcode,
                    City = model.City
                };
                address = _address;
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
            }

            var _hash = new HashCreateModel();
            _hash.CreateSecurePassword(model.Password);
            var userHash = new Hash();
            userHash.Pass = _hash.Pass;
            userHash.Salt = _hash.Salt;
            _context.Hashes.Add(userHash);
            await _context.SaveChangesAsync();


            var userRole = await _context.Roles.FirstOrDefaultAsync(x => x.Rolename == model.Role);
            if(userRole == null)
            {
                var _userRole = new Role()
                {
                    Rolename = model.Role
                };
                userRole = _userRole;
                _context.Roles.Add(userRole);
                await _context.SaveChangesAsync();
            }

            var user = new User()
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                AddressId = address.Id,
                HashId = userHash.Id,
                RoleId = userRole.Id,
            };
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var userModel = new UserModel(user.Id, user.Firstname, user.Lastname, user.Email, user.Role.Rolename, user.Address.Street, user.Address.Zipcode, user.Address.City);

            return CreatedAtAction("GetUser", new { id = user.Id }, userModel);


        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        //[UseAdminApiKey]
        public async Task<IActionResult> DeleteUser(int id)
        {



            var user = await _context.Users.Include(x => x.Address).Include(x => x.Role).Include(x => x.Hash).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.Firstname = null;
            user.Lastname = null;
            user.AddressId = null;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
