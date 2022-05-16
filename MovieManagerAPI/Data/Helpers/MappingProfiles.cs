using AutoMapper;
using MovieManagerAPI.Data.ViewModels;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Data.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Source, Targer
            CreateMap<ActorVM, Actor>();
            CreateMap<CinemaVM, Cinema>();
            CreateMap<ProducerVM, Producer>();
            CreateMap<CategoryVM, Category>();
            CreateMap<MovieVM, Movie>();
        }
    }
}
