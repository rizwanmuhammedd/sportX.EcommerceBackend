//// In ReviewService.cs - UPDATED
//using Microsoft.EntityFrameworkCore;
//using Sportex.Application.DTOs.Products;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;
//using Sportex.Infrastructure.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Sportex.Infrastructure.Services
//{
//    public class ReviewService : IReviewService
//    {
//        private readonly SportexDbContext _context;
//        private readonly IReviewRepository _reviewRepository;

//        public ReviewService(SportexDbContext context, IReviewRepository reviewRepository)
//        {
//            _context = context;
//            _reviewRepository = reviewRepository;
//        }

//        // Existing methods...
//        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 10)
//        {
//            try
//            {
//                var reviews = await _context.Reviews
//                    .Include(r => r.User)
//                    .Where(r => r.ProductId == productId)
//                    .OrderByDescending(r => r.CreatedAt)
//                    .Skip((page - 1) * pageSize)
//                    .Take(pageSize)
//                    .ToListAsync();

//                return reviews.Select(r => MapToReviewDto(r));
//            }
//            catch (Exception)
//            {
//                return new List<ReviewDto>();
//            }
//        }

//        public async Task<ReviewDto> AddReviewAsync(int productId, int userId, AddReviewDto dto)
//        {
//            // ✅ Guard against null DTO
//            if (dto == null)
//                throw new ArgumentException("Review data cannot be null");

//            if (string.IsNullOrWhiteSpace(dto.Comment))
//                throw new ArgumentException("Review comment cannot be empty");

//            // STEP 1 — Check if user already reviewed this product
//            var existingReview = await _reviewRepository.GetUserReviewAsync(productId, userId);

//            if (existingReview != null)
//            {
//                existingReview.Rating = dto.Rating;
//                existingReview.Comment = dto.Comment;   // already validated above
//                existingReview.CreatedAt = DateTime.UtcNow;

//                await _reviewRepository.UpdateAsync(existingReview);

//                return MapToReviewDto(existingReview);
//            }

//            // STEP 2 — Create new review
//            var review = new Review
//            {
//                ProductId = productId,
//                UserId = userId,
//                Rating = dto.Rating,
//                Comment = dto.Comment,   // safe now
//                CreatedAt = DateTime.UtcNow,
//                HelpfulCount = 0
//            };

//            await _reviewRepository.AddAsync(review);

//            var user = await _context.Users.FindAsync(userId);

//            return MapToReviewDto(review, user);
//        }


//        public async Task<bool> MarkReviewHelpfulAsync(int reviewId, int userId)
//        {
//            try
//            {
//                // Check if already marked as helpful
//                var existing = await _context.ReviewHelpfuls
//                    .AnyAsync(rh => rh.ReviewId == reviewId && rh.UserId == userId);

//                if (existing)
//                    return true;

//                var helpful = new ReviewHelpful
//                {
//                    ReviewId = reviewId,
//                    UserId = userId,
//                    CreatedAt = DateTime.UtcNow
//                };

//                await _reviewRepository.AddHelpfulAsync(helpful);

//                // Increment helpful count
//                var review = await _reviewRepository.GetByIdAsync(reviewId);
//                if (review != null)
//                {
//                    review.HelpfulCount++;
//                    await _reviewRepository.UpdateAsync(review);
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<double> GetAverageRatingAsync(int productId)
//        {
//            return await _reviewRepository.GetAverageRatingAsync(productId);
//        }

//        public async Task<int> GetReviewCountAsync(int productId)
//        {
//            return await _reviewRepository.GetCountByProductAsync(productId);
//        }

//        // New methods implementation
//        public async Task<ReviewDto?> GetUserReviewAsync(int productId, int userId)
//        {
//            var review = await _reviewRepository.GetUserReviewAsync(productId, userId);
//            if (review == null)
//                return null;

//            return MapToReviewDto(review);
//        }

//        public async Task<ProductRatingDto> GetProductRatingAsync(int productId)
//        {
//            var averageRating = await _reviewRepository.GetAverageRatingAsync(productId);
//            var totalReviews = await _reviewRepository.GetCountByProductAsync(productId);
//            var distribution = await _reviewRepository.GetRatingDistributionAsync(productId);

//            return new ProductRatingDto
//            {
//                AverageRating = averageRating,
//                TotalReviews = totalReviews,
//                Distribution = new RatingDistributionDto
//                {
//                    Rating5 = distribution.ContainsKey(5) ? distribution[5] : 0,
//                    Rating4 = distribution.ContainsKey(4) ? distribution[4] : 0,
//                    Rating3 = distribution.ContainsKey(3) ? distribution[3] : 0,
//                    Rating2 = distribution.ContainsKey(2) ? distribution[2] : 0,
//                    Rating1 = distribution.ContainsKey(1) ? distribution[1] : 0
//                }
//            };
//        }

