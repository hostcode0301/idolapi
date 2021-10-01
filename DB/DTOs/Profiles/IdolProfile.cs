using AutoMapper;
using idolapi.DB.DTOs;
using idolapi.DB.Models;

namespace idolapi.DB.Profiles
{
    public class IdolProfile : Profile
    {
        public IdolProfile()
        {
            CreateMap<Idol, IdolDTO>();
            CreateMap<IdolInput, Idol>();
        }
    }
}