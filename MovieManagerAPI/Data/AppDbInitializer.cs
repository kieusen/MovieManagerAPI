using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MovieManagerAPI.Data.ViewModels.Authentication;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            // Category
            if (!context.Categories.Any())
            {
                var dataJson = await File.ReadAllTextAsync("Data/Seed/Categories.json");
                var data = JsonSerializer.Deserialize<List<Category>>(dataJson);
                foreach (var item in data)
                {
                    context.Categories.Add(item);
                }
                await context.SaveChangesAsync();
            }

            // Actor
            if (!context.Actors.Any())
            {
                var dataJson = await File.ReadAllTextAsync("Data/Seed/Actors.json");
                var data = JsonSerializer.Deserialize<List<Actor>>(dataJson);
                foreach (var item in data)
                {
                    context.Actors.Add(item);
                }
                await context.SaveChangesAsync();
            }

            // Producer
            if (!context.Producers.Any())
            {
                var dataJson = await File.ReadAllTextAsync("Data/Seed/Producers.json");
                var data = JsonSerializer.Deserialize<List<Producer>>(dataJson);
                foreach (var item in data)
                {
                    context.Producers.Add(item);
                }
                await context.SaveChangesAsync();
            }

            // Cinema
            if (!context.Cinemas.Any())
            {
                var dataJson = await File.ReadAllTextAsync("Data/Seed/Cinemas.json");
                var data = JsonSerializer.Deserialize<List<Cinema>>(dataJson);
                foreach (var item in data)
                {
                    context.Cinemas.Add(item);
                }
                await context.SaveChangesAsync();
            }

            // Movie
            if (!context.Movies.Any())
            {
                Random rnd = new Random();

                var dataJson = await File.ReadAllTextAsync("Data/Seed/Movies.json");
                var data = JsonSerializer.Deserialize<List<Movie>>(dataJson);
                foreach (var item in data)
                {
                    DateTime dateRamdom = DateTime.Now.AddDays(rnd.Next(1, 100));

                    item.StartDate = dateRamdom;
                    item.EndDate = dateRamdom.AddDays(rnd.Next(1, 100));

                    context.Movies.Add(item);
                }
                await context.SaveChangesAsync();
            }

            // Movie_Actor
            if (!context.Movies_Actors.Any())
            {
                var dataJson = await File.ReadAllTextAsync("Data/Seed/Movies_Actors.json");
                var data = JsonSerializer.Deserialize<List<Movie_Actor>>(dataJson);
                foreach (var item in data)
                {
                    context.Movies_Actors.Add(item);
                }
                await context.SaveChangesAsync();
            }

            // Roles
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            // User
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Tao Admin user
            string adminEmail = "admin@movie.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    FullName = "Admin User",
                    UserName = "admin-user",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newAdminUser, "Hello@1234");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
            }

            // Tao App user
            string appUserEmail = "user@movie.com";
            var appUser = await userManager.FindByEmailAsync(appUserEmail);
            if (appUser == null)
            {
                var newAppUser = new ApplicationUser
                {
                    FullName = "App User",
                    UserName = "app-user",
                    Email = appUserEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newAppUser, "Hello@1234");
                await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
            }
        }
    }
}
