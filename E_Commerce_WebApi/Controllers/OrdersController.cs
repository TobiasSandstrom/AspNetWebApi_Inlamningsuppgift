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
    public class OrdersController : ControllerBase
    {
        private readonly SqlContext _context;

        public OrdersController(SqlContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        [UseApiKey]
        public async Task<ActionResult<IEnumerable<OrderModel>>> GetOrders()
        {
            var orderList = await _context.Orders.Include(x => x.OrderStatus).ToListAsync();
            var orderModelList = new List<OrderModel>();
            

            foreach (var o in orderList)
            {
                var _orderrows = new List<OrderrowModel>();
                var user = await _context.Users.Include(x => x.Address).Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == o.UserId);
                var userModel = new UserModel();
                
                if (user.Firstname != null)
                {
                    userModel.Id = user.Id;
                    userModel.Firstname = user.Firstname;
                    userModel.Lastname = user.Lastname;
                    userModel.Email = user.Email;
                    userModel.Role = user.Role.Rolename;
                    userModel.Street = user.Address.Street;
                    userModel.Zipcode = user.Address.Zipcode;
                    userModel.City = user.Address.City;
                }
                else
                {
                    userModel.Id = user.Id;
                }

                var orderrows = await _context.Orderrows.Where(x => x.OrderId == o.Id).ToListAsync();
                if (orderrows != null)
                {
                    foreach (var row in orderrows)
                    {
                        var _product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == row.ProductId);
                        var _productModel = new ProductModel(_product.Id, _product.Artnumber, _product.ProductName, _product.ProductDescription, _product.Price, _product.Category.Category1);
                        var _row = new OrderrowModel(row.Id, _productModel, row.Quantity);
                        _row.SetTotalPrice(row);
                        _orderrows.Add(_row);

                    }
                }
                

                

                var orderModel = new OrderModel(o.Id, _orderrows, userModel, o.Created, o.OrderStatus.Orderstatus);
                orderModel.SetTotalPrice(_orderrows);
                orderModelList.Add(orderModel);
            }
            
            return orderModelList;
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [UseApiKey]
        public async Task<ActionResult<OrderModel>> GetOrder(int id)
        {
            var order = await _context.Orders.Include(x => x.User).Include(x => x.OrderStatus).FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound("Cant find an order with that ID");
            }

            var user = await _context.Users.Include(x => x.Address).Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == order.UserId);
            var userModel = new UserModel();

            if (user.Firstname != null)
            {
                userModel.Id = user.Id;
                userModel.Firstname = user.Firstname;
                userModel.Lastname = user.Lastname;
                userModel.Email = user.Email;
                userModel.Role = user.Role.Rolename;
                userModel.Street = user.Address.Street;
                userModel.Zipcode = user.Address.Zipcode;
                userModel.City = user.Address.City;
            }
            else
            {
                userModel.Id = user.Id;
            }

            var orderrows = await _context.Orderrows.Include(x => x.Product).Where(x => x.OrderId == order.Id).ToListAsync();
            var _orderrows = new List<OrderrowModel>();
            foreach (var row in orderrows)
            {
                var _product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == row.ProductId);
                var _productModel = new ProductModel(_product.Id, _product.Artnumber, _product.ProductName, _product.ProductDescription, _product.Price, _product.Category.Category1);
                var _row = new OrderrowModel(row.Id, _productModel, row.Quantity);
                _row.SetTotalPrice(row);
                _orderrows.Add(_row);
            }

            var orderModel = new OrderModel(order.Id, _orderrows, userModel, order.Created, order.OrderStatus.Orderstatus);
            orderModel.SetTotalPrice(_orderrows);
            return orderModel;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [UseAdminApiKey]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateModel model)
        {
            if (id != model.OrderId)
            {
                return BadRequest();
            }
            

            //Tar bort de gamla orderraderna
            var oldRows = await _context.Orderrows.Where(x => x.OrderId == id).ToListAsync();
            foreach (var row in oldRows)
            {
                _context.Orderrows.Remove(row);
                await _context.SaveChangesAsync();
            }

            //Sparar in nya orderraderna

            foreach (var row in model.Orderrows)
            {
                var orderrowList = new List<Order>();
                var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Artnumber == row.Articlenumber);

                var productcheck = await _context.Products.Where(x => x.Artnumber == row.Articlenumber).FirstOrDefaultAsync();

                var orderrow = new Orderrow()
                {
                    Quantity = row.Quantity,
                    ProductId = product.Id,
                    OrderId = id

                };
                _context.Orderrows.Add(orderrow);
                await _context.SaveChangesAsync();

            }

            //Ändrar statusen
            
            var status = await _context.Statuses.Where(x => x.Orderstatus == model.Orderstatus).FirstOrDefaultAsync();

            if (status != null)
            {
                _context.Entry(status).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            else
            {
                var _status = new Status()
                {
                    Orderstatus = model.Orderstatus
                };
                status = _status;
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();
                _context.Entry(status).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            

            

            //Ändrar kunden
            var user = await _context.Users.Where(x => x.Id == model.UserId).FirstOrDefaultAsync();
            if(user == null)
            {
                return BadRequest("No user with that ID in database");
            }




            //Ändrar ordern 
            var order = await _context.Orders.FindAsync(id);



            order.OrderStatusId = status.Id;
            order.UserId = user.Id;


            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [UseApiKey]
        public async Task<ActionResult<Order>> PostOrder(OrderCreateModel model)
        {
            // Kollar så att användaren finns
            var user = await _context.Users.FindAsync(model.CustomerId);
            if (user == null)
            {
                return BadRequest("Cant find user with that ID");
            }
            if (user.Firstname == null)
            {
                return BadRequest("Assigned user is not a user of our service anymore, please choose another user or sign up again");

            }

            //Kollar så användaren inte angivit fel artikelnummer
            foreach (var row in model.OrderRows)
            {
                var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Artnumber == row.Articlenumber);
                if (product == null)
                {
                    return BadRequest("Couldnt find one or more articlenumbers assigned to this request");
                }
            }

            //Kollar efter statusen, om ingen angetts blir den automatiskt "Created"
            var status = await _context.Statuses.FirstOrDefaultAsync(x => x.Orderstatus == "Created");
            if (status == null)
            {
                var _status = new Status();
                _status.Orderstatus = "Created";
                status = _status;
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();
            }

            //Skapar och sparar ordern
            var order = new Order()
            {
                UserId = model.CustomerId,
                OrderStatusId = status.Id,
                Created = DateTime.Now,
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            

            //Skapar alla orderrader och sparar dem
            foreach (var row in model.OrderRows)
            {
                var orderrowList = new List<Order>();
                var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Artnumber == row.Articlenumber);

                var productcheck = await _context.Products.Where(x => x.Artnumber == row.Articlenumber).FirstOrDefaultAsync();
                    
                var orderrow = new Orderrow()
                {
                    Quantity = row.Quantity,
                    ProductId = product.Id,
                    OrderId = order.Id

                };
                _context.Orderrows.Add(orderrow);
                await _context.SaveChangesAsync();
                
            }

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        [UseAdminApiKey]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var orderrows = await _context.Orderrows.Where(x => x.OrderId == id).ToListAsync();

            foreach (var row in orderrows)
            {
                _context.Orderrows.Remove(row);
                await _context.SaveChangesAsync();
            }
            
            
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
