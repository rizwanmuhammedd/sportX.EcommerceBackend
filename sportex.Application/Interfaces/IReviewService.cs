//// In IReviewService.cs (Application.Interfaces)
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Sportex.Application.DTOs.Products;

//namespace Sportex.Application.Interfaces
//{
//    public interface IReviewService
//    {
//        // Existing methods
//        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10);
//        Task<ReviewDto> AddReviewAsync(int productId, int userId, AddReviewDto dto);
//        Task<bool> MarkReviewHelpfulAsync(int reviewId, int userId);
//        Task<double> GetAverageRatingAsync(int productId);
//        Task<int> GetReviewCountAsync(int productId);

//        // New methods
//        Task<ReviewDto?> GetUserReviewAsync(int productId, int userId);
//        Task<ProductRatingDto> GetProductRatingAsync(int productId);
//        Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int count = 5);
//        Task<IEnumerable<ReviewDto>> GetTopRatedReviewsAsync(int productId, int count = 5);
//        Task<IEnumerable<ReviewDto>> GetMostHelpfulReviewsAsync(int productId, int count = 5);
//        Task<bool> RemoveHelpfulAsync(int reviewId, int userId);
//        Task<ProductReviewStatsDto> GetReviewStatsAsync(int productId);
//        Task<bool> UpdateReviewAsync(int reviewId, int userId, AddReviewDto dto);
//        Task<bool> DeleteReviewAsync(int reviewId, int userId);
//        Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId);
//    }
//}








using System.Collections.Generic;
using System.Threading.Tasks;
using Sportex.Application.DTOs.Products;

namespace Sportex.Application.Interfaces
{
    public interface IReviewService
    {
        // ✅ UPDATED: Added currentUserId parameter
        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int? currentUserId = null, int page = 1, int pageSize = 10);

        Task<ReviewDto> AddReviewAsync(int productId, int userId, AddReviewDto dto);
        Task<bool> MarkReviewHelpfulAsync(int reviewId, int userId);
        Task<double> GetAverageRatingAsync(int productId);
        Task<int> GetReviewCountAsync(int productId);

        // New methods
        Task<ReviewDto?> GetUserReviewAsync(int productId, int userId);
        Task<ProductRatingDto> GetProductRatingAsync(int productId);

        // ✅ UPDATED: Added currentUserId parameter
        Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int? currentUserId = null, int count = 5);

        // ✅ UPDATED: Added currentUserId parameter
        Task<IEnumerable<ReviewDto>> GetTopRatedReviewsAsync(int productId, int? currentUserId = null, int count = 5);

        // ✅ UPDATED: Added currentUserId parameter
        Task<IEnumerable<ReviewDto>> GetMostHelpfulReviewsAsync(int productId, int? currentUserId = null, int count = 5);

        Task<bool> RemoveHelpfulAsync(int reviewId, int userId);
        Task<ProductReviewStatsDto> GetReviewStatsAsync(int productId);
        Task<bool> UpdateReviewAsync(int reviewId, int userId, AddReviewDto dto);
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
        Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId);
    }
}