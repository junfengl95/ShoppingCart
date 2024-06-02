using CartApi.Models;
using CartApi.ProductsApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{

		private readonly CartsContext _context;
		private readonly IProductClient _productClient;

		public CartController(CartsContext context, IProductClient productClient)
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

				//Check if cartItems are included
				if (cart.CartItems != null && cart.CartItems.Any())
				{
					foreach (var cartItem in cart.CartItems)
					{
						_context.CartItems.Add(cartItem);
					}
				}

				await _context.SaveChangesAsync();

				// Track changes to the cart and reload it will new values
				await _context.Entry(cart).ReloadAsync();

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
			if (object.ReferenceEquals(foundCart, null))
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

			return this.Ok(cartsWithItems);
		}

		[HttpPost("{cartId}/add-product/{productId}/{quantity}")]
		public async Task<ActionResult> AddProductToCart(int cartId, int productId, int quantity)
		{
			// Check if cart exist
			var foundCart = await _context.Carts.FindAsync(cartId);
			if (object.ReferenceEquals (foundCart, null))
			{
				return this.NotFound();
			}

			// Check if product exist using Api Clien
			var productExist = await _productClient.CheckProductExistence(productId);
			if (object.Equals(productExist, null))
			{
				return this.NotFound();
			}
		

			var cartitem = new CartItem
			{
				ProductId = productId,
				FkCartId = cartId,
				Quantity = quantity
			};

			_context.CartItems.Add(cartitem);

			foundCart.CartPrice = await CalculateTotalCartPrice(cartId);

			var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, -quantity);
			if (!updateQuantityResult)
			{
				return this.BadRequest("Failed to update product quantity");
			}

			await _context.SaveChangesAsync();

			return this.Ok(foundCart);
		}

		[HttpDelete("{cartId}/remove-product/{productId}/{quantity}")]
		public async Task<ActionResult<bool>> RemoveProductFromCart(int cartId, int productId, [FromQuery] int quantity)
		{
			var foundCartWithItems = await _context.Carts
												   .Include(cart => cart.CartItems)
												   .FirstOrDefaultAsync(cart => cart.CartId == cartId);

			if (object.ReferenceEquals(foundCartWithItems, null))
			{
				return this.NotFound();
			}

			var productInCart = foundCartWithItems.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

			if (object.ReferenceEquals(productInCart, null))
			{
				return this.NotFound();
			}

			if (productInCart.Quantity <= quantity)
			{
				// If the quantity to remove exceeds the quantity in the cart, remove the entire product
				foundCartWithItems.CartItems.Remove(productInCart);
			}
			else
			{
				productInCart.Quantity -= quantity;

                var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, quantity);
                if (!updateQuantityResult)
                {
                    return this.BadRequest("Failed to update product quantity");
                }
            }

			foundCartWithItems.CartPrice = await CalculateTotalCartPrice(cartId);

			await _context.SaveChangesAsync();

			return this.Ok(foundCartWithItems);
		}

		[HttpDelete("clear-items/{cartId}")]
		public async Task<ActionResult<bool>> RemoveAllProductsInCart(int cartId)
		{
			var foundCartWithItems = await _context.Carts
												   .Include(cart => cart.CartItems)
												   .FirstOrDefaultAsync(cart => cart.CartId == cartId);

			if (object.Equals(foundCartWithItems, null))
			{
				return this.NotFound();
			}

			// Store the quanties of products being removed from the cart
			var removedQuantities = new Dictionary<int, int>(); // productId , quantity

			foreach(var cartItem in foundCartWithItems.CartItems)
			{
				if (!removedQuantities.ContainsKey(cartItem.Quantity))
				{
                    // If dict does not contain the product Id to match with Cart no change
                    removedQuantities[cartItem.ProductId] = 0; 
                }

				// If found assign the values to the quantity for the product Id
				removedQuantities[cartItem.ProductId] += cartItem.Quantity;
			}

			foundCartWithItems.CartItems.Clear();

			foundCartWithItems.CartPrice = await CalculateTotalCartPrice(cartId);

			// Update the product quantity
			foreach (var (productId, quantity) in removedQuantities)
			{
				await _productClient.UpdateProductQuantity(productId, quantity);
			}

			await _context.SaveChangesAsync();

			return Ok();
;		}


		private async Task<decimal?> CalculateTotalCartPrice(int cartId)
		{
			var cart = await _context.Carts
									 .Include(cart => cart.CartItems)
									 .FirstAsync(c => c.CartId == cartId);

			if (object.ReferenceEquals(cart, null))
			{
				return 0;
			}

			var productQuantities = cart.CartItems
										.GroupBy(ci => ci.ProductId)
										.ToDictionary(g => g.Key, g => g.Sum(ci => ci.Quantity));

			decimal totalCartPrice = await _productClient.GetTotalProductPrice(productQuantities);

			return totalCartPrice;
		}

		[HttpDelete("{cartId}")]
		public async Task<ActionResult<bool>> DeleteCartById(int cartId)
		{
			var foundCart = await _context.Carts.FindAsync(cartId);

			if (object.Equals(foundCart, null)) 
			{ 
				return this.NotFound(); 
			}
			else
			{
				// Remove associated cart items
				var cartItems = _context.CartItems.Where(ci => ci.ProductId == cartId);
				_context.CartItems.RemoveRange(cartItems);

				// Delete the empty cart
				_context.Remove(foundCart);
				await _context.SaveChangesAsync();
				return this.Ok();
			}

		}


		[HttpGet("checkout/{cartId}")]
		public async Task<ActionResult> CheckoutCart(int cartId)
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
	}
}
