using MovieManagerAPI.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MovieManagerAPI.Models
{
    public class Category : IEntityBase
    {
        public int Id { get; set; }        
        public string Name { get; set; }

        [JsonIgnore]
        public List<Movie> Movies { get; set; }
    }
}
