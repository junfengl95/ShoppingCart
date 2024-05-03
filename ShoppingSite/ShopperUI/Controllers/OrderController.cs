using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopperUI.ApiClient;
using ShopperUI.Areas.Identity.Data;
using ShopperUI.Models;

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

		//[HttpPost]
		//public async Task<IActionResult> CheckoutCart(Order order)
		//{
		//	var cartId = HttpContext.Session.GetInt32("CartId");

		//	if (cartId == null)
		//	{
		//		Console.WriteLine("CartId wrong");
		//		// Handle the scenario where cartId is null (e.g., redirect to an error page or return an error message)
		//		return RedirectToAction("Error");
		//	}

		//	Cart? cart = await _shopperUIClient.GetCartById(cartId.Value);

		//	if (cart == null)
		//	{
		//		return NotFound();
		//	}

		//	// Created the Order after verifying the Cart exist

		//	order.CartId = cartId.Value;

		//	var createdOrder = await _shopperUIClient.CreateNewOrder(order);

		//	if (createdOrder != null)
		//	{
		//		createdOrder.TotalPrice = cart.CartPrice;

		//		return RedirectToAction("Orders", new { order = createdOrder.OrderId });
		//	}

		//}
	}
}
