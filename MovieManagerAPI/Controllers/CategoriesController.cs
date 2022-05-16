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
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoriesService _categoriesService;
        private readonly IMapper _mapper;
        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesService service, IMapper mapper)
        {
            _logger = logger;
            _categoriesService = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Get all categorys");
                var categorys = await _categoriesService.GetAllAsync();
                return Ok(categorys);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the categorys");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            
            var category = await _categoriesService.GetByIdAsync(id);            

            if (category != null) return Ok(category);

            return NotFound();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CategoryVM categoryVM)
        {
            try
            {
                var newCategory = _mapper.Map<CategoryVM, Category>(categoryVM);

                await _categoriesService.AddAsync(newCategory);

                return Created(nameof(AddAsync), newCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CategoryVM categoryVM)
        {
            try
            {
                var category = await _categoriesService.GetByIdAsync(id);
                if (category == null) return NotFound();

                category = _mapper.Map<CategoryVM, Category>(categoryVM);
                category.Id = id;

                await _categoriesService.UpdateAsync(category);

                return Ok(category);
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
                await _categoriesService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
