

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Products;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using System.Security.Claims;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/Products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly IReviewService _reviewService;
    private readonly Cloudinary _cloudinary;

    public ProductsController(IProductRepository repo, IConfiguration config, IReviewService reviewService)
    {
        _repo = repo;
        _reviewService = reviewService;

        var cloudName = config["Cloudinary:CloudName"] ?? string.Empty;
        var apiKey = config["Cloudinary:ApiKey"] ?? string.Empty;
        var apiSecret = config["Cloudinary:ApiSecret"] ?? string.Empty;

        var acc = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(acc);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repo.GetAllAsync();
        var result = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl ?? string.Empty,
            IsActive = p.IsActive
        });
        return Ok(ApiResponse.Success("Products fetched", result));
    }

    [HttpGet("GetActive")]
    public async Task<IActionResult> GetActive()
    {
        var products = await _repo.GetActiveAsync();
        var result = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl ?? string.Empty,
            IsActive = p.IsActive
        });
        return Ok(ApiResponse.Success("Active products", result));
    }

    [Authorize]
    [HttpGet("GetBy_{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        return Ok(ApiResponse.Success("Product found", new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl ?? string.Empty,
            IsActive = p.IsActive
        }));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("Admin/GetBy_{id}")]
    public async Task<IActionResult> GetByIdAdmin(int id)
    {
        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        return Ok(ApiResponse.Success("Product found", new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl ?? string.Empty,
            IsActive = p.IsActive
        }));
    }

    [Authorize]
    [HttpGet("GetCatBy_{categoryId}")]
    public async Task<IActionResult> GetByCategory(string categoryId)
    {
        var products = await _repo.GetByCategoryAsync(categoryId);
        var result = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl ?? string.Empty,
            IsActive = p.IsActive
        });
        return Ok(ApiResponse.Success("Products fetched", result));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("Admin/Create")]
    public async Task<IActionResult> Add([FromForm] CreateProductDto dto, IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest(ApiResponse.Fail(400, "Image is required"));

        var upload = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(image.FileName, image.OpenReadStream()),
            Folder = "sportex_products"
        });

        var product = new Product
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category,
            ImageUrl = upload.SecureUrl?.ToString() ?? string.Empty,
            IsActive = true
        };

        await _repo.AddAsync(product);
        return Ok(ApiResponse.Success("Product created", product));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/Toggle/{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.IsActive = !p.IsActive;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Product toggled", p));
    }

    //[Authorize(Roles = "Admin")]
    //[HttpDelete("Admin/Delete/{id}")]
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var p = await _repo.GetByIdAdminAsync(id);
    //    if (p == null)
    //        return NotFound(ApiResponse.Fail(404, "Product not found"));

    //    p.IsActive = false;
    //    await _repo.UpdateAsync(p);
    //    return Ok(ApiResponse.Success("Product deleted"));
    //}

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/SoftDelete/{id}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        await _repo.SoftDeleteAsync(id);
        return Ok(ApiResponse.Success("Product soft deleted"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/Restore/{id}")]
    public async Task<IActionResult> Restore(int id)
    {
        await _repo.RestoreAsync(id);
        return Ok(ApiResponse.Success("Product restored"));
    }

    [Authorize]
    [HttpGet("User/GetAll")]
    public async Task<IActionResult> UserGetAll()
    {
        var items = (await _repo.GetAllAsync())
            .Where(x => x.IsActive)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category.ToString(),
                ImageUrl = p.ImageUrl ?? string.Empty,
                IsActive = p.IsActive
            });
        return Ok(ApiResponse.Success("Active products", items));
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(ApiResponse.Success("Search result", new List<object>()));

        q = q.Trim();
        var all = await _repo.GetAllAsync();
        var items = all
            .Where(x => x.IsActive &&
                ((!string.IsNullOrEmpty(x.Name) && x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                 x.Category.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)))
            .Select(x => new
            {
                x.Id,
                Name = x.Name ?? string.Empty,
                x.Price,
                ImageUrl = x.ImageUrl ?? string.Empty,
                x.StockQuantity,
                x.Category,
                x.IsActive
            })
            .ToList();

        return Ok(ApiResponse.Success("Search result", items));
    }

    [HttpGet("Paged")]
    public async Task<IActionResult> Paged(int page = 1, int size = 10)
    {
        var items = (await _repo.GetAllAsync())
            .Where(x => x.IsActive)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category.ToString(),
                ImageUrl = p.ImageUrl ?? string.Empty,
                IsActive = p.IsActive
            });
        return Ok(ApiResponse.Success("Paged products", items));
    }

    [HttpGet("Filter_Sort")]
    public async Task<IActionResult> FilterSort(string? category, decimal? min, decimal? max, string? sort)
    {
        var items = (await _repo.GetAllAsync())
                    .Where(x => x.IsActive)
                    .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            items = items.Where(x => x.Category.ToString() == category);
        if (min.HasValue) items = items.Where(x => x.Price >= min.Value);
        if (max.HasValue) items = items.Where(x => x.Price <= max.Value);
        if (sort == "price_asc") items = items.OrderBy(x => x.Price);
        if (sort == "price_desc") items = items.OrderByDescending(x => x.Price);

        var result = items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl ?? string.Empty,
            IsActive = p.IsActive
        });
        return Ok(ApiResponse.Success("Filtered products", result));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/Add_Stock")]
    public async Task<IActionResult> AddStock(int id, int qty)
    {
        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.StockQuantity += qty;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Stock added", p));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("Admin/AddImages")]
    public async Task<IActionResult> AddImages(int id, IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest(ApiResponse.Fail(400, "Image is required"));

        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        var upload = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(image.FileName, image.OpenReadStream()),
            Folder = "sportex_products"
        });

        p.ImageUrl = upload.SecureUrl?.ToString() ?? string.Empty;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Image updated", p));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock()
    {
        var items = (await _repo.GetAllAsync())
                    .Where(x => x.StockQuantity <= 10 && x.IsActive);
        return Ok(ApiResponse.Success("Low stock items", items));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("Admin/Update/{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.Name = dto.Name?.Trim() ?? string.Empty;
        p.Price = dto.Price;
        p.StockQuantity = dto.StockQuantity;
        p.Category = dto.Category;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Product updated successfully", p));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/UpdatePrice/{id}")]
    public async Task<IActionResult> UpdatePrice(int id, decimal price)
    {
        if (price <= 0)
            return BadRequest(ApiResponse.Fail(400, "Invalid price"));

        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.Price = price;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Price updated", p));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/SetStock/{id}")]
    public async Task<IActionResult> SetStock(int id, int qty)
    {
        if (qty < 0)
            return BadRequest(ApiResponse.Fail(400, "Stock cannot be negative"));

        var p = await _repo.GetByIdAdminAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.StockQuantity = qty;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Stock updated", p));
    }

    // ========== NEW ENDPOINTS FOR MoreProducts PAGE ==========

    [HttpGet("categories-list")]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _repo.GetAllCategoriesAsync();
            var allCategories = new List<string> { "All" };
            allCategories.AddRange(categories);
            return Ok(ApiResponse.Success("Categories retrieved", allCategories));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error retrieving categories: {ex.Message}"));
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetProductStats()
    {
        try
        {
            var stats = await _repo.GetProductStatsAsync();
            return Ok(ApiResponse.Success("Product statistics", stats));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error retrieving stats: {ex.Message}"));
        }
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredProducts(
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9)
    {
        try
        {
            var (products, totalCount) = await _repo.GetFilteredProductsAsync(
                category, minPrice, maxPrice, sortBy, search, page, pageSize);

            var result = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category.ToString(),
                ImageUrl = p.ImageUrl ?? string.Empty,
                IsActive = p.IsActive
            }).ToList();

            var response = new
            {
                Data = result,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    HasNextPage = page * pageSize < totalCount,
                    HasPreviousPage = page > 1
                },
                Filters = new
                {
                    Category = category,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SortBy = sortBy,
                    Search = search
                }
            };

            return Ok(ApiResponse.Success("Products filtered successfully", response));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error filtering products: {ex.Message}"));
        }
    }

    [HttpGet("search-suggestions")]
    public async Task<IActionResult> GetSearchSuggestions([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(ApiResponse.Success("Search suggestions", new List<string>()));

        var allProducts = await _repo.GetAllAsync();
        var suggestions = allProducts
            .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Name) && p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Name ?? string.Empty)
            .Distinct()
            .Take(10)
            .ToList();

        return Ok(ApiResponse.Success("Search suggestions", suggestions));
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 6)
    {
        try
        {
            var allProducts = await _repo.GetAllAsync();
            var featuredProducts = allProducts
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Take(count)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name ?? string.Empty,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Category = p.Category.ToString(),
                    ImageUrl = p.ImageUrl ?? string.Empty,
                    IsActive = p.IsActive
                })
                .ToList();

            return Ok(ApiResponse.Success("Featured products", featuredProducts));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error retrieving featured products: {ex.Message}"));
        }
    }

    // ========== NEW ENDPOINTS FOR ProductDetails PAGE ==========

    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetProductStats(int id)
    {
        try
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse.Fail(404, "Product not found"));

            var stats = new
            {
                Views = await _repo.GetProductViewsAsync(id),
                Purchases = await _repo.GetPurchaseCountAsync(id),
                WishlistCount = await _repo.GetWishlistCountAsync(id),
                AverageRating = await _reviewService.GetAverageRatingAsync(id),
                ReviewCount = await _reviewService.GetReviewCountAsync(id)
            };

            return Ok(ApiResponse.Success("Product statistics", stats));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting stats: {ex.Message}"));
        }
    }

    // ✅ FIXED: SINGLE GetProductReviews method (REMOVE THE DUPLICATE!)
    [HttpGet("{id}/reviews")]
    [Authorize] // This ensures user is logged in
    public async Task<IActionResult> GetProductReviews(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? currentUserId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
            {
                currentUserId = parsedId;
            }

            var reviews = await _reviewService.GetProductReviewsAsync(id, currentUserId, page, pageSize);
            return Ok(ApiResponse.Success("Product reviews", reviews));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting reviews: {ex.Message}"));
        }
    }

    // ✅ FIXED: AddReview endpoint
    [HttpPost("{id}/reviews")]
    [Authorize]
    public async Task<IActionResult> AddReview(int id, [FromBody] AddReviewDto dto)
    {
        if (dto == null)
            return BadRequest(ApiResponse.Fail(400, "Invalid review data"));

        if (string.IsNullOrWhiteSpace(dto.Comment))
            return BadRequest(ApiResponse.Fail(400, "Comment cannot be empty"));

        // ✅ Use "uid" claim instead of ClaimTypes.NameIdentifier
        var userIdClaim = User.FindFirst("uid")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            // Try alternative claim names
            userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("nameid")?.Value
                         ?? User.FindFirst("sub")?.Value;
        }

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(ApiResponse.Fail(401, "User ID not found in token"));

        if (!int.TryParse(userIdClaim, out int userId))
            return BadRequest(ApiResponse.Fail(400, "Invalid user ID format"));

        try
        {
            var review = await _reviewService.AddReviewAsync(id, userId, dto);
            return Ok(ApiResponse.Success("Review saved successfully", review));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    // ✅ FIXED: MarkReviewHelpful endpoint
    [HttpPost("reviews/{reviewId}/helpful")]
    [Authorize]
    public async Task<IActionResult> MarkReviewHelpful(int reviewId)
    {
        try
        {
            // ✅ Use "uid" claim
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiResponse.Fail(401, "User ID not found in token"));

            await _reviewService.MarkReviewHelpfulAsync(reviewId, userId);
            return Ok(ApiResponse.Success("Review marked helpful", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    [HttpGet("{id}/related")]
    public async Task<IActionResult> GetRelatedProducts(int id, [FromQuery] int count = 4)
    {
        try
        {
            var relatedProducts = await _repo.GetRelatedProductsAsync(id, count);
            var result = relatedProducts.Select(p => new
            {
                Id = p.Id,
                Name = p.Name ?? string.Empty,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category.ToString(),
                ImageUrl = p.ImageUrl ?? string.Empty,
                IsActive = p.IsActive
            }).ToList();
            return Ok(ApiResponse.Success("Related products", result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting related products: {ex.Message}"));
        }
    }

    [HttpPost("{id}/view")]
    public async Task<IActionResult> TrackProductView(int id)
    {
        try
        {
            await _repo.IncrementViewCountAsync(id);
            return Ok(ApiResponse.Success("View tracked", null));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error tracking view: {ex.Message}"));
        }
    }

    // Additional review endpoints

    [HttpGet("{id}/rating")]
    public async Task<IActionResult> GetProductRating(int id)
    {
        try
        {
            var rating = await _reviewService.GetProductRatingAsync(id);
            return Ok(ApiResponse.Success("Product rating", rating));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting rating: {ex.Message}"));
        }
    }

    [HttpGet("{id}/reviews/stats")]
    public async Task<IActionResult> GetReviewStats(int id)
    {
        try
        {
            var stats = await _reviewService.GetReviewStatsAsync(id);
            return Ok(ApiResponse.Success("Review statistics", stats));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting review stats: {ex.Message}"));
        }
    }

    // ✅ FIXED: GetTopRatedReviews endpoint
    [HttpGet("{id}/reviews/top-rated")]
    [Authorize]
    public async Task<IActionResult> GetTopRatedReviews(int id, [FromQuery] int count = 5)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? currentUserId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
            {
                currentUserId = parsedId;
            }

            var reviews = await _reviewService.GetTopRatedReviewsAsync(id, currentUserId, count);
            return Ok(ApiResponse.Success("Top rated reviews", reviews));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting top rated reviews: {ex.Message}"));
        }
    }

    // ✅ FIXED: GetMostHelpfulReviews endpoint
    [HttpGet("{id}/reviews/most-helpful")]
    [Authorize]
    public async Task<IActionResult> GetMostHelpfulReviews(int id, [FromQuery] int count = 5)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? currentUserId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
            {
                currentUserId = parsedId;
            }

            var reviews = await _reviewService.GetMostHelpfulReviewsAsync(id, currentUserId, count);
            return Ok(ApiResponse.Success("Most helpful reviews", reviews));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting helpful reviews: {ex.Message}"));
        }
    }

    // ✅ FIXED: GetMyReview endpoint
    [HttpGet("{id}/my-review")]
    [Authorize]
    public async Task<IActionResult> GetMyReview(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiResponse.Fail(401, "User ID not found in token"));

            var review = await _reviewService.GetUserReviewAsync(id, userId);

            if (review == null)
                return Ok(ApiResponse.Success("No review found", null));

            return Ok(ApiResponse.Success("Your review", review));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting your review: {ex.Message}"));
        }
    }

    // ✅ FIXED: DeleteReview endpoint
    [HttpDelete("reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiResponse.Fail(401, "User ID not found in token"));

            var result = await _reviewService.DeleteReviewAsync(reviewId, userId);

            if (!result)
                return BadRequest(ApiResponse.Fail(400, "Cannot delete review"));

            return Ok(ApiResponse.Success("Review deleted", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    // ✅ FIXED: UpdateReview endpoint
    [HttpPut("reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] AddReviewDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiResponse.Fail(401, "User ID not found in token"));

            var result = await _reviewService.UpdateReviewAsync(reviewId, userId, dto);

            if (!result)
                return BadRequest(ApiResponse.Fail(400, "Cannot update review"));

            return Ok(ApiResponse.Success("Review updated", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    // ✅ FIXED: RemoveHelpful endpoint
    [HttpDelete("reviews/{reviewId}/helpful")]
    [Authorize]
    public async Task<IActionResult> RemoveHelpful(int reviewId)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiResponse.Fail(401, "User ID not found in token"));

            var result = await _reviewService.RemoveHelpfulAsync(reviewId, userId);

            if (!result)
                return BadRequest(ApiResponse.Fail(400, "Cannot remove helpful mark"));

            return Ok(ApiResponse.Success("Helpful mark removed", null));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    // ✅ FIXED: GetRecentReviews endpoint
    [HttpGet("recent-reviews")]
    [Authorize]
    public async Task<IActionResult> GetRecentReviews([FromQuery] int count = 10)
    {
        try
        {
            var userIdClaim = User.FindFirst("uid")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int? currentUserId = null;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
            {
                currentUserId = parsedId;
            }

            var reviews = await _reviewService.GetRecentReviewsAsync(currentUserId, count);
            return Ok(ApiResponse.Success("Recent reviews", reviews));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail(500, $"Error getting recent reviews: {ex.Message}"));
        }
    }

    // Debug endpoint to check claims
    [HttpGet("debug/claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
}