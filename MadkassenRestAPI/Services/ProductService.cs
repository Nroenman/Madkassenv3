using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MadkassenRestAPI.Services;

public class ProductService(ApplicationDbContext context)
{
    public async Task<List<Produkter>> GetAllProductsAsync()
    {
        return await context.Produkter.ToListAsync();  // Fetches all products directly
    }

    public async Task<Produkter> GetProductByIdAsync(int id)
    {
        var product = await context.Produkter.FindAsync(id);
        if (product == null) 
        {
            return null; 
        }
        return product; 
    }

    public async Task<Produkter> AddProductAsync(Produkter product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        var existingProduct = await context.Produkter
            .FirstOrDefaultAsync(p => p.ProductName == product.ProductName);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Product with name {product.ProductName} already exists."); // Prevents adding duplicate products
        }

        context.Produkter.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<Produkter> UpdateProductStockAsync(int id, int quantity)
    {
        var product = await context.Produkter.FindAsync(id);
        if (product == null)
        {
            return null;
        }

        if (quantity < 0 && product.StockLevel < Math.Abs(quantity)) 
        {
            return null;
        }

        product.StockLevel += quantity;

        await context.SaveChangesAsync();
        return product;
    }

    public async Task<List<Produkter>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await context.Produkter
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Price)
            .ToListAsync();

        return products ?? new List<Produkter>();  
    }
  
}