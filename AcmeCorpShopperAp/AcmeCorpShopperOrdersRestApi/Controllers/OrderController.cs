using AcmeCorpShopperOrdersRestApi.ApiClient;
using AcmeCorpShopperOrdersRestApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcmeCorpShopperOrdersRestApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly OrdersAcmeContext _context;
		private readonly ICartClient _cartClient;

		public OrderController(OrdersAcmeContext context, ICartClient cartClient)
		{
			_context = context;
			_cartClient = cartClient;
		}

		[HttpPost]
		public async Task<ActionResult<Order>> CreateOrderAsync([FromBody] Order order)
		{
			try
			{	
				
				// check if any details are null or missing
				if (!string.IsNullOrEmpty(order.CustomerName) && order.CartId !=0)
				{
					_context.Orders.Add(order);
					await _context.SaveChangesAsync();
					return this.Ok(order);
				}
				return this.NotFound();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
		

		[HttpGet("{orderId}")]
			public async Task<ActionResult<Order>> ReadOrderByIdAsync(int orderId)
			{
				var foundOrder = await _context.Orders.FindAsync(orderId);
				if (object.ReferenceEquals(foundOrder, null))
				{
					return this.NotFound();
				}

				return this.Ok(foundOrder);
			}
		}
	}
