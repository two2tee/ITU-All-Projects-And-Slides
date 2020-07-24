// WorkoutProfile.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Data.Entities;
using LiftLog.Web.ViewModels.WorkoutViewModels;

namespace LiftLog.Web.Mappings
{
    public class WorkoutProfile : Profile
    {
        /// <summary>
        /// Automapper profiles for workout
        /// </summary>
        public WorkoutProfile()
        {
            CreateMap<WorkoutEntryDto, WorkoutEntry>().ReverseMap();
            CreateMap<NewWorkoutEntryDto, WorkoutEntry>();
            CreateMap<WorkoutDto, Workout>().ReverseMap();
            CreateMap<AddWorkoutEntryToWorkoutDto, WorkoutEntry>();
            CreateMap<Workout, PortableWorkoutDto>();
            CreateMap<WorkoutEntry, PortableWorkoutEntryDto>();
            CreateMap<NewWorkoutDto, WorkoutViewModel>().ReverseMap();
            CreateMap<NewWorkoutEntryDto, WorkoutEntryViewModel>().ReverseMap();
            CreateMap<WorkoutDto, WorkoutViewModel>().ReverseMap()
                .ForMember(dest => dest.WorkoutEntryDtos, opt => opt.MapFrom(src => src.WorkoutEntries)).ReverseMap();
            CreateMap<Workout, WorkoutViewModel>().ReverseMap();
            CreateMap<WorkoutViewModel, NewWorkoutEntryDto>();
            CreateMap<WorkoutEntryDto, WorkoutEntryViewModel>();
            CreateMap<WorkoutEntryDto, ShareWorkoutEntryViewModel>();
        }
    }
}