using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Controllers;
using ProductApi.Models;

namespace ProductApi.Test.ControllerTest
{
    public class ProductControllerTests
    {
        // Mock the DbContext
        private async Task<ProductsContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ProductsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new ProductsContext(options);
            if (await databaseContext.Products.CountAsync() <= 0)
            {
                // if the db if has zero values 
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Products.Add(
                        new Product()
                        {
                            ProductName = "FakeProduct",
                            ProductPrice = 100.50M, // M suffix to treat as decimial not float
                            ProductQuantity = 50,
                            ProductRating = 3.0M,
                            ProductDescription = "productDescription",
                            ProductCategory = "Category",
                            ProductImage = "ShoppingSite\\ProductImages\\Image.jpg"
                        });
                    await databaseContext.SaveChangesAsync();
                }

                var newProduct = new Product()
                {

                    ProductName = "UniqueProduct",
                    ProductPrice = 50.50M, // M suffix to treat as decimial not float
                    ProductQuantity = 50,
                    ProductRating = 4.0M,
                    ProductDescription = "uniqueProductDescription",
                    ProductCategory = "UniqueCategory",
                    ProductImage = "ShoppingSite\\ProductImages\\Unique.jpg"
                };
                databaseContext.Products.Add(newProduct);
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void ProductController_ReadAllProductsAsync_ReturnProducts()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var controller = new ProductController(dbContext);

            // Act
            var result = await controller.ReadAllProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().HaveCount(11);
            result.Value.Should().AllBeOfType<Product>();

            // Additional test to check contents
            result.Value.Should().Contain(product => product.ProductName == "FakeProduct");
            result.Value.Should().Contain(product => product.ProductPrice == 100.50M);
        }

        [Fact]
        public async void ProductController_ReadProductByIdAsync_ReturnProduct()
        {
            // Arrange
            var productId = 3;
            var dbContext = await GetDbContextAsync();
            var controller = new ProductController(dbContext);

            // Act
            var result = await controller.ReadProductByIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<Product>();
        }

        [Fact]
        public async void ProductController_CreateProduct_ReturnCreatedAtAction()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var controller = new ProductController(dbContext);
            var newProduct = new Product()
            {
                ProductName = "UniqueProduct",
                ProductPrice = 50.50M, // M suffix to treat as decimial not float
                ProductQuantity = 50,
                ProductRating = 4.0M,
                ProductDescription = "uniqueProductDescription",
                ProductCategory = "UniqueCategory",
                ProductImage = "/ProductImages/Unique.jpg"
            };

            // Act
            var result = await controller.CreateProductAsync(newProduct);


            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();

            var createdAtActionResult = result.Result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
        
            createdAtActionResult.Value.Should().BeEquivalentTo(newProduct);
            createdAtActionResult.ActionName.Should().Be("ReadProductById");
            createdAtActionResult.RouteValues["productId"].Should().Be(newProduct.ProductId);

            var productInDb = await dbContext.Products.FindAsync(newProduct.ProductId);
            productInDb.Should().NotBeNull();
            productInDb.Should().BeEquivalentTo(newProduct);
        }


        [Fact]
        public async void ProductController_ReadProductsByNameAsync_ReturnProducts()
        {
            // Arrange 
            var dbContext = await GetDbContextAsync();
            var controller = new ProductController(dbContext);
            var name = "Unique";


            // Act
            var result = await controller.ReadProductsByNameAsync(name);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().AllBeOfType<Product>();
            result.Value.Should().Contain(product => product.ProductName.Contains(name));
        }

        [Fact]
        public async void ProductController_UpdateProductQuantityFromPurchase_ReturnMoreProduct()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var controller = new ProductController(dbContext);
            var quantity = 5;
            var productId = 11;


            // Act
            var result = await controller.UpdateProductQuantityFromPurchase(productId, quantity);

            // Assert
            result.Should().NotBeNull();

            var actionResult = result.Result;
            actionResult.Should().BeOfType<OkObjectResult>();

            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();

            var updatedProduct = okResult.Value as Product;
            updatedProduct.Should().NotBeNull();
            updatedProduct.ProductQuantity.Should().Be(50 + quantity);

            // Check for change in Mock db
            var productInDb = await dbContext.Products.FindAsync(productId);
            productInDb.Should().NotBeNull();
            productInDb.ProductQuantity.Should().Be(50 + quantity);
        }

        [Fact]
        public async void ProductController_UpdateProductQuantityFromPurchase_ReturnLessProduct()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var controller = new ProductController(dbContext);
            var quantity = -5;
            var productId = 1;


            // Act
            var result = await controller.UpdateProductQuantityFromPurchase(productId, quantity);

            // Assert
            result.Should().NotBeNull();

            var actionResult = result.Result;
            actionResult.Should().BeOfType<OkObjectResult>();

            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();

            var updatedProduct = okResult.Value as Product;
            updatedProduct.Should().NotBeNull();
            updatedProduct.ProductQuantity.Should().Be(50 + quantity);

            // Check for change in Mock db
            var productInDb = await dbContext.Products.FindAsync(productId);
            productInDb.Should().NotBeNull();
            productInDb.ProductQuantity.Should().Be(50 + quantity);
        }
    }
}



    
