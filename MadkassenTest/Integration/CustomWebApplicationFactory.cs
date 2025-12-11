using MadkassenRestAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MadkassenTest.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            var currentDirectory = Directory.GetCurrentDirectory();
            string backendPath;

            if (currentDirectory.Contains("IntegrationTest"))
            {
                var solutionDir = Directory.GetParent(currentDirectory);
                while (solutionDir != null && !Directory.Exists(Path.Combine(solutionDir.FullName, "MadkassenRestAPI")))
                {
                    solutionDir = solutionDir.Parent;
                }

                if (solutionDir != null)
                {
                    backendPath = Path.Combine(solutionDir.FullName, "MadkassenRestAPI");
                }
                else
                {
                    throw new InvalidOperationException("Could not find MadkassenRestAPI project directory");
                }
            }
            else
            {
                backendPath = currentDirectory;
            }

            builder.UseContentRoot(backendPath);

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName)
                        .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
                services.RemoveAll<IHostedService>();
            });
        }
    }
}