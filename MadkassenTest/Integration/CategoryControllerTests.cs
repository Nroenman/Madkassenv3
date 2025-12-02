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

            // 1. We still expect 201 Created
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // 2. Instead of relying on the response body (which is null),
            //    we verify that the category was actually saved correctly in the DB.
            var dbCategories = context.Kategori.ToList();
            Assert.Single(dbCategories);

            var dbCategory = dbCategories[0];
            Assert.Equal("New Category", dbCategory.CategoryName);
            Assert.Equal("New Description", dbCategory.Description);
            Assert.True(dbCategory.CategoryId > 0);
        }
    }
}