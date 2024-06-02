# ShoppingCart
A project on creating a Shopping Cart using C#, SQL .NETAPI endpoints.
The project was created using .NET version 7 on the Microsoft Visual Studios 2022

The project consists of three components
1. Products REST API
2. Carts REST API
3. Orders REST API
4. Shopper UI Client

## Products REST API
This API provides services of Product resources that will persist to a Sql Server Database named Products

## Carts REST API
This API provides services of Cart resources that will persist to a Sql Server Database named Carts

The database will contain two tables
- Carts
- CartItems

## Orders REST API
This API provide services of Order resources that will persist to a Sql Server Database named Orders

## Shopper UI Client
This is a C# ASP.NET Core Web App (Model-View-Controller) project

This app provides a web-based display catalogue listing of available products to purchase by a client user.
Available products are the product resources managed by the Porduct REST API.

The client user will be adding and removing products from a cart which is managed by the CART REST API.

## Screenshots

### AllProducts Page
![All Products](ShoppingSite/MVC%20Screenshots/AllProduct.png)


### ProductDetails Page
![All Products](ShoppingSite/MVC%20Screenshots/Product%20Details.png)


For more details of each component refer to their individual README.md 
