using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.ViewModels
{
    public class MovieVM
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CinemaId { get; set; }
        public int ProducerId { get; set; }
        public int CategoryId { get; set; }
        public List<int> ActorIds { get; set; }
    }
}
