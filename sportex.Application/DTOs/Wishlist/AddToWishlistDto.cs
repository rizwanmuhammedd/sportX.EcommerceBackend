using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportex.Application.DTOs.Wishlist;

public class AddToWishlistDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
}

