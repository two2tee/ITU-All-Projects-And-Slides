// ExerciseService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace LiftLog.Service.UserServices
{
    /// <summary>
    ///     This is a concrete implementation of an exercise service
    /// </summary>
    public class ExerciseService : BaseService, IExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IMapper _mapper;

        public ExerciseService(ILoggerFactory logger, IMapper mapper, IExerciseRepository exerciseRepository)
        {
            Logger = logger.CreateLogger<ExerciseService>();
            _mapper = mapper;
            _exerciseRepository = exerciseRepository;
            PopulateDatabase(); //Todo remove
        }

        public async Task<ICollection<ExerciseDto>> GetAllExercisesAsync()
        {
            var dtos = new List<ExerciseDto>();
            var entities = await _exerciseRepository.GetAllAsync();

            if (entities == null)
            {
                LogWarning("GetAllExercisesAsync", "No exercise found in database.");
                return dtos;
            }

            foreach (var exercise in entities)
                dtos.Add(_mapper.Map<ExerciseDto>(exercise));
            LogInformation("GetAllExercisesAsync", "Returned all exercises");
            return dtos;
        }


        /// <summary>
        ///     Private method only used to populate the database, Normally you will not be doing it like
        ///     this but use a different method of seeding the database
        /// </summary>
        /// <returns></returns>
        private async Task PopulateDatabase()
        {
            var entities = await _exerciseRepository.GetAllAsync();
            if (entities == null || !entities.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Name = "Bench Press",
                        Description = "Compound Exercise"
                    },
                    new Exercise
                    {
                        Name = "Squat",
                        Description = "Compound Exercise"
                    },
                    new Exercise
                    {
                        Name = "Deadlift",
                        Description = "Compound Exercise"
                    },
                    new Exercise
                    {
                        Name = "Shoulder press",
                        Description = "Compound Exercise"
                    }
                };

                foreach (var exercise in exercises)
                    await _exerciseRepository.CreateAsync(exercise);
                LogInformation("PopulateDatabase", "Database populated");
            }
            else
            {
                LogInformation("PopulateDatabase", "Database already populated");
            }
        }
    }
}