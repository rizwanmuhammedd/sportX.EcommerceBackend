using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Sportex.Application.DTOs.Products
{
    public class AddReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty; // Initialize with empty string
    }
}
