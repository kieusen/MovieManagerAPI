using MovieManagerAPI.Data.Base;
using MovieManagerAPI.Data.ViewModels;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.Services
{
    public interface IMoviesService : IEntityBaseRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetAllMovieAsync();
        Task<Movie> GetMovieByIdAsync(int id);
        Task AddMovieAsync(MovieVM movieVM);
        Task UpdateMovieAsync(int id, MovieVM movieVM);       

    }
}
