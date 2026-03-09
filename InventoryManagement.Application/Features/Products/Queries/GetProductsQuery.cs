namespace InventoryManagement.Application.Features.Products.Queries;

public class GetProductsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
}