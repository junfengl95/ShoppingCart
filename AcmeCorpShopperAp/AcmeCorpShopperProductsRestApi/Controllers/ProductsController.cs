using AcmeCorpShopperProductsRestApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpShopperProductsRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsAcmeContext _context;

        public ProductsController(ProductsAcmeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProductAsync([FromBody] Product product)
        {
            // Exclude productId from being set explicitly
            // Primary key is set to Identity
            // Other option is to SET IDENTITY_INSERT Products OFF in SQL
            // Or set it to null if it's nullable

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return this.CreatedAtAction("ReadProductById", new { id = product.ProductId }, product);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> ReadAllProductsAsync()
        {
            return this.Ok(await _context.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> ReadProductByIdAsync(int id)
        {
            var foundProduct = await _context.Products.FindAsync(id);
            if (object.Equals(foundProduct, null))
            {
                return this.NotFound();
            }
            return this.Ok(foundProduct);

        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Product>>> ReadProductByNameAsync(string name)
        {
            var foundProduct = await _context.Products.Where(product => product.ProductName == name).ToListAsync();
            if (object.Equals(foundProduct, null))
            {
                return this.BadRequest();
            }
            return this.Ok(foundProduct);
        }

        [HttpPut]
        public async Task<ActionResult<Product>> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (object.ReferenceEquals(existingProduct, null))
            {
                return this.NotFound();
            }
            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductPrice = product.ProductPrice;
            await _context.SaveChangesAsync();
            return this.Ok(existingProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProductAsync(int id)
        {
            var foundProduct = await _context.Products.FindAsync(id);
            if (object.ReferenceEquals(foundProduct, null))
            {
                return this.NotFound();
            }
            _context.Products.Remove(foundProduct);
            await _context.SaveChangesAsync();
            return this.Ok();
        }
    }
}
