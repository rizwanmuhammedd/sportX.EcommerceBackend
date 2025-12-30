using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportex.Application.DTOs.Checkout;

public class ConfirmCheckoutDto
{
    public int UserId { get; set; }
    public string ShippingAddress { get; set; } = "";
}

