using AutoMapper;
using DatingApp.Model.Entity;
using DatingApp.Model.Photo;
using DatingApp.Model.User;
using System;
using System.Linq;

namespace DatingApp.Mapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserListDto>()
                .ForMember(d=>d.PhotoUrl, s => {
                    s.MapFrom(src => src.Photos.FirstOrDefault(f => f.IsMain).Url);
                })
                .ForMember(d=>d.Age, s=> {
                    s.MapFrom(s => CalculateAge(s.DateOfBirth));
                });

            CreateMap<User, UserDataDto>()
                .ForMember(d => d.PhotoUrl, s => {
                    s.MapFrom(src => src.Photos.FirstOrDefault(f => f.IsMain).Url);
                })
                .ForMember(d => d.Age, s => {
                    s.MapFrom(s => CalculateAge(s.DateOfBirth));
                });

            CreateMap<Photo, PhotoDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserRegisterDto, User>();
        }

        public static int CalculateAge(DateTime date)
        {
            var age = DateTime.Today.Year - date.Year;
            if (date.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }
    }
}
