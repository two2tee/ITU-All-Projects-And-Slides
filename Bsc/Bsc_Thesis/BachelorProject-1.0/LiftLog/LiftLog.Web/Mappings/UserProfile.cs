// UserProfile.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Data.Entities;
using LiftLog.Web.ViewModels.HomeViewModels;
using LiftLog.Web.ViewModels.ManageViewModels;

namespace LiftLog.Web.Mappings
{
    /// <summary>
    ///     Represents mapping profile used to convert different representations of the User model throughout the application
    ///     (entity, view model, dtos).
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<NewUserDto, User>();
            CreateMap<UserViewModel, UserDto>().ReverseMap();
            CreateMap<UserViewModel, NewUserDto>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, PortableUserDto>();
            CreateMap<User, IndexViewModel>().ReverseMap();
            CreateMap<IndexViewModel, ModifiedUserDto>();
            CreateMap<UserDto, IndexViewModel >().ReverseMap();

        }
    }
}