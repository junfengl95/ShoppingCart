using AcmeCorpShopperCartsRestApi.ApiClient;
using AcmeCorpShopperCartsRestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorpShopperCartsRestApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly CartsAcmeContext _context;
		private readonly IProductClient _productClient;

		public CartController(CartsAcmeContext context, IProductClient productClient)
		{
			_context = context;
			_productClient = productClient;
		}

		[HttpPost]
		public async Task<ActionResult<Cart>> CreateCartAsync([FromBody] Cart cart)
		{
			try
			{
				_context.Carts.Add(cart);

				// Check if cartItems are included in the request
				if (cart.CartItems != null && cart.CartItems.Any())
				{
					foreach (var cartItem in cart.CartItems)
					{
						_context.CartItems.Add(cartItem);
					}
				}

				await _context.SaveChangesAsync();

				// Ensure that CartId is properly assigned after saving changes
				// Reload the cart from the context to get the updated CartId
				await _context.Entry(cart).ReloadAsync();

				// Log route values
				Console.WriteLine($"Route values: cartId = {cart.CartId}");

				// Return the created cart
				return CreatedAtAction("ReadCartById", new { cartId = cart.CartId }, cart);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{cartId}")]
		public async Task<ActionResult<Cart>> ReadCartByIdAsync(int cartId)
		{
			var foundCart = await _context.Carts.FindAsync(cartId);
			if (object.Equals(foundCart, null))
			{
				return this.NotFound();
			}

			var foundCartwithItems = await _context.Carts
												   .Include(cart => cart.CartItems)
												   .FirstOrDefaultAsync(cart => cart.CartId == cartId);

			return this.Ok(foundCartwithItems);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Cart>>> ReadAllCartsAsync()
		{
			var cartsWithItems = await _context.Carts
										   .Include(cart => cart.CartItems)
										   .ToListAsync();

			foreach (var cart in cartsWithItems)
			{
				cart.CartPrice = await CalculateTotalCartPrice(cart.CartId);
				await _context.SaveChangesAsync();
			}

			//return this.Ok(await _context.Carts.ToListAsync());
			return this.Ok(cartsWithItems);
		}


		[HttpPost("{cartId}/add-product/{productId}")]
		public async Task<IActionResult> AddProdcutToCart(int cartId, int productId)
		{
			// check if cart exist first
			var foundCart = await _context.Carts.FindAsync(cartId);
			if (object.Equals(foundCart, null))
			{
				return this.NotFound();
			}
			// Check if product exist using API Client
			var productExist = await _productClient.CheckProductExistence(productId);

			if (!productExist)
			{
				return this.NotFound();
			}

			var cartItem = new CartItem
			{
				ProductId = productId,
				FkCartId = cartId,
			};

			_context.CartItems.Add(cartItem);

			foundCart.CartPrice = await CalculateTotalCartPrice(cartId);

			await _context.SaveChangesAsync();

			return this.Ok(foundCart);
		}



		private async Task<decimal?> CalculateTotalCartPrice(int cartId)
		{
			var cart = await _context.Carts
									 .Include(c => c.CartItems)
									 .FirstAsync(c => c.CartId == cartId);

			if (cart == null)
			{
				return 0; // No items so total price is 0
			}

			var productIds = cart.CartItems.Select(ci => ci.ProductId).ToList();

			decimal totalCartPrice = await _productClient.GetTotalProductPrice(productIds);

			return totalCartPrice;
		}

		[HttpDelete("{cartId}/remove-product/{productId}")]
		public async Task<ActionResult<bool>> RemoveProductFromCart(int cartId, int productId)
		{
			var foundCartwithItems = await _context.Carts
												   .Include(cart => cart.CartItems)
												   .FirstOrDefaultAsync(cart => cart.CartId == cartId);

			if (object.ReferenceEquals(foundCartwithItems, null))
			{
				return this.NotFound();
			}

			Console.WriteLine($"Number of cart items in foundCart: {foundCartwithItems.CartItems.Count}");

			var productInCart = foundCartwithItems.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

			if (!object.ReferenceEquals(productInCart, null))
			{
				//_context.Entry(productInCart).State = EntityState.Deleted;

				foundCartwithItems.CartItems.Remove(productInCart);
				//productInCart.FkCart = null!;
				foundCartwithItems.CartPrice = await CalculateTotalCartPrice(cartId);
				await _context.SaveChangesAsync();

				// Log whether the removal was successful
				//Console.WriteLine($"Successful removal: {successfulRemove}");
			}
			else
			{
				return NotFound();
			}

			return this.Ok();
		}

		[HttpDelete("clear-items/{cartId}")]
		public async Task<ActionResult<bool>> RemoveAllProductInCart(int cartId)
		{
			var foundCartwithItems = await _context.Carts
												   .Include(cart => cart.CartItems)
												   .FirstOrDefaultAsync(cart => cart.CartId == cartId);

			if (!object.Equals(foundCartwithItems, null))
			{
				foundCartwithItems.CartItems.Clear();
				foundCartwithItems.CartPrice = await CalculateTotalCartPrice(cartId);
				await _context.SaveChangesAsync();
			}
			else
			{
				return this.NotFound();
			}

			return this.Ok();
		}

		[HttpDelete("{cartId}")]
		public async Task<ActionResult<bool>> DeleteCartById(int cartId)
		{

			var foundCartwithItems = await _context.Carts.FindAsync(cartId);
												   

			if (object.ReferenceEquals(foundCartwithItems, null))
			{
				return this.NotFound();
				
			}

			_context.Remove(foundCartwithItems);
			await _context.SaveChangesAsync();

			return this.Ok();
		}
	}
}
