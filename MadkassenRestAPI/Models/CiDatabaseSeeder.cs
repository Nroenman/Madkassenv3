using System;
using System.Linq;
using ClassLibrary.Model;
using BCrypt.Net;
using ClassLibrary;

namespace MadkassenRestAPI.Models
{
    public static class CiDatabaseSeeder
    {
        public static void Seed(ApplicationDbContext ctx)
        {

            if (!ctx.Kategori.Any())
            {
                var categories = new[]
                {
                    new Kategori { CategoryName = "Mejeri", Description = "Mælk, Ost, Æg osv" },
                    new Kategori { CategoryName = "Frugter", Description = "Friske frugter" },
                    new Kategori
                        { CategoryName = "Kød", Description = "Oksekød, Svinekød, Kylling og hvad man ellers begærer" },
                    new Kategori
                        { CategoryName = "Grøntsager", Description = "Friske grøntsager som salat, kartofler og løg" },
                    new Kategori { CategoryName = "Bagværk", Description = "Brød, kager og andre bagte varer" },
                    new Kategori
                        { CategoryName = "Drikkevarer", Description = "Juice, sodavand, vand og andre drikke" },
                    new Kategori { CategoryName = "Snacks", Description = "Chips, kiks og andre små lækkerier" },
                    new Kategori
                        { CategoryName = "Fisk og Skaldyr", Description = "Laks, rejer, torsk og andre fisketyper" },
                    new Kategori { CategoryName = "Krydderier", Description = "Salt, peber og andre krydderier" },
                    new Kategori { CategoryName = "Konserves", Description = "Dåsemad som bønner, tomater og supper" },
                    new Kategori { CategoryName = "Færdigretter", Description = "Klar-til-spisning måltider" },
                    new Kategori
                    {
                        CategoryName = "Kolonialvarer", Description = "Basisingredienser som mel, sukker og krydderier"
                    },
                    new Kategori { CategoryName = "Slik og Chokolade", Description = "Alt til den søde tand" },
                    new Kategori
                    {
                        CategoryName = "Nødder og Tørret Frugt", Description = "Sunde snacks som mandler og rosiner"
                    },
                    new Kategori { CategoryName = "Supper og Saucer", Description = "Færdige supper og saucer" },
                    new Kategori
                    {
                        CategoryName = "Morgenmad og Cerealer",
                        Description = "Cornflakes, havregryn og andet til morgenbordet"
                    },
                    new Kategori { CategoryName = "Ris og Pasta", Description = "Forskellige typer ris og pasta" },
                    new Kategori
                        { CategoryName = "Olier og Eddiker", Description = "Madlavningsolier og eddiker til dressing" },
                    new Kategori { CategoryName = "Frostvarer", Description = "Frosne grøntsager, pizzaer og is" },
                    new Kategori { CategoryName = "Vegetariske Produkter", Description = "Kødfrie alternativer" },
                    new Kategori
                        { CategoryName = "Veganerprodukter", Description = "Produkter uden animalske ingredienser" },
                    new Kategori { CategoryName = "Glutenfri Produkter", Description = "Specialprodukter uden Gluten" },
                    new Kategori { CategoryName = "Laktosefri Produkter", Description = "Mælkeprodukter uden laktose" }
                };

                ctx.Kategori.AddRange(categories);
                ctx.SaveChanges();
            }

            var mejeri = ctx.Kategori.First(c => c.CategoryName == "Mejeri");
            var frugter = ctx.Kategori.First(c => c.CategoryName == "Frugter");
            var koed = ctx.Kategori.First(c => c.CategoryName == "Kød");
            var glutenfri = ctx.Kategori.First(c => c.CategoryName == "Glutenfri Produkter");
            var noedderCat = ctx.Kategori.First(c => c.CategoryName == "Nødder og Tørret Frugt");
            var fiskSkal = ctx.Kategori.First(c => c.CategoryName == "Fisk og Skaldyr");


            if (!ctx.Produkter.Any())
            {
                ctx.Produkter.AddRange(
                    new Produkter
                    {
                        ProductName = "Mælk",
                        Description = "Frisk økologisk mælk",
                        Price = 12.50m,
                        StockLevel = 200,
                        CategoryId = mejeri.CategoryId,
                        AllergyType = AllergyType.Laktose
                    },
                    new Produkter
                    {
                        ProductName = "Ost",
                        Description = "Mild gul ost",
                        Price = 25m,
                        StockLevel = 50,
                        CategoryId = mejeri.CategoryId,
                        AllergyType = AllergyType.Laktose
                    },
                    new Produkter
                    {
                        ProductName = "Æble",
                        Description = "Danske æbler",
                        Price = 3m,
                        StockLevel = 500,
                        CategoryId = frugter.CategoryId,
                        AllergyType = null
                    },
                    new Produkter
                    {
                        ProductName = "Oksekød",
                        Description = "Hakket oksekød 8-12%",
                        Price = 35m,
                        StockLevel = 80,
                        CategoryId = koed.CategoryId,
                        AllergyType = null
                    },
                    new Produkter
                    {
                        ProductName = "Glutenfri Brød",
                        Description = "Lækkert glutenfrit brød",
                        Price = 20m,
                        StockLevel = 40,
                        CategoryId = glutenfri.CategoryId,
                        AllergyType = null
                    },
                    new Produkter
                    {
                        ProductName = "Nøddeblanding",
                        Description = "Mandler, valnødder og cashews",
                        Price = 30m,
                        StockLevel = 60,
                        CategoryId = noedderCat.CategoryId,
                        AllergyType = null
                    },
                    new Produkter
                    {
                        ProductName = "Rejer",
                        Description = "Pillede rejer",
                        Price = 40m,
                        StockLevel = 25,
                        CategoryId = fiskSkal.CategoryId,
                        AllergyType = null
                    }
                );

                ctx.SaveChanges();
            }
            if (!ctx.Users.Any(x => x.Email.StartsWith("testuser")))
            {
                var now = DateTime.UtcNow;
                const string plainPassword = "TestPassword123!";

                for (int i = 1; i <= 100; i++)
                {
                    var email = $"testuser{i:D3}@test.dk";
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                    ctx.Users.Add(new Users
                    {
                        UserName = $"Test User {i}",
                        Email = email,
                        PasswordHash = hashedPassword,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Roles = "User"
                    });
                }

                ctx.SaveChanges();
            }
        }
    }
}
