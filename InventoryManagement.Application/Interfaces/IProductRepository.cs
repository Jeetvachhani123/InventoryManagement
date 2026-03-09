using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<(IEnumerable<Product> Items, int TotalCount)>
        GetAllAsync(int page, int pageSize, string? search);
}