//        public async Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int count = 5)
//        {
//            var reviews = await _reviewRepository.GetRecentReviewsAsync(count);
//            return reviews.Select(r => MapToReviewDto(r));
//        }

//        public async Task<IEnumerable<ReviewDto>> GetTopRatedReviewsAsync(int productId, int count = 5)
//        {
//            var reviews = await _reviewRepository.GetTopRatedReviewsAsync(productId, count);
//            return reviews.Select(r => MapToReviewDto(r));
//        }

//        public async Task<IEnumerable<ReviewDto>> GetMostHelpfulReviewsAsync(int productId, int count = 5)
//        {
//            var reviews = await _reviewRepository.GetMostHelpfulReviewsAsync(productId, count);
//            return reviews.Select(r => MapToReviewDto(r));
//        }

//        public async Task<bool> RemoveHelpfulAsync(int reviewId, int userId)
//        {
//            return await _reviewRepository.RemoveHelpfulAsync(reviewId, userId);
//        }

//        public async Task<ProductReviewStatsDto> GetReviewStatsAsync(int productId)
//        {
//            var averageRating = await _reviewRepository.GetAverageRatingAsync(productId);
//            var totalReviews = await _reviewRepository.GetCountByProductAsync(productId);
//            var verifiedPurchases = await _reviewRepository.GetVerifiedPurchaseCountAsync(productId);

//            var reviews = await _context.Reviews
//                .Where(r => r.ProductId == productId)
//                .ToListAsync();

//            var withComments = reviews.Count(r => !string.IsNullOrWhiteSpace(r.Comment));
//            var helpfulCount = reviews.Sum(r => r.HelpfulCount);

//            return new ProductReviewStatsDto
//            {
//                AverageRating = averageRating,
//                TotalReviews = totalReviews,
//                VerifiedPurchases = verifiedPurchases,
//                WithComments = withComments,
//                HelpfulCount = helpfulCount
//            };
//        }

//        public async Task<bool> UpdateReviewAsync(int reviewId, int userId, AddReviewDto dto)
//        {
//            var review = await _reviewRepository.GetByIdAsync(reviewId);
//            if (review == null || review.UserId != userId)
//                return false;

//            review.Rating = dto.Rating;
//            review.Comment = dto.Comment ?? string.Empty;
//            review.CreatedAt = DateTime.UtcNow;

//            await _reviewRepository.UpdateAsync(review);
//            return true;
//        }

//        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
//        {
//            var review = await _reviewRepository.GetByIdAsync(reviewId);
//            if (review == null || review.UserId != userId)
//                return false;

//            // Remove all helpful votes first
//            var helpfuls = await _context.ReviewHelpfuls
//                .Where(rh => rh.ReviewId == reviewId)
//                .ToListAsync();

//            _context.ReviewHelpfuls.RemoveRange(helpfuls);
//            _context.Reviews.Remove(review);

//            await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId)
//        {
//            return await _reviewRepository.GetRatingDistributionAsync(productId);
//        }

//        private ReviewDto MapToReviewDto(Review review, User? user = null)
//        {
//            user ??= review.User;

//            // For now, set IsVerifiedPurchase to false - you'll need to implement this
//            var isVerifiedPurchase = false; // You need to check order history

//            return new ReviewDto
//            {
//                Id = review.Id,
//                ProductId = review.ProductId,
//                UserId = review.UserId,
//                UserName = user?.Name ?? "Anonymous",
//                UserAvatar = GetAvatarInitials(user?.Name),
//                Rating = review.Rating,
//                Comment = review.Comment ?? string.Empty,
//                CreatedAt = review.CreatedAt,
//                HelpfulCount = review.HelpfulCount,
//                IsVerifiedPurchase = isVerifiedPurchase,
//                IsHelpful = false // You'll need to pass current user to set this
//            };
//        }

//        private string GetAvatarInitials(string? name)
//        {
//            if (string.IsNullOrEmpty(name))
//                return "A";

//            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
//            if (parts.Length >= 2)
//                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

//            return name.Length >= 2
//                ? name.Substring(0, 2).ToUpper()
//                : name.ToUpper();
//        }


//        // Add this method to your ReviewService class
//        private async Task<bool> IsReviewHelpfulByUser(int reviewId, int userId)
//        {
//            return await _reviewRepository.IsReviewHelpfulByUserAsync(reviewId, userId);
//        }

//        // Also update the MapToReviewDto method to use currentUserId:
//        private ReviewDto MapToReviewDto(Review review, User? user = null, int? currentUserId = null)
//        {
//            user ??= review.User;

