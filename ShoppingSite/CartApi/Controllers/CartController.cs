using CartApi.Models;
using CartApi.ProductsApiClient;
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

			return Ok(cartsWithItems);
		}

		[HttpPost("{cartId}/update-product/{productId}/quantity/{quantity}")]
		public async Task<ActionResult> UpdateProductQuantityInCart(int cartId, int productId, int quantity)
		{
			// Check if quantity is zero
			if (quantity == 0)
			{
				return BadRequest("Quantity must be non-zero");
			}

			// Check if cart exist
			var foundCart = await _context.Carts.FindAsync(cartId);
			if (foundCart == null) { return this.NotFound("Cart not found"); }

			// Check if productExist
			var productExist = await _productClient.CheckProductExistence(productId);
			if (!productExist) { return this.NotFound("Product not found"); }

			var cartItem = await CheckIfCartItemContainProduct(productId, cartId);

			if (quantity > 0) // adding product to Cart
			{
				if (cartItem == null) // No entry for that cart and product product exist
				{
					var newCartItem = new CartItem
					{
						ProductId = productId,
						FkCartId = cartId,
						Quantity = quantity,
					};

					_context.CartItems.Add(newCartItem);
				}
				else
				{
					// Update quantity in the CartItem
					cartItem.Quantity += quantity;
					_context.CartItems.Update(cartItem);
				}

				var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, quantity);
				if (!updateQuantityResult) { return this.BadRequest(" Failed to update product quantity"); }
			}
			else // Removing product if quantity negative
			{
				if (cartItem == null) { return this.NotFound("Cart Item not found"); }

				// If the quantity to remove is greater or equal to the quantity in the cart remove the entire product
				if (cartItem.Quantity <= -quantity)
				{
					_context.CartItems.Remove(cartItem);

					var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, -cartItem.Quantity);
					if (!updateQuantityResult) { return this.BadRequest("Failed to update the product quantity"); }
				}
				else
				{
					// Reduce the quantity in the CartItem
					cartItem.Quantity += quantity; // Since the quantity is negative,
					_context.CartItems.Update(cartItem);

                    var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, -cartItem.Quantity);
                    if (!updateQuantityResult) { return this.BadRequest("Failed to update the product quantity"); }
                }
			}

			foundCart.CartPrice = await CalculateTotalCartPrice(cartId);

			await _context.SaveChangesAsync();

			return Ok(foundCart);
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


        private async Task<CartItem> CheckIfCartItemContainProduct(int productId, int cartId)
        {
            var cartItem = await _context.CartItems
                                 .Where(ci => ci.ProductId == productId && ci.FkCartId == cartId)
                                 .FirstOrDefaultAsync();
            return cartItem;
        }
    }
}

//[HttpPost("{cartId}/add-product/{productId}/{quantity}")]
//public async Task<ActionResult> AddProductToCart(int cartId, int productId, int quantity)
//{
//	if (quantity <= 0)
//	{
//              return BadRequest("Quantity must be greater than zero.");
//          }


//	// Check if cart exist
//	var foundCart = await _context.Carts.FindAsync(cartId);
//	if (foundCart == null)
//	{
//		return this.NotFound();
//	}

//	// Check if product exist using Api Client
//	var productExist = await _productClient.CheckProductExistence(productId);
//	if (!productExist)
//	{
//		return this.NotFound();
//	}

//          var cartItemWithProduct = await CheckIfCartItemContainProduct(productId, cartId);

//	if (cartItemWithProduct == null) // No entry for that cart and product exist yet
//	{
//              var cartitem = new CartItem
//              {
//                  ProductId = productId,
//                  FkCartId = cartId,
//                  Quantity = quantity
//              };

//              _context.CartItems.Add(cartitem);
//          }
//	else
//	{
//              // Update quantity in the CartItem
//              cartItemWithProduct.Quantity += quantity;
//		_context.CartItems.Update(cartItemWithProduct);
//	}

//	foundCart.CartPrice = await CalculateTotalCartPrice(cartId);

//	var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, -quantity);
//	if (!updateQuantityResult)
//	{
//		return this.BadRequest("Failed to update product quantity");
//	}

//	await _context.SaveChangesAsync();

//	return this.Ok(foundCart);
//}



//      [HttpDelete("{cartId}/remove-product/{productId}/{quantity}")]
//public async Task<ActionResult<bool>> RemoveProductFromCart(int cartId, int productId, [FromQuery] int quantity)
//{
//	var foundCartWithItems = await _context.Carts
//										   .Include(cart => cart.CartItems)
//										   .FirstOrDefaultAsync(cart => cart.CartId == cartId);

//	if (foundCartWithItems == null)
//	{
//		return this.NotFound();
//	}

//          var cartItem = CheckIfCartItemContainProduct(productId, cartId);

//          // If the quantity to Remove is greater than Quantity in Cart
//          if (cartItem.Result.Quantity <= quantity)
//	{
//		// If the quantity to remove exceeds the quantity in the cart, remove the entire product
//		foundCartWithItems.CartItems.Remove(cartItem.Result);

//              var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, cartItem.Result.Quantity);
//              if (!updateQuantityResult)
//              {
//                  return this.BadRequest("Failed to update product quantity");
//              }
//          }
//	else
//	{
//              // If the quantity to Remove is less than the Quantity in Cart
//              cartItem.Result.Quantity -= quantity;

//              var updateQuantityResult = await _productClient.UpdateProductQuantity(productId, quantity);
//              if (!updateQuantityResult)
//              {
//                  return this.BadRequest("Failed to update product quantity");
//              }
//              _context.CartItems.Update(cartItem.Result);
//          }

//	foundCartWithItems.CartPrice = await CalculateTotalCartPrice(cartId);

//	await _context.SaveChangesAsync();

//	return this.Ok(foundCartWithItems);
//}
