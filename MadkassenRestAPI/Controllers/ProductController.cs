using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Model;
using MadkassenRestAPI.Services;
using System.Security.Claims;
using MadkassenRestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MadkassenRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ProductService productService, ApplicationDbContext context)
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produkter>>> GetAllProducts()
        {
            var products = await context.Produkter.ToListAsync();

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageUrl =
                        "https://i.imghippo.com/files/KCsO2582jBE.png"; // Apply placeholder if null or empty
                }
            }

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Produkter>> AddProduct(Produkter product)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token" });
            }

            if (string.IsNullOrEmpty(product.ImageUrl) || product.ImageUrl == "string")
            {
                product.ImageUrl = null; // Setting ImageUrl to null will trigger the default image in ComputedImageUrl
            }

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Produkter>> GetProduct(int id)
        {
            // Fetch product by its ID using the product service
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductStock(int id, [FromBody] UpdateStockRequest request)
        {
            var updatedProduct = await productService.UpdateProductStockAsync(id, request.Quantity);

            if (updatedProduct == null)
            {
                return BadRequest("Not enough stock available");
            }

            return Ok(updatedProduct);
        }
        // Endpoint to get products by category ID
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            // Check if the categoryId is valid (e.g., greater than 0)
            if (categoryId <= 0)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            var productsQuery = context.Produkter
                .Where(p => p.CategoryId == categoryId); // Filter by category

            productsQuery = productsQuery.OrderBy(p => p.Price);

            var products = await productsQuery
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound(new { message = "No products found in this category" });
            }

            // Return the products list with pagination details if necessary
            return Ok(products);
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            //var message = "Only admin have permission to delete products";

            if (string.IsNullOrEmpty(userId))
            { 
                return Unauthorized(new { message = "Invalid or missing user ID in token" });
            }

            /*if (userRole != "Admin")
            {
                return Forbid(message);
            }
            */
            var product = await context.Produkter.FindAsync(Id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            context.Produkter.Remove(product);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}