//            // For now, set IsVerifiedPurchase to false - you'll need to implement this
//            var isVerifiedPurchase = false; // You need to check order history

//            return new ReviewDto
//            {
//                Id = review.Id,
//                ProductId = review.ProductId,
//                UserId = review.UserId,
//                UserName = user?.Name ?? "Anonymous",
//                UserAvatar = GetAvatarInitials(user?.Name),
//                Rating = review.Rating,
//                Comment = review.Comment ?? string.Empty,
//                CreatedAt = review.CreatedAt,
//                HelpfulCount = review.HelpfulCount,
//                IsVerifiedPurchase = isVerifiedPurchase,
//                IsHelpful = currentUserId.HasValue
//                    ? IsReviewHelpfulByUser(review.Id, currentUserId.Value).GetAwaiter().GetResult()
//                    : false
//            };
//        }

//        // Update GetProductReviewsAsync method signature:
//        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int? currentUserId = null, int page = 1, int pageSize = 10)
//        {
//            try
//            {
//                var reviews = await _context.Reviews
//                    .Include(r => r.User)
//                    .Where(r => r.ProductId == productId)
//                    .OrderByDescending(r => r.CreatedAt)
//                    .Skip((page - 1) * pageSize)
//                    .Take(pageSize)
//                    .ToListAsync();

//                return reviews.Select(r => MapToReviewDto(r, null, currentUserId));
//            }
//            catch (Exception)
//            {
//                return new List<ReviewDto>();
//            }
//        }
//    }


//}


