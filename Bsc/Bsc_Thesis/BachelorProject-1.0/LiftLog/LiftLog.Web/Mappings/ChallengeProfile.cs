// ChallengeProfile.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Data.Entities;
using LiftLog.Web.ViewModels.ChallengeViewModels;
using LiftLog.Web.ViewModels.WorkoutViewModels;

namespace LiftLog.Web.Mappings
{
    /// <summary>
    ///     Automapper profiles for challenge
    /// </summary>
    public class ChallengeProfile : Profile
    {
        public ChallengeProfile()
        {
            CreateMap<NewChallengeDto, Challenge>();
            CreateMap<ChallengeDto, Challenge>().ReverseMap();
            CreateMap<ChallengeAbleUserDto, User>().ReverseMap();
            CreateMap<ChallengeDto, ChallengeResultViewModel>().ReverseMap();
            CreateMap<ChallengeResultViewModel, ChallengeResultDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ChallengeeId)).ReverseMap();
            CreateMap<WorkoutEntryDto, NewChallengeViewModel>();
            CreateMap<NewChallengeViewModel, NewChallengeDto>();
            CreateMap<ChallengeDto, ShareChallengeViewModel>();
        }
    }
}