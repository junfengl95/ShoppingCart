using AcmeCorpShopperUIWebApp.ApiClient;
using AcmeCorpShopperUIWebApp.Models;
using AcmeCorpShopperUIWebApp.ViewModels;
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

			await Console.Out.WriteLineAsync($"product: {productId} deleted from cart: {cartId}");

			return RedirectToAction("Cart", "AcmeCorpShopper");
		}

		[HttpDelete]
		public async Task<IActionResult> ClearCart(int cartId)
		{
			await _acmeCorpClient.ClearCart(cartId);

			return RedirectToAction(nameof(Cart));
		}

		[HttpPost]
		public IActionResult BeforeCreation()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateOrder([Bind("CustomerName")] Order order)
		{
			try
			{
				// Check if CustomerName is null or empty
				if (string.IsNullOrEmpty(order.CustomerName))
				{
					ModelState.AddModelError(nameof(order.CustomerName), "Customer name is required.");
					return View(order); // Return to the same view with error messages
				}

				// Fetch the cartId from the cookie
				int.TryParse(HttpContext.Request.Cookies["CartId"], out int cartId);

				// Prepare the order object
				order.CartId = cartId;

				// Call the client method to create a new order
				var createdOrder = await _acmeCorpClient.CreateNewOrder(order);

				if (createdOrder != null && createdOrder.OrderId != 0)
				{
					Console.WriteLine($"Order created for customer: {order.CustomerName}, CartId: {cartId}");

					// Redirect to the OrderDetails action with the newly created order's ID
					return RedirectToAction("OrderDetails", new { orderId = createdOrder.OrderId }); //replaced createdOrder with orderView
				}
				else
				{
					// Handle case where order creation failed
					Console.WriteLine($"Failed to create order for customer: {order.CustomerName}, CartId: {cartId}");
					ModelState.AddModelError(string.Empty, "Failed to create order. Please try again later.");
					return View(order); // Return to the same view with error messages
				}
			}
			catch (Exception ex)
			{
				// Log any exceptions that occur during order creation
				Console.WriteLine($"An error occurred while creating the order: {ex.Message}");
				ModelState.AddModelError(string.Empty, "An error occurred while creating the order. Please try again later.");
				return View(order); // Return to the same view with error messages
			}
		}



		[HttpGet]
		public async Task<IActionResult> OrderDetails(int orderId)
		{
			// Fetch order details from the backend using orderId
			Order? order = await _acmeCorpClient.GetOrderByIdAsync(orderId);

			if (order == null)
			{
				// If order is not found, return a 404 Not Found status
				return NotFound();
			}

			Cart foundCart = await _acmeCorpClient.GetCartById(order.CartId);

			var orderView = new OrderViewModel()
			{
				OrderId = order.OrderId,
				CustomerName = order.CustomerName,
				CartId = order.CartId,
				TotalPrice = foundCart.CartPrice,
				CartItems = foundCart.CartItems
			};


			return View(orderView);
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
