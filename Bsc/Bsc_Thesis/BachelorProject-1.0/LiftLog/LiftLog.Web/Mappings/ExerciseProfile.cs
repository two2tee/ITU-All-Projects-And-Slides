// ExerciseProfile.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;

namespace LiftLog.Web.Mappings
{
    /// <summary>
    /// Automapper profiles for exercise
    /// </summary>
    public class ExerciseProfile : Profile
    {
        public ExerciseProfile()
        {
            CreateMap<ExerciseDto, Exercise>().ReverseMap();
        }
    }
}