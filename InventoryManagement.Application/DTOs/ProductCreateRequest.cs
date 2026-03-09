using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Application.DTOs;

public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public IFormFile? Image { get; set; }
}