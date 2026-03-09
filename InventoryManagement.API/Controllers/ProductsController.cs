using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Products.Handlers;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductHandlers _handlers;
    private readonly IFileStorageService _fileStorage;

    public ProductsController(
        ProductHandlers handlers,
        IFileStorageService fileStorage)
    {
        _handlers = handlers;
        _fileStorage = fileStorage;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
    {
        // ✅ Image validation
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest("Product image is required");

        // ✅ File type validation
        var allowedTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(request.Image.ContentType))
            return BadRequest("Only JPG or PNG images are allowed");

        // ✅ File size validation (2 MB)
        if (request.Image.Length > 2 * 1024 * 1024)
            return BadRequest("Image size must be less than 2MB");

        // ✅ Upload image
        var imageUrl = await _fileStorage
            .UploadAsync(request.Image.OpenReadStream(), request.Image.FileName);

        // ✅ Create command
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = imageUrl
        };

        await _handlers.CreateAsync(command);

        return Ok(new
        {
            message = "Product created successfully"
        });
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> Get([FromQuery] GetProductsQuery query)
    {
        var result = await _handlers.GetAsync(query);
        return Ok(result);
    }
}