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
    public class ActorsController : ControllerBase
    {
        private readonly ILogger<ActorsController> _logger;
        private readonly IActorsService _actorsService;
        private readonly IMapper _mapper;
        public ActorsController(ILogger<ActorsController> logger, IActorsService service, IMapper mapper)
        {
            _logger = logger;
            _actorsService = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Get all actors");
                var actors = await _actorsService.GetAllAsync();
                return Ok(actors);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the actors");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            
            var actor = await _actorsService.GetByIdAsync(id);            

            if (actor != null) return Ok(actor);

            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] ActorVM actorVM)
        {
            try
            {
                var newActor = _mapper.Map<ActorVM, Actor>(actorVM);

                await _actorsService.AddAsync(newActor);

                return Created(nameof(AddAsync), newActor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ActorVM actorVM)
        {
            try
            {
                var actor = await _actorsService.GetByIdAsync(id);
                if (actor == null) return NotFound();

                actor = _mapper.Map<ActorVM, Actor>(actorVM);
                actor.Id = id;

                await _actorsService.UpdateAsync(actor);

                return Ok(actor);
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
                await _actorsService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
