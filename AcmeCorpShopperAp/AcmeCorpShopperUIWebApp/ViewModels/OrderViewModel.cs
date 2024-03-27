using AcmeCorpShopperUIWebApp.Models;

namespace AcmeCorpShopperUIWebApp.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public int CartId { get; set; }
        public decimal? TotalPrice { get; set; }
		public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
	}
}
