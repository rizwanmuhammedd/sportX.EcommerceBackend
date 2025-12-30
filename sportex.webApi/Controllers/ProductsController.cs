using Microsoft.AspNetCore.Mvc;
using Sportex.Application.DTOs;
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

    // 1️⃣ GET ALL PRODUCTS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repo.GetAllAsync();
        return Ok(products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category.ToString(),
            ImageUrl = p.ImageUrl
        }));
    }

    // 2️⃣ GET PRODUCT BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return NotFound("Product not found");

        return Ok(product);
    }

    // 3️⃣ ADD NEW PRODUCT
    [HttpPost]
    public async Task<IActionResult> Add(Product product)
    {
        await _repo.AddAsync(product);
        return Ok("Product Added");
    }

    // 4️⃣ UPDATE STOCK (selling)
    [HttpPut("stock/{id}")]
    public async Task<IActionResult> UpdateStock(int id, int quantity)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return NotFound("Product not found");

        product.StockQuantity = quantity;
        await _repo.UpdateAsync(product);

        return Ok("Stock Updated");
    }
}
