using AutoMapper;
using TravelSBE.Models;
using TravelSBE.Entity;

namespace TravelSBE.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Objective, ObjectiveModel>();
            CreateMap<ObjectiveModel, Objective>();
        }
    }
}