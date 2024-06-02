using Microsoft.AspNetCore.Authorization;
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
			return CreatedAtAction("ReadProductById", new { productId = product.ProductId }, product);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Product>>> ReadAllProductsAsync()
		{

            return await _context.Products.ToListAsync();
		}

		[HttpGet("{productId}")]
		public async Task<ActionResult<Product>> ReadProductByIdAsync(int productId)
		{
			var foundProduct = await _context.Products.FindAsync(productId);
			if (ReferenceEquals(foundProduct, null))
			{
				return NotFound();
			}
			return foundProduct;
		}

		[HttpGet("name/{name}")]
		public async Task<ActionResult<IEnumerable<Product>>> ReadProductsByNameAsync(string name)
		{
			var foundProducts = await _context.Products.Where(p => p.ProductName.Contains(name)).ToListAsync();
			if (ReferenceEquals(foundProducts, null))
			{
				return BadRequest();
			}
			return foundProducts;
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
			return existingProduct;
		}

		[HttpPut("{productId}/quantityChange/{quantity}")]
		public async Task<ActionResult<Product>> UpdateProductQuantityFromPurchase(int productId, int quantity)
		{
			var existingProduct = await _context.Products.FindAsync(productId);
			if (ReferenceEquals(existingProduct, null))
			{
				return NotFound();
			}

			if (quantity > existingProduct.ProductQuantity)
			{
				return BadRequest("Amount entered exceed Inventory"); // Usually this would never happen just as a security feature
			}

            existingProduct.ProductQuantity += quantity;
            await _context.SaveChangesAsync();
            return Ok(existingProduct);
        }

		[HttpGet("filePath/{productId}")]
		public async Task<ActionResult<string>> GetImageFilePathByProductId(int productId)
		{
			var existingProduct = await _context.Products.FindAsync(productId);
			if (ReferenceEquals(existingProduct, null))
			{
				return NotFound();
			}

			return existingProduct.ProductImage;
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
