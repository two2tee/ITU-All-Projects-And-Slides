// WorkoutService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
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
    ///     This class is responible for creating, retriving workouts for users.
    /// </summary>
    public class WorkoutService : BaseService, IWorkoutService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;
        private readonly IWorkoutEntryRepository _workoutEntryRepository;
        private readonly IWorkoutRepository _workoutRepository;

        public WorkoutService(ILoggerFactory logger, IMapper mapper, IUserRepository userRepository,
            IWorkoutRepository workoutRepository, IExerciseRepository exerciseRepository,
            IWorkoutEntryRepository workoutEntryRepository)
        {
            Logger = logger.CreateLogger<WorkoutService>();
            _userRepository = userRepository;
            _workoutRepository = workoutRepository;
            _exerciseRepository = exerciseRepository;
            _mapper = mapper;
            _workoutEntryRepository = workoutEntryRepository;
        }

        /// <summary>
        ///     Save a new workout
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> AddWorkoutAsync(NewWorkoutDto dto)
        {
            if (dto == null || dto.UserId < 1)
            {
                LogError("AddWorkoutAsync", "Dto was invalid. Aborted workout creation");
                return -1;
            }

            var user = await _userRepository.GetUser(dto.UserId);
            if (user == null)
            {
                LogNonExistingUserError("AddWorkoutAsync", dto.UserId);
                return -1;
            }

            if (user.Workouts == null)
                user.Workouts = new List<Workout>();

            var workout = new Workout
            {
                User = user,
                Name = dto.Name,
                CreationDate = DateTime.Now
            };

            //user.Workouts.Add(workout);
            var workoutId = await _workoutRepository.CreateAsync(workout);

            LogInformation("AddWorkoutAsync", $"Workout was added for {dto.UserId}");
            return workoutId;
        }


        /// <summary>
        ///     Deletes a given workout
        ///     Uses userid for validation purposes
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <param name="workoutId">uid of workout</param>
        /// <returns></returns>
        public async Task<bool> DeleteWorkoutAsync(int userId, int workoutId)
        {
            if (workoutId < 1 || userId < 1)
            {
                LogError("DeleteWorkoutAsync",
                    $"Provided one or more Ids were invalid : userid {userId}, workoutId {workoutId} ");
                return false;
            }

            var user = await _userRepository.GetUser(userId);
            if (user == null) return false;
            LogNonExistingUserError("DeleteWorkoutAsync", userId);

            var isSuccess = await _workoutRepository.DeleteAsync(workoutId);
            if (isSuccess)
                LogCritical("DeleteWorkoutAsync", $"Deleted workout {workoutId} for user with id {userId}");
            else
                LogError("DeleteWorkoutAsync", $"Failed to workout {workoutId} for user with id {userId}");
            return isSuccess;
        }

        /// <summary>
        ///     Deletes all workouts for a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>if success</returns>
        public async Task<bool> DeleteAllWorkoutAsync(int userId)
        {
            if (userId < 1)
            {
                LogInvalidIdError(nameof(DeleteAllWorkoutAsync), userId);
                return false;
            }

            var user = await _userRepository.GetUser(userId);

            if (user == null)
            {
                LogNonExistingUserError(nameof(DeleteAllWorkoutAsync), userId);
                return false;
            }

            user.Workouts = new List<Workout>();

            var isSucces = await _userRepository.UpdateAsync(user);

            LogCritical(nameof(DeleteAllWorkoutAsync),
                isSucces
                    ? $"Deleted all challenges for User id {userId}"
                    : $"Failed to deleted all challenges for User id {userId}");

            return isSucces;
        }


        /// <summary>
        ///     Returns all workout for a given user.
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>workouts of user</returns>
        public async Task<ICollection<WorkoutDto>> GetAllWorkoutsAsync(int userId)
        {
            if (userId < 1)
            {
                LogInvalidIdError("GetAllWorkoutsAsync", userId);
                return null; //Invalid Id
            }

            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                LogNonExistingUserError("GetAllWorkoutsAsync", userId);
                return null;
            }

            LogInformation("GetAllWorkoutsAsync", $"Returned all workouts for user with id {userId}");
            return MapWorkout(user.Workouts);
        }

        //public Task<WorkoutDto> GetWorkoutAsync(int workoutId)
        //{
        //    var result = _workoutRepository.
        //}

        /// <summary>
        ///     rEturns all workouts that exist on a given date for a specific user
        /// </summary>
        /// <param name="userId">User id of user</param>
        /// <param name="requestDate">The date to compare</param>
        /// <returns>collection of workoutdtos</returns>
        public async Task<ICollection<WorkoutDto>> GetWorkoutOnDateAsync(int userId, DateTime requestDate)
        {
            if (userId < 1)
            {
                LogInvalidIdError("GetWorkoutOnDateAsync", userId);
                return null; //Invalid Id
            }
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                LogNonExistingUserError("GetWorkoutOnDateAsync", userId);
                return null;
            }

            //Select all workouts equal to requested date
            var entities = (
                from workout in user.Workouts
                where IsDate(workout.CreationDate, requestDate)
                select workout
            ).ToList();

            LogInformation("GetAllWorkoutsAsync",
                $"Returned all workouts for given date {requestDate} for user with id {userId}");
            return MapWorkout(entities);
        }

        /// <summary>
        ///     Add workout entry/entries to an existing workout
        /// </summary>
        /// <param name="workoutDto">dto</param>
        /// <returns>if add was successfull</returns>
        public async Task<int> AddWorkoutEntryAsync(AddWorkoutEntryToWorkoutDto workoutDto)
        {
            //Check if user exists
            var user = await _userRepository.GetUser(workoutDto.UserId);

            if (user == null)
            {
                LogNonExistingUserError("AddWorkoutEntryAsync", workoutDto.UserId);
                return 0;
            }

            //Get Workout
            var workout = await _workoutRepository.GetWorkoutWithEntries(workoutDto.WorkoutId);

            //Check if workout exists
            if (workout == null)
            {
                LogError("AddWorkoutEntryAsync", $"Workout with id {workoutDto.WorkoutId} did not exist in database");
                return 0;
            }

            //Convert Entries
            var entries = MakeNewWorkoutPersistantEntries(workoutDto.WorkoutEntryDtos);

            //Add entries to workout
            var counter = 0;
            foreach (var workoutEntry in entries)
            {
                workout.WorkoutEntries.Add(workoutEntry);
                counter = counter + 1;
            }

            //update
            var isSuccess = await _userRepository.UpdateAsync(user);

            //Return number of added entries of 0 if fail
            if (isSuccess)
                LogInformation("AddWorkoutEntryAsync",
                    $"User with id {workoutDto.UserId} has added {counter} entries to workout it id {workoutDto.WorkoutId}");
            else
                LogError("AddWorkoutEntryAsync",
                    $"Failed to add {counter} entries to workout with id {workoutDto.WorkoutId} for user with id {workoutDto.UserId}");
            return !isSuccess ? 0 : counter;
        }

        /// <summary>
        ///     Deletes a given WorkoutEntry
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteWorkoutEntryAsync(int userId, int entryId)
        {
            if (entryId < 1 || userId < 1)
            {
                LogError("DeleteWorkoutEntryAsync",
                    $"Provided one or more Ids were invalid : userid {userId}, entryId {entryId} ");
                return false;
            }

            var user = _userRepository.GetUser(userId);
            if (user == null)
            {
                LogNonExistingUserError("DeleteWorkoutEntryAsync", userId);
                return false;
            }

            var isSuccess = await _workoutEntryRepository.DeleteAsync(entryId);
            if (isSuccess)
                LogInformation("DeleteWorkoutEntryAsync", $"Deleted workoutEntry {entryId} from user {userId}");
            else
                LogError("DeleteWorkoutEntryAsync", $"Failed top delete workoutEntry {entryId} from user {userId}");
            return isSuccess;
        }

        /// <summary>
        ///     Gets a specific workout
        /// </summary>
        /// <param name="workoutId">ùid of workout</param>
        /// <returns>workout dto</returns>
        public async Task<WorkoutDto> GetWorkoutAsync(int workoutId)
        {
            if (workoutId < 1)
            {
                LogInvalidIdError("GetWorkoutAsync", workoutId);
                return null;
            }
            var workout = await _workoutRepository.GetWorkoutWithEntries(workoutId);

            if (workout == null)
            {
                LogError("GetWorkoutAsync", "Workout was null");
                return null;
            }

            var workoutDto = _mapper.Map<WorkoutDto>(workout);
            workoutDto.WorkoutEntryDtos = MapWorkoutEntries(workout.WorkoutEntries);

            LogInformation("GetWorkoutAsync", $"Workout with id {workoutId} was retrieved");
            return workoutDto;
        }

        /// <summary>
        ///     Gets a specific workout entry
        /// </summary>
        /// <param name="entryId">uid of workout entry</param>
        /// <returns></returns>
        public async Task<WorkoutEntryDto> GetWorkoutEntryAsync(int entryId)
        {
            if (entryId < 1)
            {
                LogInvalidIdError(nameof(GetWorkoutEntryAsync), entryId);
                return null;
            }
            var entry = await _workoutEntryRepository.GetWorkoutEntry(entryId);
            if (entry == null)
            {
                LogError(nameof(GetWorkoutEntryAsync), $"Workout entry with id {entryId} was null");
                return null;
            }

            var dto = _mapper.Map<WorkoutEntryDto>(entry);
            dto.ExerciseName = entry.Exercise.Name;
            dto.ExerciseId = entry.Exercise.Id;

            LogInformation(nameof(GetWorkoutEntryAsync), $"Workout entry with id {entryId} was retrieved");
            return dto;
        }

        /// <summary>
        ///     Gets all workout entries for a specific workout
        /// </summary>
        /// <param name="workoutId"></param>
        /// <returns></returns>
        public async Task<ICollection<WorkoutEntryDto>> GetWorkoutEntriesAsync(int workoutId)
        {
            if (workoutId < 1)
            {
                LogInvalidIdError("GetWorkoutWithEntries", workoutId);
                return null;
            }
            var result = await _workoutRepository.GetWorkoutWithEntries(workoutId);

            if (result == null)
            {
                LogError("GetWorkoutWithEntries", "Workout was null");
                return null;
            }

            LogInformation("GetWorkoutWithEntries", $"Workout with id {workoutId} was retrieved");
            var dto = MapWorkoutEntries(result.WorkoutEntries);
            return dto;
        }


        /// <summary>
        ///     Converts dto to persistant entity
        /// </summary>
        /// <param name="workoutEntryDtos"></param>
        /// <returns></returns>
        private ICollection<WorkoutEntry> MakeNewWorkoutPersistantEntries(
            IEnumerable<NewWorkoutEntryDto> workoutEntryDtos)
        {
            var workoutEntries = new List<WorkoutEntry>();
            foreach (var newWorkoutEntryDto in workoutEntryDtos)
            {
                var entity = _mapper.Map<WorkoutEntry>(newWorkoutEntryDto);

                workoutEntries.Add(entity);
            }
            return workoutEntries;
        }

        /// <summary>
        ///     Custom mapping of workouts
        /// </summary>
        /// <param name="workouts"></param>
        /// <returns>Dto</returns>
        private ICollection<WorkoutDto> MapWorkout(ICollection<Workout> workouts)
        {
            var dtos = new List<WorkoutDto>();
            foreach (var workout in workouts)
            {
                var workoutDto = _mapper.Map<WorkoutDto>(workout);
                workoutDto.WorkoutEntryDtos = GetWorkoutEntriesAsync(workout.Id).Result;
                dtos.Add(workoutDto);
            }
            return dtos;
        }


        /// <summary>
        ///     Custom workout entry mapping
        /// </summary>
        /// <param name="workoutEntries"></param>
        /// <returns>Dto</returns>
        private ICollection<WorkoutEntryDto> MapWorkoutEntries(ICollection<WorkoutEntry> workoutEntries)
        {
            var dtos = new List<WorkoutEntryDto>();
            foreach (var workoutEntry in workoutEntries)
            {
                var entryDto = _mapper.Map<WorkoutEntryDto>(workoutEntry);

                var exercise = _exerciseRepository.FindAsync(workoutEntry.ExerciseId).Result;
                if (exercise == null)
                {
                    LogError("MapWorkoutEntries", $"exercise was null for exercise id {entryDto.ExerciseId}");
                    continue;
                }
                entryDto.ExerciseName = exercise.Name;
                dtos.Add(entryDto);
            }

            return dtos;
        }

        private bool IsDate(DateTime date1, DateTime date2)
        {
            return date1.Day == date2.Day && date1.Month == date2.Month && date1.Year == date2.Year;
        }
    }
}