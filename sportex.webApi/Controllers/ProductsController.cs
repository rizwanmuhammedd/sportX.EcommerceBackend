

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Sportex.Application.Common;
//using Sportex.Application.DTOs;
//using Sportex.Application.DTOs.Products;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;

//namespace Sportex.WebApi.Controllers;

//[ApiController]
//[Route("api/products")]
//public class ProductsController : ControllerBase
//{
//    private readonly IProductRepository _repo;
//    public ProductsController(IProductRepository repo) => _repo = repo;

//    // 1️⃣ GET ALL PRODUCTS
//    [Authorize]
//    [HttpGet]
//    public async Task<IActionResult> GetAll()
//    {
//        var products = await _repo.GetAllAsync();

//        var result = products.Select(p => new ProductDto
//        {
//            Id = p.Id,
//            Name = p.Name,
//            Price = p.Price,
//            StockQuantity = p.StockQuantity,
//            Category = p.Category.ToString(),
//            ImageUrl = p.ImageUrl
//        });

//        return Ok(ApiResponse.Success("Products fetched", result));
//    }

//    // 2️⃣ GET PRODUCT BY ID
//    [Authorize]
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var p = await _repo.GetByIdAsync(id);
//        if (p == null)
//            return NotFound(ApiResponse.Fail(404, "Product not found"));

//        var result = new ProductDetailDto
//        {
//            Id = p.Id,
//            Name = p.Name,
//            Price = p.Price,
//            StockQuantity = p.StockQuantity,
//            Category = p.Category.ToString(),
//            ImageUrl = p.ImageUrl
//        };

//        return Ok(ApiResponse.Success("Product found", result));
//    }

//    // 3️⃣ GET BY CATEGORY
//    [Authorize]
//    [HttpGet("category/{category}")]
//    public async Task<IActionResult> GetByCategory(string category)
//    {
//        var products = await _repo.GetByCategoryAsync(category);

//        var result = products.Select(p => new ProductDto
//        {
//            Id = p.Id,
//            Name = p.Name,
//            Price = p.Price,
//            StockQuantity = p.StockQuantity,
//            Category = p.Category.ToString(),
//            ImageUrl = p.ImageUrl
//        });

//        return Ok(ApiResponse.Success("Products fetched", result));
//    }

//    // 4️⃣ ADD PRODUCT (ADMIN)
//    [Authorize(Roles = "Admin")]
//    [HttpPost]
//    public async Task<IActionResult> Add(CreateProductDto dto)
//    {
//        var product = new Product
//        {
//            Name = dto.Name.Trim(),
//            Price = dto.Price,
//            StockQuantity = dto.StockQuantity,
//            Category = dto.Category,
//            ImageUrl = dto.ImageUrl
//        };

//        await _repo.AddAsync(product);
//        return Ok(ApiResponse.Success("Product added successfully", product));
//    }

//    // 5️⃣ UPDATE STOCK (ADMIN)
//    [Authorize(Roles = "Admin")]
//    [HttpPut("stock/{id}")]
//    public async Task<IActionResult> UpdateStock(int id, int quantity)
//    {
//        if (quantity < 0)
//            return BadRequest(ApiResponse.Fail(400, "Stock cannot be negative"));

//        var product = await _repo.GetByIdAsync(id);
//        if (product == null)
//            return NotFound(ApiResponse.Fail(404, "Product not found"));

//        product.StockQuantity = quantity;
//        await _repo.UpdateAsync(product);

//        var result = new
//        {
//            product.Id,
//            product.Name,
//            product.StockQuantity
//        };

//        return Ok(ApiResponse.Success("Stock updated", result));
//    }

//}






using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Products;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/Products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly Cloudinary _cloudinary;

    public ProductsController(IProductRepository repo, IConfiguration config)
    {
        _repo = repo;

        var acc = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );

        _cloudinary = new Cloudinary(acc);
    }

    // ---------------- EXISTING ----------------


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repo.GetAllAsync();

        var result = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl
        });

        return Ok(ApiResponse.Success("Products fetched", result));
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
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl
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
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl
        });

        return Ok(ApiResponse.Success("Products fetched", result));
    }

    // ---------------- ADMIN ----------------

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
            Name = dto.Name.Trim(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category,
            ImageUrl = upload.SecureUrl.ToString(),
            IsActive = true
        };

        await _repo.AddAsync(product);
        return Ok(ApiResponse.Success("Product created", product));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/Toggle/{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.IsActive = !p.IsActive;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Product toggled", p));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Admin/Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.IsActive = false;
        await _repo.UpdateAsync(p);
        return Ok(ApiResponse.Success("Product deleted"));
    }

    // ---------------- USER ----------------

    [Authorize]
    [HttpGet("User/GetAll")]
    public async Task<IActionResult> UserGetAll()
    {
        var items = (await _repo.GetAllAsync()).Where(x => x.IsActive);
        return Ok(ApiResponse.Success("Active products", items));
    }

    // ---------------- SEARCH / FILTER / PAGED ----------------

    [HttpGet("Search")]
    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(ApiResponse.Success("Search result", new List<object>()));

        q = q.Trim();

        var all = await _repo.GetAllAsync();   // keep your repository

        var items = all
            .Where(x => x.IsActive &&
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    x.Category.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                ))
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                x.ImageUrl,
                x.StockQuantity,
                x.Category
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
            .Take(size);

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

        return Ok(ApiResponse.Success("Filtered products", items));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("Admin/Add_Stock")]
    public async Task<IActionResult> AddStock(int id, int qty)
    {
        var p = await _repo.GetByIdAsync(id);
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

        var p = await _repo.GetByIdAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        var upload = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(image.FileName, image.OpenReadStream()),
            Folder = "sportex_products"
        });

        p.ImageUrl = upload.SecureUrl.ToString();
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
        var p = await _repo.GetByIdAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.Name = dto.Name.Trim();
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

        var p = await _repo.GetByIdAsync(id);
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

        var p = await _repo.GetByIdAsync(id);
        if (p == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        p.StockQuantity = qty;
        await _repo.UpdateAsync(p);

        return Ok(ApiResponse.Success("Stock updated", p));
    }


}
