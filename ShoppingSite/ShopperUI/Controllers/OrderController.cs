using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopperUI.ApiClient;
using ShopperUI.Areas.Identity.Data;
using ShopperUI.Models;
using ShopperUI.ViewModels;

namespace ShopperUI.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IShopperUIClient _shopperUIClient;
		private readonly UserManager<ShopperUIUser> _userManager;

		public OrderController(IShopperUIClient shopperUIClient, UserManager<ShopperUIUser> userManager)
		{
			_shopperUIClient = shopperUIClient;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> AllOrders()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return Unauthorized();
			}

			var orders = await _shopperUIClient.GetAllOrders(user.Id);

			if (orders == null)
			{
				return NotFound();
			}

			return View("~/Views/ShopWebPage/AllOrders.cshtml", orders);
		}

		[HttpGet]
		public async Task<IActionResult> OrderDetails(int orderId)
		{
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Fetch order details 
            Order? order = await _shopperUIClient.GetOrderByIdAsync(orderId);

			if (order == null)
			{
				return NotFound();
			}

			Cart? foundCart = await _shopperUIClient.GetCartById(order.CartId);

			if(foundCart == null)
			{
				return NotFound();
			}


			var orderView = new OrderViewModel()
			{
				OrderId = orderId,
				DateOfCreation = order.DateOfCreation,
				CartId = foundCart.CartId,
				TotalPrice = foundCart.CartPrice,
				CartItems = foundCart.CartItems,
				UserId = user.Id
			};


			await _shopperUIClient.ClearCart(foundCart.CartId);


			return View("~/Views/ShopWebPage/OrderDetails.cshtml", orderView);
		}

		[HttpPost]
		public async Task<IActionResult> CheckoutCart()
		{
			var cartId = HttpContext.Session.GetInt32("CartId");

			if (cartId == null)
			{
				Console.WriteLine("CartId wrong");
				// Handle the scenario where cartId is null (e.g., redirect to an error page or return an error message)
				return RedirectToAction("Error");
			}

			Cart? cart = await _shopperUIClient.GetCartById(cartId.Value);

			if (cart == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return Unauthorized();
			}

			// Created the Order after verifying the Cart exist

			var createdOrder = await _shopperUIClient.CreateNewOrder(cartId.Value, user.Id, cart.CartPrice);

			if (createdOrder != null)
			{
				createdOrder.TotalPrice = cart.CartPrice;

				//Console.WriteLine($"Total price : {cart.CartPrice}");

				createdOrder.CustomerId = user.Id;

				//Console.WriteLine($"UserId : {user.Id}");

				user.Orders.Add(createdOrder);

				return RedirectToAction("AllOrders", "Order");
			}

			else
			{
                Console.WriteLine($"Failed to create order form CartId: {cartId}");
                ModelState.AddModelError(string.Empty, "Failed to create order. Please try again later.");
                return View("~/Views/ShopWebPage/AllOrders.cshtml"); // Return to the same view with error messages
            }
		}
	}
}
