// In IReviewRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportex.Domain.Entities;

namespace Sportex.Application.Interfaces
{
    public interface IReviewRepository
    {
        // Existing methods
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetByProductAsync(int productId, int page = 1, int pageSize = 10);
        Task<double> GetAverageRatingAsync(int productId);
        Task<int> GetCountByProductAsync(int productId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task<bool> ExistsAsync(int productId, int userId);
        Task<IEnumerable<ReviewHelpful>> GetHelpfulByUserAsync(int userId);
        Task AddHelpfulAsync(ReviewHelpful helpful);

        // New methods
        Task<Review?> GetUserReviewAsync(int productId, int userId);
        Task<IEnumerable<Review>> GetRecentReviewsAsync(int count = 5);
        Task<IEnumerable<Review>> GetTopRatedReviewsAsync(int productId, int count = 5);
        Task<IEnumerable<Review>> GetMostHelpfulReviewsAsync(int productId, int count = 5);
        Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId);
        Task<int> GetVerifiedPurchaseCountAsync(int productId);
        Task<bool> HasUserReviewedAsync(int productId, int userId);
        Task<bool> IsReviewHelpfulByUserAsync(int reviewId, int userId);
        Task<bool> RemoveHelpfulAsync(int reviewId, int userId);
    }
}