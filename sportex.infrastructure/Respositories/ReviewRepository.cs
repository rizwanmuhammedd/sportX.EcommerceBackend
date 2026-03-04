// In ReviewRepository.cs - FIXED VERSION
using Microsoft.EntityFrameworkCore;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sportex.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly SportexDbContext _context;

        public ReviewRepository(SportexDbContext context)
        {
            _context = context;
        }

        // Existing methods...
        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetByProductAsync(int productId, int page = 1, int pageSize = 10)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return Math.Round(reviews.Average(r => r.Rating), 1);
        }

        public async Task<int> GetCountByProductAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .CountAsync();
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int productId, int userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.UserId == userId);
        }

        public async Task<IEnumerable<ReviewHelpful>> GetHelpfulByUserAsync(int userId)
        {
            return await _context.ReviewHelpfuls
                .Where(rh => rh.UserId == userId)
                .ToListAsync();
        }

        public async Task AddHelpfulAsync(ReviewHelpful helpful)
        {
            _context.ReviewHelpfuls.Add(helpful);
            await _context.SaveChangesAsync();
        }

        // New methods implementation
        public async Task<Review?> GetUserReviewAsync(int productId, int userId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
        }

        public async Task<IEnumerable<Review>> GetRecentReviewsAsync(int count = 5)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetTopRatedReviewsAsync(int productId, int count = 5)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && r.Rating >= 4)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetMostHelpfulReviewsAsync(int productId, int count = 5)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.HelpfulCount)
                .ThenByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int productId)
        {
            var distribution = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new Dictionary<int, int>();

            // Initialize all ratings 1-5 with 0
            for (int i = 1; i <= 5; i++)
            {
                result[i] = 0;
            }

            // Fill with actual data
            foreach (var item in distribution)
            {
                result[item.Rating] = item.Count;
            }

            return result;
        }

        // FIXED: Added await for Task.FromResult
        public async Task<int> GetVerifiedPurchaseCountAsync(int productId)
        {
            // Placeholder implementation - you'll need to implement this with actual order data
            return await Task.FromResult(0);
        }

        public async Task<bool> HasUserReviewedAsync(int productId, int userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.UserId == userId);
        }

        public async Task<bool> IsReviewHelpfulByUserAsync(int reviewId, int userId)
        {
            return await _context.ReviewHelpfuls
                .AnyAsync(rh => rh.ReviewId == reviewId && rh.UserId == userId);
        }

        public async Task<bool> RemoveHelpfulAsync(int reviewId, int userId)
        {
            var helpful = await _context.ReviewHelpfuls
                .FirstOrDefaultAsync(rh => rh.ReviewId == reviewId && rh.UserId == userId);

            if (helpful == null)
                return false;

            _context.ReviewHelpfuls.Remove(helpful);

            // Decrement helpful count
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                review.HelpfulCount = Math.Max(0, review.HelpfulCount - 1);
                _context.Reviews.Update(review);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}