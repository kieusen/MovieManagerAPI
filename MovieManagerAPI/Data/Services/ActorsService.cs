using Microsoft.EntityFrameworkCore;
using MovieManagerAPI.Data.Base;
using MovieManagerAPI.Data.Helpers;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.Services
{
    public class ActorsService : EntityBaseRepository<Actor>, IActorsService
    {
        private readonly AppDbContext _context;
        public ActorsService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Actor> GetAll(string search, string sortBy, int pageIndex = 1)
        {
            var actors = _context.Actors.AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(search))
                actors = actors.Where(a => 
                    a.Name.Contains(search) || a.Bio.Contains(search));

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        actors = actors.OrderByDescending(a => a.Name);
                        break;
                    default:
                        actors = actors.OrderBy(a => a.Name);
                        break;
                }
            }

            // Paging
            int pageSize = 5;
            var result = PaginatedList<Actor>.Create(actors, pageIndex, pageSize);

            return result;
        }
    }
}
