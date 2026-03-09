namespace InventoryManagement.Application.Features.Products.Commands;

public class CreateProductCommand
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public string ImageUrl { get; set; } = null!;
}
