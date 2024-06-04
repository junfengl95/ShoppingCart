using CartApi.Controllers;
using FluentAssertions;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using CartApi.Models;
using CartApi.ProductsApiClient;

namespace CartApi.Test
{
    public class CartControllerTests : IDisposable
    {

        private readonly CartsContext _context;
        private readonly IProductClient _productClient;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _context = GetDbContextAsync();
            _productClient = A.Fake<IProductClient>();
            _controller = new CartController(_context, _productClient);
        }

        private CartsContext GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<CartsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new CartsContext(options);

            // Optionally seed the database with initial data
            SeedDatabase(context);

            return context;
        }

        private static void SeedDatabase(CartsContext context)
        {

            var carts = new List<Cart>
    {
        new Cart { CartId = 1, CartPrice = 30 },
        new Cart { CartId = 2, CartPrice = 50 }
    };

            var cartItems = new List<CartItem>
    {
        new CartItem { CartItemId = 1, ProductId = 1, Quantity = 10, FkCartId = 1 },
        new CartItem { CartItemId = 2, ProductId = 5, Quantity = 20, FkCartId = 2 }
    };

            context.Carts.AddRange(carts);
            context.CartItems.AddRange(cartItems);

            context.SaveChanges();
        }

        [Fact]
        public async void CartController_CreateCartAsync_ReturnCart()
        {
            // Arrange
            var newCart = new Cart
            {
                CartItems = new List<CartItem>
                {
                    new CartItem{ CartItemId = 3, Quantity = 10, ProductId = 5, FkCartId = 3},
                }

            };
            // Act
            var result = await _controller.CreateCartAsync(newCart);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result.Result as CreatedAtActionResult;

            createdAtResult.Should().NotBeNull();
            createdAtResult.ActionName.Should().Be("ReadCartById");
            createdAtResult.RouteValues["cartId"].Should().Be(newCart.CartId);
            createdAtResult.Value.Should().BeEquivalentTo(newCart);
            createdAtResult.Value.Should().BeOfType<Cart>();

            var createdCart = createdAtResult.Value as Cart;

            createdCart.CartItems.Should().HaveCount(1);
            createdCart.CartItems.First().ProductId.Should().Be(5);

            var cartInDb = await _context.Carts.FindAsync(newCart.CartId);
            cartInDb.Should().NotBeNull();
            cartInDb.Should().BeEquivalentTo(newCart);
        }

        [Fact]
        public async void CartController_ReadAllCartsAsync_ReturnCarts()
        {
            // Arrange

            // Act
            var result = await _controller.ReadAllCartsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();

            var okObjectResult = result.Result as OkObjectResult;
            okObjectResult.Should().NotBeNull();

            okObjectResult.Value.Should().NotBeNull().And.BeAssignableTo<IEnumerable<Cart>>();

            var returnedCarts = okObjectResult.Value as IEnumerable<Cart>;
            returnedCarts.Should().HaveCount(2); // Adjust count as per your seeded data

            foreach (var cart in returnedCarts)
            {
                cart.CartItems.Should().NotBeNull(); // Ensure CartItems are included
                cart.CartPrice.Should().BeGreaterThanOrEqualTo(0); // Ensure CartPrice is calculated
            }
        }

        [Fact]
        public async Task CartController_UpdateProductQuantity_AddProductDoesNotExistInCart()
        {
            // Arrange
            int cartId = 1;
            int productId = 2; // A product ID not present in the cart
            int quantity = 5;

            var expectedCartItem = new CartItem
            {
                ProductId = productId,
                FkCartId = cartId,
                Quantity = quantity
            };

            A.CallTo(() => _productClient.CheckProductExistence(productId)).Returns(true);
            A.CallTo(() => _productClient.UpdateProductQuantity(productId, -quantity)).Returns(true);

            // Act
            var result = await _controller.UpdateProductQuantityInCart(cartId, productId, quantity);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedCart = okResult.Value as Cart;
            returnedCart.Should().NotBeNull();

            var cartItemInDb = await _context.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.FkCartId == cartId);
            cartItemInDb.Should().NotBeNull();
            cartItemInDb.Should().BeEquivalentTo(expectedCartItem, options => options.Excluding(ci => ci.CartItemId)
                                                                                     .Excluding(ci => ci.FkCart));
            // To prevent cyclic error as we did not include the the JsonPropertyHandlers in this DbContext but is present in the Program
        }

        [Fact]
        public async void CartController_UpdateProductQuantity_AddProductWhenExistInCart()
        {
            // Arrange
            int cartId = 2;
            int productId = 5; // A product ID already present in the cart
            int quantity = 5;



            A.CallTo(() => _productClient.CheckProductExistence(productId)).Returns(true);
            A.CallTo(() => _productClient.UpdateProductQuantity(productId, -quantity)).Returns(true);

            // Act
            var result = await _controller.UpdateProductQuantityInCart(cartId, productId, quantity);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedCart = okResult.Value as Cart;
            returnedCart.Should().NotBeNull();

            // Check that the cartIteminDb is updated correctly 
            var cartItemInDb = _context.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.FkCartId == cartId);
            cartItemInDb.Should().NotBeNull();
            cartItemInDb.Quantity.Should().Be(25);

        }

        [Fact]
        public async void CartController_UpdateProductQuantity_RemoveProductWhenExistInCart()
        {
            {
                // Arrange
                int cartId = 1;
                int productId = 1; // A product ID already present in the cart
                int quantity = 5;

                A.CallTo(() => _productClient.CheckProductExistence(productId)).Returns(true);
                A.CallTo(() => _productClient.UpdateProductQuantity(productId, -quantity)).Returns(true);


                // Act
                var result = await _controller.UpdateProductQuantityInCart(cartId, productId, -quantity);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedCart = okResult.Value as Cart;
                returnedCart.Should().NotBeNull();

                // Check that the cartItemInDb is updated correctly 
                var cartItemInDb = _context.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.FkCartId == cartId);
                cartItemInDb.Should().NotBeNull();
                cartItemInDb.Quantity.Should().Be(5); // Ensure the quantity is updated correctly
            }
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
