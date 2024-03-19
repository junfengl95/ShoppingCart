using AcmeCorpShopperUIWebApp.ApiClient;
using AcmeCorpShopperUIWebApp.Models;
using AcmeCorpShopperUIWebApp.ModelView;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpShopperUIWebApp.Controllers
{
	public class AcmeCorpShopperController : Controller
	{
		private readonly IAcmeCorpClient _acmeCorpClient;

		public AcmeCorpShopperController(IAcmeCorpClient acmeCorpClient)
		{
			_acmeCorpClient = acmeCorpClient;
		}

		[HttpGet] // By default HttpGet unless specified
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> AllProducts()
		{
			var products = await _acmeCorpClient.GetAllProductsAsync();

			return View(products);
		}

		[HttpGet]
		public async Task<IActionResult> OneProduct(int id)
		{

			if (HttpContext.Request.Cookies.TryGetValue("CartId", out string cartId))
			{
				ViewBag.CartId = cartId;
			}
			else
			{
				ViewBag.CartId = null;
			}

			Product? product = await _acmeCorpClient.GetProductByIdAsync(id);

			if (object.ReferenceEquals(product, null))
			{
				return NotFound();
			}

			return View(product);
		}


		// 
		[HttpGet]
		public async Task<IActionResult> Cart()
		{
			int? cartId = int.TryParse(HttpContext.Request.Cookies["CartId"], out int parsedCartId) ? parsedCartId : null;

			if (cartId != null)
			{
				// Check if the cartId is stored in cookie exist in the database
				Cart? cart = await _acmeCorpClient.RetrieveCartAsync((int)cartId);

				if (!object.ReferenceEquals(cart, null))
				{
					return View(cart);
				}
				else
				{
					// Clear the cookie if no cart found
					Response.Cookies.Delete("CartId");

					cart = await _acmeCorpClient.CreateNewCart();

					Response.Cookies.Append("CartId", cart.CartId.ToString());

					return View(cart);
				}
			}
			else
			{
				// Create a new cart if the CartId cookie is not set
				Cart? cart = await _acmeCorpClient.CreateNewCart();

				// Set the CartId cookie with the ID of the new cart
				Response.Cookies.Append("CartId", cart.CartId.ToString());

				// Return the view with the newly created cart model
				return View(cart);
			}
		}

		//[HttpGet]
		//public async Task<IActionResult> Cart()
		//{
		//    // Retrieve the cartId from the cookie
		//    int? cartId = int.TryParse(HttpContext.Request.Cookies["CartId"], out int parsedCartId) ? parsedCartId : null;

		//    // Check if the cartId is null or not
		//    if (cartId != null)
		//    {
		//        // Retrieve the cart from the client API
		//        Cart? cart = await _acmeCorpClient.RetrieveCartAsync((int)cartId);

		//        // If the cart exists, return the view with the cart model
		//        if (cart != null)
		//        {
		//            List<CartItem> cartItems = new List<CartItem>();
		//            foreach (var item in cart.CartItems)
		//            {
		//                cartItems.Add(item);
		//            }

		//            //Map cart items to view Model
		//            List<CartItemDetails> cartItemDetailsList = cartItems.Select(ci => new CartItemDetails
		//            {
		//                CartItemId = ci.CartItemId,
		//                ProductId = ci.ProductId,
		//                ProductName = ci.Product.ProductName,
		//                ProductPrice = ci.Product.ProductPrice
		//            }).ToList();

		//            CartItemViewModel viewModel = new CartItemViewModel
		//            {
		//                CartId = cart.CartId,
		//                CartPrice = cart.CartPrice,
		//                CartItems = cartItemDetailsList
		//            };

		//            return View(viewModel);
		//        }
		//        else
		//        {
		//            // Clear the invalid cartId cookie and create a new cart
		//            ClearAndCreateNewCart();
		//        }
		//    }
		//    else
		//    {
		//        // If the cartId cookie is not set, create a new cart
		//        ClearAndCreateNewCart();
		//    }

		//    return View();
		//}

		//private async Task ClearAndCreateNewCart()
		//{
		//    // Clear the cartId cookie
		//    Response.Cookies.Delete("CartId");

		//    // Create a new cart
		//    Cart? cart = await _acmeCorpClient.CreateNewCart();

		//    // Set the CartId cookie with the ID of the new cart
		//    Response.Cookies.Append("CartId", cart.CartId.ToString());
		//}

		[HttpPost]
		public async Task<IActionResult> AddProductToCart(int cartId, int productId)
		{
			var cartItem = await _acmeCorpClient.AddProductToCartAsync(cartId, productId);

			return RedirectToAction("Cart", "AcmeCorpShopper");
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteProductFromCart(int cartId, int productId)
		{
			await _acmeCorpClient.DeleteProductFromCartAsync(cartId, productId);

			//await Console.Out.WriteLineAsync($"product: {productId} deleted from cart: {cartId}");

			return RedirectToAction("Cart", "AcmeCorpShopper");
		}

		[HttpDelete]
		public async Task<IActionResult> ClearCart(int cartId)
		{
			await _acmeCorpClient.ClearCart(cartId);

			return RedirectToAction(nameof(Cart));
		}



		public IActionResult CheckForCookies()
		{
			if (HttpContext.Request.Cookies.TryGetValue("CartId", out string cartId))
			{
				ViewBag.CartId = cartId;
			}
			else
			{
				ViewBag.CartId = null;
			}
			return View();
		}
	}
}
