using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportex.Application.DTOs.Products
{
    public class ProductRatingDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public RatingDistributionDto Distribution { get; set; } = new RatingDistributionDto(); // Initialize here
    }
}
