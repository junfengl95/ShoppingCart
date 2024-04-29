using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly ProductsContext _context;

		public ProductController(ProductsContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<ActionResult<Product>> CreateProductAsync([FromBody] Product product)
		{
			//Exclude productId from being set explicitly
			// Primary key set as Identity

			_context.Products.Add(product);
			await _context.SaveChangesAsync();
			return CreatedAtAction("ReadProductById", new { id = product.ProductId }, product);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> ReadAllProductsAsync()
		{
			return Ok(await _context.Products.ToListAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Product>> ReadProductByIdAsync(int id)
		{
			var foundProduct = await _context.Products.FindAsync(id);
			if (ReferenceEquals(foundProduct, null))
			{
				return NotFound();
			}
			return Ok(foundProduct);
		}

		[HttpGet("name/{name}")]
		public async Task<ActionResult<IEnumerable<Product>>> ReadProductByNameAsync(string name)
		{
			var foundProducts = await _context.Products.Where(p => p.ProductName == name).ToListAsync();
			if (ReferenceEquals(foundProducts, null))
			{
				return BadRequest();
			}
			return Ok(foundProducts);
		}

		[HttpPut]
		public async Task<ActionResult<Product>> UpdateProductAsync(Product product)
		{
			var existingProduct = await _context.Products.FindAsync(product.ProductId);
			if (ReferenceEquals(existingProduct, null))
			{
				return NotFound();
			}
			existingProduct.ProductName = product.ProductName;
			existingProduct.ProductPrice = product.ProductPrice;
			existingProduct.ProductQuantity = product.ProductQuantity;
			//existingProduct.ProductRating = product.ProductRating;
			await _context.SaveChangesAsync();
			return Ok(existingProduct);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<Product>> DeleteProductByIdAsync(int id)
		{
			var foundProduct = await _context.Products.FindAsync(id);
			if (ReferenceEquals(foundProduct, null))
			{
				return NotFound();
			}
			_context.Products.Remove(foundProduct);
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}
