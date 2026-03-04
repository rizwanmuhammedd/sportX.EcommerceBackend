namespace Sportex.Application.DTOs.Products
{
    public class ProductStatsDto
    {
        public int TotalProducts { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int LowStockCount { get; set; }
        public int TotalStock { get; set; }

        // Additional stats for product details page
        public int Views { get; set; }
        public int Purchases { get; set; }
        public int WishlistCount { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Rating distribution
        public int Rating5Count { get; set; }
        public int Rating4Count { get; set; }
        public int Rating3Count { get; set; }
        public int Rating2Count { get; set; }
        public int Rating1Count { get; set; }
    }
}