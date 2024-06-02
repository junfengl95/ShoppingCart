using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopperUI.ApiClient;
using ShopperUI.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace ShopperUI.Controllers
{
	[Authorize]
	public class ProductController : Controller
	{
		private readonly IShopperUIClient _shopperUIClient;

		public ProductController(IShopperUIClient shopperUIClient)
		{
			_shopperUIClient = shopperUIClient;
		}

		[HttpGet]
		public async Task<IActionResult> AllProducts(string searchString)
		{
			var products = await _shopperUIClient.GetAllProductsAsync();

			if (products == null)
			{
				return NotFound();
			}
			else
			{
				if (!string.IsNullOrEmpty(searchString))
				{
					String searchLower = searchString.ToLower();

					products = products.Where(p => p.ProductName.ToLower().Contains(searchLower)).ToList();

					return View("~/Views/ShopWebPage/AllProducts.cshtml", products);

				}
				return View("~/Views/ShopWebPage/AllProducts.cshtml", products);
			}
		}

		[HttpGet("/Product/OneProduct/{productId}")]
		public async Task<IActionResult> OneProduct(int productId)
		{
			
			Product? product = await _shopperUIClient.GetProductByIdAsync(productId);

			if (object.ReferenceEquals(product, null))
			{
				return NotFound();
			}

			return View("~/Views/ShopWebPage/OneProduct.cshtml", product);
		}

		[HttpGet("GetResizedImage")]
		public async Task<IActionResult> GetResizedImageFromDatabase(int productId, int width, int height)
		{
			var product = await _shopperUIClient.GetProductByIdAsync(productId);
			if (product == null || string.IsNullOrEmpty(product.ProductImage))
			{
				return NotFound("Product does not Exist");
			}

			var imagePath = Path.Combine("wwwroot", product.ProductImage.TrimStart('/'));

			if (!System.IO.File.Exists(imagePath))
			{
				return NotFound("File not Found");
			}

			using (var image = Image.Load(imagePath))
			{
				image.Mutate(x => x.Resize(width, height));

				var memoryStream = new MemoryStream();
				image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
				memoryStream.Seek(0, SeekOrigin.Begin);

				return File(memoryStream, "image/png"); // ASP.NET Core will dispose the stream
			}
		}
	}
}