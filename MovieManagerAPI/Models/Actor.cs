using MovieManagerAPI.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieManagerAPI.Models
{
    public class Actor : IEntityBase
    {
        public int Id { get; set; }        
        public string Name { get; set; }      
        public string ImageUrl { get; set; }       
        public string Bio { get; set; }

        [JsonIgnore]
        public List<Movie_Actor> Movies_Actors { get; set; }
    }
}
