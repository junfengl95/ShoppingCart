using ShopperUI.Models;

namespace ShopperUI.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId {  get; set; }

        public DateTime DateOfCreation { get; set; }

        public int CartId { get; set; }

        public decimal? TotalPrice { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public string UserId {  get; set; }
    }
}
