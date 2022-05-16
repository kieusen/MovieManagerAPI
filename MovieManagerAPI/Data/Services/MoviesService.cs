using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Data.Base;
using MovieManagerAPI.Data.ViewModels;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.Services
{
    public class MoviesService : EntityBaseRepository<Movie>, IMoviesService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        
        public MoviesService(AppDbContext context, IMapper mappter) : base(context)
        {
            _context = context;
            _mapper = mappter;
        }

        public async Task AddMovieAsync(MovieVM movieVM)
        {
            var newMovie = _mapper.Map<MovieVM, Movie>(movieVM);
            await _context.Movies.AddAsync(newMovie);
            await _context.SaveChangesAsync();

            // Movie Actor
            for (int i = 0; i < movieVM.ActorIds.Count; i++)
            {
                var movie_actor = new Movie_Actor()
                {
                    MovieId = newMovie.Id,
                    ActorId = movieVM.ActorIds[i]
                };
                await _context.Movies_Actors.AddAsync(movie_actor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Movie>> GetAllMovieAsync()
        {
            var movies = await _context.Movies
                .Include(c => c.Ciname)
                .Include(p => p.Producer)
                .Include(c => c.Category)
                .Include(ma => ma.Movies_Actors).ThenInclude(a => a.Actor)
                .ToListAsync();

            return movies;
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            var movie = await _context.Movies
                .Include(c => c.Ciname)
                .Include(p => p.Producer)
                .Include(c => c.Category)
                .Include(ma => ma.Movies_Actors).ThenInclude(a => a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            return movie;
        }

        public async Task UpdateMovieAsync(int id, MovieVM movieVM)
        {
            Movie movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if (movie != null)
            {
                movie = _mapper.Map<Movie>(movieVM);
                await _context.SaveChangesAsync();


                // Xoa Movie Actor cu
                var movies_actors = _context.Movies_Actors.Where(ma => ma.MovieId == id);
                _context.Movies_Actors.RemoveRange(movies_actors);
                await _context.SaveChangesAsync();

                // Them Movie Actor moi
                foreach (var item in movieVM.ActorIds)
                {
                    var movie_actor = new Movie_Actor()
                    {
                        MovieId = id,
                        ActorId = item
                    };
                    await _context.Movies_Actors.AddAsync(movie_actor);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
