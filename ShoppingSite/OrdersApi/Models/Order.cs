using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersApi.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime DateOfCreation { get; set; }

	public decimal? TotalPrice { get; set; }

    public int CartId { get; set; }

    public string? CustomerId { get; set; }

}
