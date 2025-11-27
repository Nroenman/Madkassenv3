using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace IntegrationTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                // Remove background services only (they hang tests)
                var hostedServices = services.Where(d => 
                    d.ServiceType == typeof(IHostedService)).ToList();
                foreach (var service in hostedServices)
                {
                    services.Remove(service);
                }
                
                // Keep SQL Server - use real database for integration tests
            });
        }
    }
}