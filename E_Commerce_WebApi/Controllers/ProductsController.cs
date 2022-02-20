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
    public class ProductsController : ControllerBase
    {
        private readonly SqlContext _context;

        public ProductsController(SqlContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        [UseApiKey]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        {
            var products = await _context.Products.Include(x => x.Category).ToListAsync();
            var productModels = new List<ProductModel>();
            foreach (var p in products)
            {
                var model = new ProductModel(p.Id, p.Artnumber, p.ProductName, p.ProductDescription, p.Price, p.Category.Category1);
                productModels.Add(model);
            }
            return productModels;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [UseApiKey]
        public async Task<ActionResult<ProductModel>> GetProduct(int id)
        {
            var p = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
            {
                return NotFound();
            }
            var model = new ProductModel(p.Id, p.Artnumber, p.ProductName, p.ProductDescription, p.Price, p.Category.Category1);
            return model;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [UseAdminApiKey]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateModel model)
        {
            
            if (id != model.Id)
            {
                return BadRequest();
            }
           
            var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductName == model.Name);
            if (product != null && product.ProductName.ToLower() != model.Name.ToLower())
            {
                return BadRequest("Product with that name already exists");
            }
            product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Artnumber == model.ArticleNumber);
            if (product != null && product.Id != model.Id)
            {
                return BadRequest("Product with that articlenumber already exists");
            }

            var newCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Category1 == model.Category);
            if (newCategory == null)
            {
                var category = new Category();
                category.Category1 = model.Category;
                newCategory = category;
                _context.Categories.Add(newCategory);
                await _context.SaveChangesAsync();
            }


            product.Id = model.Id;
            product.Artnumber = model.ArticleNumber;
            product.ProductName = model.Name;
            product.ProductDescription = model.Description;
            product.Price = model.Price;
            product.CategoryId = newCategory.Id;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [UseAdminApiKey]
        public async Task<ActionResult<Product>> PostProduct(ProductCreateModel model)
        {
            var product = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Artnumber == model.ArticleNumber);
            if (product != null)
            {
                return BadRequest("Product with that articlenumber already exists");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Category1 == model.Category);
            if (category == null)
            {
                var _category = new Category()
                {
                    Category1 = model.Category
                };
                category = _category;
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            var _product = new Product()
            {
                ProductName = model.Name,
                ProductDescription = model.Description,
                Artnumber = model.ArticleNumber,
                Price = model.Price,
                CategoryId = category.Id
            };
            _context.Products.Add(_product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = _product.Id }, _product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [UseAdminApiKey]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product =  await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = "";
            product.ProductDescription = "";
            product.Price = 0;


            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
