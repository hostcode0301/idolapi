using AutoMapper;
using idolapi.DB.DTOs;
using idolapi.DB.Models;

namespace idolapi.DB.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<LoginDTO, User>();
            CreateMap<RegisterDTO, User>();
            CreateMap<User, UserDTO>();
        }
    }
}