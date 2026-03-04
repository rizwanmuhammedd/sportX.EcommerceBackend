using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportex.Application.DTOs.Products
{
    public class ProductReviewStatsDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int VerifiedPurchases { get; set; }
        public int WithComments { get; set; }
        public int HelpfulCount { get; set; }
    }
}
