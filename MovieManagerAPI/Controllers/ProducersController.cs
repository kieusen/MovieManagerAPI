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
    public class ProducersController : ControllerBase
    {
        private readonly ILogger<ProducersController> _logger;
        private readonly IProducersService _producersService;
        private readonly IMapper _mapper;
        public ProducersController(ILogger<ProducersController> logger, IProducersService service, IMapper mapper)
        {
            _logger = logger;
            _producersService = service;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll(string search, string sortBy, int pageIndex = 1)
        {
            try
            {
                _logger.LogInformation("Get all producers");
                var producers = _producersService.GetAll(search, sortBy, pageIndex);
                return Ok(producers);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the producers");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            
            var producer = await _producersService.GetByIdAsync(id);            

            if (producer != null) return Ok(producer);

            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] ProducerVM producerVM)
        {
            try
            {
                var newProducer = _mapper.Map<ProducerVM, Producer>(producerVM);

                await _producersService.AddAsync(newProducer);

                return Created(nameof(AddAsync), newProducer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProducerVM producerVM)
        {
            try
            {
                var producer = await _producersService.GetByIdAsync(id);
                if (producer == null) return NotFound();

                producer = _mapper.Map<ProducerVM, Producer>(producerVM);
                producer.Id = id;

                await _producersService.UpdateAsync(producer);

                return Ok(producer);
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
                await _producersService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
