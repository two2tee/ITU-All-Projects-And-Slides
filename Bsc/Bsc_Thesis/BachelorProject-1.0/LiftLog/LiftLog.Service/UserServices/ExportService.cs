// ExportService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.Interfaces;
using LiftLog.Service.Utilities;
using Microsoft.Extensions.Logging;

namespace LiftLog.Service.UserServices
{
    /// <summary>
    ///     This is a concrete implemention of an export service
    /// </summary>
    public class ExportService : BaseService, IExportService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IFIleProvider<PortableUserDto> _fileProvider;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IWorkoutRepository _workoutRepository;

        public ExportService(ILoggerFactory logger, IUserRepository userRepository, IMapper mapper,
            IFIleProvider<PortableUserDto> fileProvider, IWorkoutRepository workoutRepository,
            IExerciseRepository exerciseRepository)
        {
            Logger = logger.CreateLogger<ExportService>();
            _userRepository = userRepository;
            _mapper = mapper;
            _fileProvider = fileProvider;
            _workoutRepository = workoutRepository;
            _exerciseRepository = exerciseRepository;
        }

        /// <summary>
        ///     Returns a portable object with all user data
        ///     Please note that the user token will be updated for each request and thus
        ///     the token can only be used once before the user needs to request another token.
        ///     This is created as a safeguard against unauthorized data calls.
        /// </summary>
        /// <param name="userToken">unique user token</param>
        /// <returns>portable object of user data</returns>
        public async Task<PortableUserDto> ExportToPortableObjectAsync(Guid userToken)
        {
            //Retrieve User
            var user = await _userRepository.GetUserWithToken(userToken);

            if (user == null)
            {
                LogError("ExportToPortableObjectAsync", $"No user found with token{userToken}");
                return null;
            }

            //Convert to Dto
            var dto = MakePortableUserDto(user);

            if (dto == null)
            {
                LogError("ExportToPortableObjectAsync", $"Failed to export user with id {user.Id}. Null was returned");
                return null;
            }

            //Return dto
            LogCritical("ExportToPortableObjectAsync", $"Exported all user data for user with Id {user.Id}");
            await RefreshToken(user.Id);
            return dto;
        }

        /// <summary>
        ///     This method wille create a fileDTO with pdf bytes.
        ///     GUIDE: https://stackoverflow.com/questions/36983300/export-to-pdf-using-asp-net-5/42023039#42023039
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<FileDto> ExportToFileAsync(int userId)
        {
            //validate data
            if (userId < 1)
            {
                LogInvalidIdError("ExportToPortableObjectAsync", userId);
                return null;
            }

            //Retrieve User
            var user = await _userRepository.GetUser(userId);

            if (user == null)
            {
                LogNonExistingUserError("ExportToPortableObjectAsync", userId);
                return null;
            }

            //Make portable object to export
            var portable = MakePortableUserDto(user);

            if (portable == null)
            {
                LogError("ExportToFileAsync", "PortableObject was null. Please see logs for ExportToPortableObjectAsync");
                return null;
            }

            //Creates the pdf file
            var file = await _fileProvider.MakeFileAsync(portable);

            if (file == null || file.FileContent == null || file.FileContent.Length == 0)
            {
                LogError("ExportToFileAsync",
                    "The file was not created. Something went wrong during MakeFileAsync. Please check the provider");
                return null;
            }

            return file;
        }

        /// <summary>
        ///     Returns a token used to export data.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetExportTokenAsync(int userId)
        {
            if (userId < 0)
            {
                LogInvalidIdError("GetExportTokenAsync", userId);
                return null;
            }

            //Retrieve User
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                LogNonExistingUserError("GetExportTokenAsync", userId);
                return null;
            }
            if (user.ExportToken == default(Guid))
            {
                await RefreshToken(userId);
            }

            return user.ExportToken.ToString();
        }

        private async Task<bool> RefreshToken(int userId)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user == null) return false;

            user.ExportToken = Guid.NewGuid();

            var isSuccess = await _userRepository.UpdateAsync(user);
            if (isSuccess)
                LogCritical("RefreshToken", $"Refreshed token for user with id {userId}");
            else
                LogError("RefreshToken", $"Failed to refreshed token for user with id {userId}");
            return isSuccess;
        }


        private PortableUserDto MakePortableUserDto(User user)
        {
            var portableUserDto = _mapper.Map<PortableUserDto>(user);
            portableUserDto.CountryName = CountryConverter.GetCountryEnglish(user.Country);
            portableUserDto.SexType = SexConverter.GetSexEnglish(user.Sex);
            portableUserDto.ChallengeGiven = user.GivenChallenges.Count;
            portableUserDto.ChallengeReceived = user.ReceivedChallenges.Count;
            portableUserDto.RetrievalDate = DateTime.Now;
            portableUserDto.Workouts = MakePortableWorkoutDto(user.Workouts);
            portableUserDto.BirthDay = user.BirthDate;
            portableUserDto.CreationDate = user.CreationDate;


            return portableUserDto;
        }

        private ICollection<PortableWorkoutDto> MakePortableWorkoutDto(IEnumerable<Workout> workouts)
        {
            var dtos = new List<PortableWorkoutDto>();
            foreach (var workout in workouts)
            {
                var dto = _mapper.Map<PortableWorkoutDto>(workout);
                var entries = _workoutRepository.GetWorkoutWithEntries(workout.Id).Result;
                dto.WorkoutEntryDtos = MakePortableWorkoutEntryDto(entries.WorkoutEntries);
                dtos.Add(dto);
            }
            return dtos;
        }

        private ICollection<PortableWorkoutEntryDto> MakePortableWorkoutEntryDto(
            IEnumerable<WorkoutEntry> workoutEntries)
        {
            var dtos = new List<PortableWorkoutEntryDto>();
            foreach (var workoutEntry in workoutEntries)
            {
                var entry = _mapper.Map<PortableWorkoutEntryDto>(workoutEntry);
                entry.ExerciseName = _exerciseRepository.FindAsync(workoutEntry.ExerciseId).Result.Name;
                dtos.Add(entry);
            }
            return dtos;
        }
    }
}