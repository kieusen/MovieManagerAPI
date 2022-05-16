using MovieManagerAPI.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieManagerAPI.Models
{
    public class Movie : IEntityBase
    {
        public int Id { get; set; }        
        public string Name { get; set; }      
        public string ImageUrl { get; set; }       
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CinemaId { get; set; }
        public Cinema Ciname { get; set; }

        public int ProducerId { get; set; }
        public Producer Producer { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [JsonIgnore]
        public List<Movie_Actor> Movies_Actors { get; set; }
    }
}
