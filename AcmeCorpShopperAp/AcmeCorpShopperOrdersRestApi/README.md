# Orders REST API

The API will be calling the endpoints from the Carts REST API to populate the Order entities.

The CustomerName will be obtained from the User when they submit it as an input.

The API provide the client user to be able to checkout a cart for creating an order.

When creating an order, a customer name should be collected. 
Both the checked out cart and customer name should be assigned to the new order when the order is persisted to the database.
