using ProductService.DTOs;
using ProductService.Entities;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;

namespace ProductService.Services;

public class ProductServices
{
    private readonly AppDbContext _context;

    public ProductServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> CreateProduct(CreateProductDto dto)
    {
        var existing = await _context.Products.FirstOrDefaultAsync(p=>p.Name == dto.Name);
        if(existing != null) return null;

        if(dto.Price < 0 || dto.StockQuantity<0) return null;

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity

        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto
        {
             Id = product.Id,
          Name = product.Name,
          Category = product.Category,
          Description = product.Description,
          StockQuantity = product.StockQuantity,
          Price = product.Price
        };


    }
    public async Task<List<ProductDto>> GetAllProduct()
    {
        var getAll = await _context.Products
            .Select(p=>  new ProductDto
            {   
                Id =p.Id,
                Name = p.Name,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                Price = p.Price,
                Category = p.Category
            })
            .ToListAsync();

        return getAll;
    }

    public async Task<List<ProductDto>>GetProductsByCategory(string? category)
    {
        var products = await _context.Products
        .Where(p=>p.Category == category)
        .Select(p=> new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category
            
        })
        .ToListAsync();

        return products;
      
    }

    public async Task<ProductDto?> UpdateProduct(int id, CreateProductDto dto)
{
    if (dto.Price < 0 || dto.StockQuantity < 0)
        return null;

    var product = await _context.Products.FindAsync(id);

    if (product == null)
        return null;

    product.Name = dto.Name;
    product.Description = dto.Description;
    product.Category = dto.Category;
    product.Price = dto.Price;
    product.StockQuantity = dto.StockQuantity;

    await _context.SaveChangesAsync();

    return new ProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
   //     StockQuantity = product.StockQuantity,
        Category = product.Category
    };
}
    public async Task<bool> DeleteProduct(int id)
    {
        var product =await _context.Products.FindAsync(id);

        if(product == null)
        {
            return false;
        }
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
   
    }

    public async Task<ProductDto?> UpdateStock(int id , UpdateStockDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if(product == null)
        {
            return null;
        }

        int new_stock = dto.Quantity + product.StockQuantity;

        if(new_stock < 0)
        {
            return null;
        }
        product.StockQuantity = new_stock;
        await _context.SaveChangesAsync();

        return new ProductDto
        {
          Id = product.Id,
          Name = product.Name,
          Description = product.Description,
          Category = product.Category,
          Price = product.Price,
          StockQuantity = product.StockQuantity

        };

    }
}