using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Products;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sportex.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly SportexDbContext _context;
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(SportexDbContext context, IReviewRepository reviewRepository)
        {
            _context = context;
            _reviewRepository = reviewRepository;
        }

        // ✅ UPDATED: Accepts currentUserId parameter
        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int? currentUserId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.ProductId == productId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return reviews.Select(r => MapToReviewDto(r, null, currentUserId));
            }
            catch (Exception)
            {
                return new List<ReviewDto>();
            }
        }

        public async Task<ReviewDto> AddReviewAsync(int productId, int userId, AddReviewDto dto)
        {
            // ✅ Guard against null DTO
            if (dto == null)
                throw new ArgumentException("Review data cannot be null");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                throw new ArgumentException("Review comment cannot be empty");

            // STEP 1 — Check if user already reviewed this product
            var existingReview = await _reviewRepository.GetUserReviewAsync(productId, userId);

            if (existingReview != null)
            {
                existingReview.Rating = dto.Rating;
                existingReview.Comment = dto.Comment;   // already validated above
                existingReview.CreatedAt = DateTime.UtcNow;

                await _reviewRepository.UpdateAsync(existingReview);

                return MapToReviewDto(existingReview);
            }

            // STEP 2 — Create new review
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,   // safe now
                CreatedAt = DateTime.UtcNow,
                HelpfulCount = 0
            };

            await _reviewRepository.AddAsync(review);

            var user = await _context.Users.FindAsync(userId);

            return MapToReviewDto(review, user);
        }

        public async Task<bool> MarkReviewHelpfulAsync(int reviewId, int userId)
        {
            try
            {
                // Check if already marked as helpful
                var existing = await _context.ReviewHelpfuls
                    .AnyAsync(rh => rh.ReviewId == reviewId && rh.UserId == userId);

                if (existing)
                    return true;

                var helpful = new ReviewHelpful
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _reviewRepository.AddHelpfulAsync(helpful);

                // Increment helpful count
                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review != null)
                {
                    review.HelpfulCount++;
                    await _reviewRepository.UpdateAsync(review);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _reviewRepository.GetAverageRatingAsync(productId);
        }

        public async Task<int> GetReviewCountAsync(int productId)
        {
            return await _reviewRepository.GetCountByProductAsync(productId);
        }

        // New methods implementation
        public async Task<ReviewDto?> GetUserReviewAsync(int productId, int userId)
        {
            var review = await _reviewRepository.GetUserReviewAsync(productId, userId);
            if (review == null)
                return null;

            return MapToReviewDto(review, null, userId);
        }

        public async Task<ProductRatingDto> GetProductRatingAsync(int productId)
        {
            var averageRating = await _reviewRepository.GetAverageRatingAsync(productId);
            var totalReviews = await _reviewRepository.GetCountByProductAsync(productId);
            var distribution = await _reviewRepository.GetRatingDistributionAsync(productId);

            return new ProductRatingDto
            {
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                Distribution = new RatingDistributionDto
                {
                    Rating5 = distribution.ContainsKey(5) ? distribution[5] : 0,
                    Rating4 = distribution.ContainsKey(4) ? distribution[4] : 0,
                    Rating3 = distribution.ContainsKey(3) ? distribution[3] : 0,
                    Rating2 = distribution.ContainsKey(2) ? distribution[2] : 0,
                    Rating1 = distribution.ContainsKey(1) ? distribution[1] : 0
                }
            };
        }

        // ✅ UPDATED: Accepts currentUserId parameter
        public async Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int? currentUserId = null, int count = 5)
        {
            var reviews = await _reviewRepository.GetRecentReviewsAsync(count);
            return reviews.Select(r => MapToReviewDto(r, null, currentUserId));
        }

        // ✅ UPDATED: Accepts currentUserId parameter
        public async Task<IEnumerable<ReviewDto>> GetTopRatedReviewsAsync(int productId, int? currentUserId = null, int count = 5)
        {
            var reviews = await _reviewRepository.GetTopRatedReviewsAsync(productId, count);
            return reviews.Select(r => MapToReviewDto(r, null, currentUserId));
        }

        // ✅ UPDATED: Accepts currentUserId parameter
        public async Task<IEnumerable<ReviewDto>> GetMostHelpfulReviewsAsync(int productId, int? currentUserId = null, int count = 5)
        {
            var reviews = await _reviewRepository.GetMostHelpfulReviewsAsync(productId, count);
            return reviews.Select(r => MapToReviewDto(r, null, currentUserId));
        }

        public async Task<bool> RemoveHelpfulAsync(int reviewId, int userId)
        {
            return await _reviewRepository.RemoveHelpfulAsync(reviewId, userId);
        }

        public async Task<ProductReviewStatsDto> GetReviewStatsAsync(int productId)
        {
            var averageRating = await _reviewRepository.GetAverageRatingAsync(productId);
            var totalReviews = await _reviewRepository.GetCountByProductAsync(productId);
            var verifiedPurchases = await _reviewRepository.GetVerifiedPurchaseCountAsync(productId);

            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            var withComments = reviews.Count(r => !string.IsNullOrWhiteSpace(r.Comment));
            var helpfulCount = reviews.Sum(r => r.HelpfulCount);

            return new ProductReviewStatsDto
            {
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                VerifiedPurchases = verifiedPurchases,
                WithComments = withComments,
                HelpfulCount = helpfulCount
            };
        }

        public async Task<bool> UpdateReviewAsync(int reviewId, int userId, AddReviewDto dto)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
                return false;

            review.Rating = dto.Rating;
            review.Comment = dto.Comment ?? string.Empty;
            review.CreatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review);
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
                return false;

            // Remove all helpful votes first
            var helpfuls = await _context.ReviewHelpfuls
                .Where(rh => rh.ReviewId == reviewId)
                .ToListAsync();

            _context.ReviewHelpfuls.RemoveRange(helpfuls);
            _context.Reviews.Remove(review);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId)
        {
            return await _reviewRepository.GetRatingDistributionAsync(productId);
        }

        // ✅ UPDATED: Added IsReviewHelpfulByUser method
        private async Task<bool> IsReviewHelpfulByUser(int reviewId, int userId)
        {
            return await _reviewRepository.IsReviewHelpfulByUserAsync(reviewId, userId);
        }

        // ✅ FIXED: MapToReviewDto method with proper async handling
        private async Task<ReviewDto> MapToReviewDtoAsync(Review review, User? user = null, int? currentUserId = null)
        {
            user ??= review.User;

            // ✅ Load user if not loaded
            if (user == null && review.UserId > 0)
            {
                user = await _context.Users.FindAsync(review.UserId);
            }

            // For now, set IsVerifiedPurchase to false - you'll need to implement this
            var isVerifiedPurchase = false; // You need to check order history

            return new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserName = user?.Name ?? "Anonymous",
                UserAvatar = GetAvatarInitials(user?.Name),
                UserProfileImage = user?.ProfileImageUrl ?? string.Empty,
                Rating = review.Rating,
                Comment = review.Comment ?? string.Empty,
                CreatedAt = review.CreatedAt,
                HelpfulCount = review.HelpfulCount,
                IsVerifiedPurchase = isVerifiedPurchase,
                IsHelpful = currentUserId.HasValue
                    ? await IsReviewHelpfulByUser(review.Id, currentUserId.Value) // ✅ Use await
                    : false
            };
        }

        // ✅ Keep the original MapToReviewDto for compatibility
        private ReviewDto MapToReviewDto(Review review, User? user = null, int? currentUserId = null)
        {
            // Call the async version and wait for it
            return MapToReviewDtoAsync(review, user, currentUserId).GetAwaiter().GetResult();
        }

        private string GetAvatarInitials(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return "A";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

            return name.Length >= 2
                ? name.Substring(0, 2).ToUpper()
                : name.ToUpper();
        }
    }
}