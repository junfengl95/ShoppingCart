# Cart REST API

The API will be calling the endpoints from the Product REST API to populate the Cart entities.

A Cart type will contain the following Properties:
- CartId
- A collection of product items
- Total price of these contained products

A cart can contain many products and a product can be found in many carts.
When a product is added to a cart it is persisted as a cart item.

All carts and cart items are persisted to the database.

The API will provide the following services:
- CreateCart
- ReadAllCart
- ReadCartById
- AddProductToCart
- RemoveProductFromCart
- ClearCart
- DeleteCart
