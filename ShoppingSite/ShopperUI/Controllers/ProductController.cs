using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopperUI.ApiClient;
using ShopperUI.Models;

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
			//productId = 50001;
			Product? product = await _shopperUIClient.GetProductByIdAsync(productId);

			if (object.ReferenceEquals(product, null))
			{
				return NotFound();
			}

			return View("~/Views/ShopWebPage/OneProduct.cshtml", product);
		}
	}
}