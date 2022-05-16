using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tao khoa Movie_Actor
            builder.Entity<Movie_Actor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId});
            builder.Entity<Movie_Actor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.Movies_Actors)
                .HasForeignKey(ma => ma.MovieId);
            builder.Entity<Movie_Actor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.Movies_Actors)
                .HasForeignKey(ma => ma.ActorId);
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Movie_Actor> Movies_Actors { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
