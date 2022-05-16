using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieManagerAPI.Data.Services;
using MovieManagerAPI.Data.ViewModels;
using MovieManagerAPI.Data.ViewModels.Authentication;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IMoviesService _moviesService;
        private readonly IMapper _mapper;
        public MoviesController(ILogger<MoviesController> logger, IMoviesService service, IMapper mapper)
        {
            _logger = logger;
            _moviesService = service;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Get all movies");
                var movies = await _moviesService.GetAllMovieAsync();
                return Ok(movies);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the movies");
            }

        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            
            var movie = await _moviesService.GetMovieByIdAsync(id);            

            if (movie != null) return Ok(movie);

            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] MovieVM movieVM)
        {
            try
            {
                await _moviesService.AddMovieAsync(movieVM);

                return Created(nameof(AddAsync), movieVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] MovieVM movieVM)
        {
            try
            {
                var movie = await _moviesService.GetByIdAsync(id);
                if (movie == null) return NotFound();   

                await _moviesService.UpdateMovieAsync(id, movieVM);

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var movie = await _moviesService.GetByIdAsync(id);
                if (movie == null) return NotFound();

                await _moviesService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
