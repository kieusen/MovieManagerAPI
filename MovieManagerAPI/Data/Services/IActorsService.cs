using MovieManagerAPI.Data.Base;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.Services
{
    public interface IActorsService : IEntityBaseRepository<Actor>
    {
        List<Actor> GetAll(string search, string sortBy, int pageIndex = 1);
    }
}
