using System;
using System.Collections.Generic;

namespace OrdersApi.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime DateOfCreation { get; set; }

    public decimal TotalPrice { get; set; }

    public int CartId { get; set; }

    public int CartCustomerId { get; set; }

    public string ProductIds { get; set; } = null!;
}
