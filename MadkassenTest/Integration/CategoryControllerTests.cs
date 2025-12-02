using System.Net;
using System.Net.Http.Json;
using ClassLibrary;
using MadkassenRestAPI.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MadkassenTest.Integration
{
    public class CategoryControllerTests(CustomWebApplicationFactory factory)
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();
        private readonly CustomWebApplicationFactory _factory = factory;

        private ApplicationDbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private async Task<int> SeedDatabase()
        {
            var context = GetDbContext();
            context.Kategori.RemoveRange(context.Kategori);
            await context.SaveChangesAsync();

            var category = new Kategori
            {
                CategoryName = "Test Category",
                Description = "Test Description"
            };
            context.Kategori.Add(category);
            await context.SaveChangesAsync();

            return category.CategoryId;
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOkAndCategories()
        {
            await SeedDatabase();

            var response = await _client.GetAsync("/api/Category");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var categories = await response.Content.ReadFromJsonAsync<List<Kategori>>();

            Assert.NotNull(categories);
            Assert.Equal("Test Category", categories[0].CategoryName);
            Assert.Equal("Test Description", categories[0].Description);
        }

        [Fact]
        public async Task GetCategory_ReturnsOkAndCategory()
        {
            var categoryId = await SeedDatabase();

            var response = await _client.GetAsync($"/api/Category/{categoryId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var category = await response.Content.ReadFromJsonAsync<Kategori>();
            Assert.NotNull(category);
            Assert.Equal("Test Category", category.CategoryName);
            Assert.Equal("Test Description", category.Description);
        }

        [Fact]
        public async Task PostCategory_ReturnsCreatedCategory()
        {
            var context = GetDbContext();
            context.Kategori.RemoveRange(context.Kategori);
            await context.SaveChangesAsync();

            var newCategory = new Kategori
            {
                CategoryName = "New Category",
                Description = "New Description"
            };

            var response = await _client.PostAsJsonAsync("/api/Category", newCategory);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdCategory = await response.Content.ReadFromJsonAsync<Kategori>();
            Assert.NotNull(createdCategory);
            Assert.Equal("New Category", createdCategory.CategoryName);
            Assert.Equal("New Description", createdCategory.Description);
            Assert.True(createdCategory.CategoryId > 0);

            var verifyContext = GetDbContext();
            var dbCategory = await verifyContext.Kategori.FindAsync(createdCategory.CategoryId);
            Assert.NotNull(dbCategory);
            Assert.Equal("New Category", dbCategory.CategoryName);
            Assert.Equal("New Description", dbCategory.Description);
        }
    }
}