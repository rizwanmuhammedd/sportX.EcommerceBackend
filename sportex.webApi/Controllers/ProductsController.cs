//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Sportex.Application.Common;
//using Sportex.Application.DTOs;
//using Sportex.Application.DTOs.Products;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;
//using Sportex.Domain.Enums;

//namespace Sportex.WebApi.Controllers;

//[ApiController]
//[Route("api/products")]
//[Authorize(Roles = "Admin")]
//public class ProductsController : ControllerBase
//{
//    private readonly IProductRepository _repo;
//    public ProductsController(IProductRepository repo) => _repo = repo;

//    // 1️⃣ GET ALL PRODUCTS (Public)
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

//    // 2️⃣ GET PRODUCT BY ID (Public)
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var product = await _repo.GetByIdAsync(id);
//        if (product == null)
//            return NotFound(ApiResponse.Fail(404, "Product not found"));

//        return Ok(ApiResponse.Success("Product found", product));
//    }

//    // 3️⃣ ADD NEW PRODUCT (Admin only)
//    [Authorize]
//    [HttpPost]
//    public async Task<IActionResult> Add(CreateProductDto dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ApiResponse.Fail(400, "Invalid product data"));

//        var product = new Product
//        {
//            Name = dto.Name.Trim(),
//            Price = dto.Price,
//            StockQuantity = dto.StockQuantity,
//            Category = dto.Category,
//            ImageUrl = dto.ImageUrl
//        };

//        await _repo.AddAsync(product);
//        return Ok(ApiResponse.Success("Product added successfully"));
//    }

//    // 4️⃣ UPDATE STOCK
//    [Authorize]
//    [HttpPut("stock/{id}")]
//    public async Task<IActionResult> UpdateStock(int id, int quantity)
//    {
//        var product = await _repo.GetByIdAsync(id);
//        if (product == null)
//            return NotFound(ApiResponse.Fail(404, "Product not found"));

//        product.StockQuantity = quantity;
//        await _repo.UpdateAsync(product);

//        return Ok(ApiResponse.Success("Stock updated"));
//    }
//}







//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Sportex.Application.Common;
//using Sportex.Application.DTOs;
//using Sportex.Application.DTOs.Products;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;
//using Sportex.Domain.Enums;

//namespace Sportex.WebApi.Controllers;

//[ApiController]
//[Route("api/products")]
//public class ProductsController : ControllerBase   // 🔥 REMOVED Admin ONLY HERE
//{
//    private readonly IProductRepository _repo;
//    public ProductsController(IProductRepository repo) => _repo = repo;

//    // 1️⃣ GET ALL PRODUCTS (User + Admin)
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

//    // 2️⃣ GET PRODUCT BY ID (User + Admin)
//    [Authorize]
//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var product = await _repo.GetByIdAsync(id);
//        if (product == null)
//            return NotFound(ApiResponse.Fail(404, "Product not found"));

//        return Ok(ApiResponse.Success("Product found", product));
//    }

//    // 3️⃣ ADD NEW PRODUCT (Admin only)
//    [Authorize(Roles = "Admin")]
//    [HttpPost]
//    public async Task<IActionResult> Add(CreateProductDto dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ApiResponse.Fail(400, "Invalid product data"));

//        var product = new Product
//        {
//            Name = dto.Name.Trim(),
//            Price = dto.Price,
//            StockQuantity = dto.StockQuantity,
//            Category = dto.Category,
//            ImageUrl = dto.ImageUrl
//        };

//        await _repo.AddAsync(product);
//        return Ok(ApiResponse.Success("Product added successfully"));
//    }

//    // 4️⃣ UPDATE STOCK (Admin only)
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

//        return Ok(ApiResponse.Success("Stock updated", product));
//    }
//}








using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs;
using Sportex.Application.DTOs.Products;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Domain.Enums;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    public ProductsController(IProductRepository repo) => _repo = repo;

    // 1️⃣ GET ALL PRODUCTS (User + Admin)
    [Authorize]
    [HttpGet]
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

    // 2️⃣ GET PRODUCT BY ID (User + Admin)
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        return Ok(ApiResponse.Success("Product found", product));
    }

    // 🔥 NEW: GET PRODUCTS BY CATEGORY (User + Admin)
    [Authorize]
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var products = await _repo.GetByCategoryAsync(category);
        return Ok(ApiResponse.Success("Products fetched", products));
    }

    // 3️⃣ ADD NEW PRODUCT (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail(400, "Invalid product data"));

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Category = dto.Category,
            ImageUrl = dto.ImageUrl
        };

        await _repo.AddAsync(product);
        return Ok(ApiResponse.Success("Product added successfully", product));
    }

    // 4️⃣ UPDATE STOCK (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPut("stock/{id}")]
    public async Task<IActionResult> UpdateStock(int id, int quantity)
    {
        if (quantity < 0)
            return BadRequest(ApiResponse.Fail(400, "Stock cannot be negative"));

        var product = await _repo.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse.Fail(404, "Product not found"));

        product.StockQuantity = quantity;
        await _repo.UpdateAsync(product);

        return Ok(ApiResponse.Success("Stock updated", product));
    }
}
