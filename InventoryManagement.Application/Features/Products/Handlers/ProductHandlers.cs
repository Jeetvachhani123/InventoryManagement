using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Features.Products.Handlers;

public class ProductHandlers
{
    private readonly IProductRepository _repository;
    private readonly IFileStorageService _fileStorage;

    public ProductHandlers(
        IProductRepository repository,
        IFileStorageService fileStorage)
    {
        _repository = repository;
        _fileStorage = fileStorage;
    }

    public async Task CreateAsync(CreateProductCommand command)
    {
        var product = new Product
        {
            Name = command.Name,
            Price = command.Price,
            Stock = command.Stock,
            ImageUrl = command.ImageUrl
        };

        await _repository.AddAsync(product);
    }

    public async Task<object> GetAsync(GetProductsQuery query)
    {
        var (items, total) = await _repository.GetAllAsync(
            query.Page, query.PageSize, query.Search);

        return new
        {
            TotalCount = total,
            Data = items
        };
    }
}
