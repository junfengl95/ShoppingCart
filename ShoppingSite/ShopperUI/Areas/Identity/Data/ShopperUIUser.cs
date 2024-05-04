using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ShopperUI.Models;

namespace ShopperUI.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ShopperUIUser class
public class ShopperUIUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }


    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }


    [PersonalData]
    [Column(TypeName = "int")]
    public int? CartId { get; set; }

	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

