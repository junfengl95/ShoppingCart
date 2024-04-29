using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopperUI.ApiClient;
using ShopperUI.Areas.Identity.Data;
using ShopperUI.Models;
using System.Diagnostics;

namespace ShopperUI.Controllers
{
    //Ensure only authorized users can access webpage
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly UserManager<ShopperUIUser> _userManager;
		private readonly IShopperUIClient _shopperUIClient;

		public HomeController(ILogger<HomeController> logger, UserManager<ShopperUIUser> userManager, IShopperUIClient shopperUIClient)
        {
            _logger = logger;
			this._userManager = userManager;
			_shopperUIClient = shopperUIClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}

			Cart? cart = await _shopperUIClient.GetCartById((int)user.CartId);

			if (object.ReferenceEquals(cart, null))
			{
				// Reassign a new cart to the user
				Cart? newCart = await _shopperUIClient.CreateNewCart();

				if (newCart != null)
				{
					user.CartId = newCart.CartId;
					ViewBag.CartId = newCart.CartId;
					HttpContext.Session.SetInt32("CartId", newCart.CartId);
					return View();
				}
				else
				{
					return NotFound();
				}
			}
			else
			{
				ViewBag.CartId = cart.CartId;
				HttpContext.Session.SetInt32("CartId", cart.CartId);
				return View();
			}
		}
        

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
