using System.Linq;
using AutoMapper;
using MariageApp.API.Dtos;
using MariageApp.API.Models;

namespace MariageApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForListDto>().
            ForMember(dest=>dest.PhotoUrl,opt=>{opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);}).ForMember(dest=>dest.Age,opt=>{opt.ResolveUsing(src=>src.DateOfBirth.CalculateAge());});
            CreateMap<User,UserForDetailsDto>().
            ForMember(dest=>dest.PhotoUrl,opt=>{opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);}).
            ForMember(dest=>dest.Age,opt=>{opt.ResolveUsing(src=>src.DateOfBirth.CalculateAge());});
            CreateMap<Photo,PhotoForDetailsDto>();
            CreateMap<UserForUpdateDto,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreateDto,Photo>();
            CreateMap<UserForRegisterDto,User>();
            
        }
    }
}