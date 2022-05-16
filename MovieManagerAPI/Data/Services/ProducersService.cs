using MovieManagerAPI.Data.Base;
using MovieManagerAPI.Data.Helpers;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.Services
{
    public class ProducersService : EntityBaseRepository<Producer>, IProducersService
    {
        private readonly AppDbContext _context;
        public ProducersService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Producer> GetAll(string search, string sortBy, int pageIndex = 1)
        {
            var producers = _context.Producers.AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(search))
                producers = producers.Where(p =>
                    p.Name.Contains(search) || p.Bio.Contains(search));

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "name_desc":
                        producers = producers.OrderByDescending(p => p.Name);
                        break;
                    default:
                        producers = producers.OrderBy(p => p.Name);
                        break;
                }
            }

            // Paging
            int pageSize = 5;
            var result = PaginatedList<Producer>.Create(producers, pageIndex, pageSize);

            return result;
        }
    }
}
