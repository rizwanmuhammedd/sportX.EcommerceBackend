//namespace Sportex.Application.DTOs.Products
//{

//    public class ProductDetailDto
//    {
//        public int Id { get; set; }
//        public string Name { get; set; } = "";
//        public decimal Price { get; set; }
//        public int StockQuantity { get; set; }
//        public string Category { get; set; } = "";
//        public string? ImageUrl { get; set; }
//    }
//}




namespace Sportex.Application.DTOs.Products
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true; // ✅ ADD THIS
    }
}