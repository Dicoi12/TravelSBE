using AutoMapper;
using TravelSBE.Entity;
using TravelSBE.Models;

namespace TravelSBE.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Objective, ObjectiveModel>();
            CreateMap<ObjectiveModel, Objective>();
            CreateMap<Event, EventModel>().ReverseMap();
            CreateMap<ObjectiveImage, ObjectiveImageModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}