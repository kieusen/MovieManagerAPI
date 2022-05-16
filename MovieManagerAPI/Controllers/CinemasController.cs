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
    public class CinemasController : ControllerBase
    {
        private readonly ILogger<CinemasController> _logger;
        private readonly ICinemasService _cinemasService;
        private readonly IMapper _mapper;
        public CinemasController(ILogger<CinemasController> logger, ICinemasService service, IMapper mapper)
        {
            _logger = logger;
            _cinemasService = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Get all cinemas");
                var cinemas = await _cinemasService.GetAllAsync();
                return Ok(cinemas);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the cinemas");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            
            var cinema = await _cinemasService.GetByIdAsync(id);            

            if (cinema != null) return Ok(cinema);

            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CinemaVM cinemaVM)
        {
            try
            {
                var newCinema = _mapper.Map<CinemaVM, Cinema>(cinemaVM);

                await _cinemasService.AddAsync(newCinema);

                return Created(nameof(AddAsync), newCinema);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CinemaVM cinemaVM)
        {
            try
            {
                var cinema = await _cinemasService.GetByIdAsync(id);
                if (cinema == null) return NotFound();

                cinema = _mapper.Map<CinemaVM, Cinema>(cinemaVM);
                cinema.Id = id;

                await _cinemasService.UpdateAsync(cinema);

                return Ok(cinema);
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
                await _cinemasService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
