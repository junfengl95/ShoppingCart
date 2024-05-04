using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersApi.ApiClient;
using OrdersApi.Models;

namespace OrdersApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly OrdersContext _context;
		private readonly ICartClient _cartClient;

		public OrderController(OrdersContext orderContext, ICartClient cartClient)
		{
			_context = orderContext;
			_cartClient = cartClient;
		}

		[HttpPost]
		public async Task<ActionResult<Order>> CreateOrderAsync([FromBody] Order order)
		{
			try
			{
				// check if any details are null or missing
				if (order.CartId != 0)
				{
					_context.Orders.Add(order);
					await _context.SaveChangesAsync();
					return this.Ok(order);
				}
				return BadRequest("Invalid CartId");
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

		[HttpGet("user/{customerId}")]
		public async Task<ActionResult<IEnumerable<Order>>> ReadAllOrdersForCustomer(string customerId)
		{
			var orders = await _context.Orders.Where(order => order.CustomerId ==customerId).ToListAsync();

			if (orders == null)
			{
				return NotFound();
			}

			return this.Ok(orders);
		}

		[HttpGet("AllOrders")]
		public async Task<ActionResult<IEnumerable<Order>>> ReadAllOrders()
		{
			var orders = await _context.Orders.ToListAsync();

			if (orders == null)
			{
				return NotFound();
			}

			return this.Ok(orders);
		}
	}
}
