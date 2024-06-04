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
    public class CartController : Controller
    {
        private readonly IShopperUIClient _shopperUIClient;
        private readonly UserManager<ShopperUIUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(UserManager<ShopperUIUser> userManager, IShopperUIClient shopperUIClient, ILogger<CartController> logger)
        {
            _userManager = userManager;
            _shopperUIClient = shopperUIClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            
            Cart? cart = await _shopperUIClient.GetCartById(user.CartId.Value);

            if (object.ReferenceEquals(cart,null))
            {
                // Reassign a new cart to the user
                Cart? newCart = await _shopperUIClient.CreateNewCart();

                if (newCart != null)
                {
                    user.CartId = newCart.CartId;
					ViewBag.CartId = newCart.CartId;
                    HttpContext.Session.SetInt32("CartId", newCart.CartId);
					return View("~/Views/ShopWebPage/EmptyCart.cshtml", newCart);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
				// Since Cart exist reassign the Values to the ViewModels
				// Create CartItemViewModel
				var cartItems = new List<CartItemViewModel>();

				foreach (var ci in cart.CartItems)
				{
					var product = await _shopperUIClient.GetProductByIdAsync(ci.ProductId);

					if (product != null)
					{
						cartItems.Add(new CartItemViewModel
						{
							CartItemId = ci.CartItemId,
							ProductId = ci.ProductId,
							ProductName = product.ProductName,
							ProductImage = product.ProductImage,
							Quantity = ci.Quantity,
							ProductPrice = product.ProductPrice,
							FKCartId = ci.FkCartId
						});
					}
				}

				var cartViewModel = new CartViewModel
				{
					CartId = cart.CartId,
					CartPrice = cart.CartPrice,
					CartItems = cartItems
				};
				ViewBag.CartId = cart.CartId;
				HttpContext.Session.SetInt32("CartId", cart.CartId);

                return View("~/Views/ShopWebPage/Cart.cshtml", cartViewModel);
            }
        }

		[HttpPost]
		public async Task<IActionResult> AddProductToCart(int productId, int quantity)
		{
			// Retrieve cartId from session state
			var cartId = HttpContext.Session.GetInt32("CartId");

			// Check if cartId is null or not
			if (cartId == null)
			{
                Console.WriteLine("CartId wrong");
				// Handle the scenario where cartId is null (e.g., redirect to an error page or return an error message)
				return RedirectToAction("Error");
			}

			// Call the AddProductToCartAsync method with the retrieved cartId and productId
			var cartItem = await _shopperUIClient.AddProductToCartAsync(cartId.Value, productId, quantity);

			// Check if cartItem is null (e.g., if the API call fails)
			if (cartItem == null)
			{
				Console.WriteLine($"Error with cartId {cartId} and productId:{productId}");
				// Handle the scenario where cartItem is null (e.g., redirect to an error page or return an error message)
				return RedirectToAction("Error");
			}

			// Redirect to GetUserCart action method to view the updated cart
			return RedirectToAction("GetUserCart", "Cart");
		}

        // Instead of HttpDelete substitute with HttpPost
        [HttpPost] 
        public async Task<IActionResult> DeleteProductFromCart(int productId, int quantity)
        {
			var cartId = HttpContext.Session.GetInt32("CartId");

            if (!cartId.HasValue)
            {
                return RedirectToAction("Error");
            }

            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero");
            }

            
			await _shopperUIClient.DeleteProductFromCartAsync(cartId.Value, productId, quantity);


            return RedirectToAction("GetUserCart", "Cart");

        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
			var cartId = HttpContext.Session.GetInt32("CartId");

            if (!cartId.HasValue)
            {
                return RedirectToAction("Error");
            }
            await _shopperUIClient.ClearCart(cartId.Value);

            return RedirectToAction(nameof(GetUserCart));
		}
	}
}
