// ChallengeService.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Core.Enums;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LiftLog.Service.UserServices
{
    /// <summary>
    ///     This is a concret implementation of a challenge service.
    /// </summary>
    public class ChallengeService : BaseService, IChallengeService
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;


        public ChallengeService(ILoggerFactory logger, IMapper mapper, IUserRepository userRepository,
            IChallengeRepository challengeRepository, IExerciseRepository exerciseRepository,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _challengeRepository = challengeRepository;
            _exerciseRepository = exerciseRepository;
            _userManager = userManager;
            Logger = logger.CreateLogger<ChallengeService>();
        }

        /// <summary>
        ///     Sets the result for a given challenge
        /// </summary>
        /// <param name="dto">dto of object</param>
        /// <returns></returns>
        public async Task<bool> SetResultAsync(ChallengeResultDto dto)
        {
            //Validate data
            if (dto.ChallengeId < 1)
            {
                LogInvalidIdError(nameof(GetChallengeAsync), dto.ChallengeId);
                return false;
            }

            var challenge = await _challengeRepository.FindAsync(dto.ChallengeId);
            if (challenge == null)
            {
                LogError(nameof(SetResultAsync),
                    $"Failed to set result for challenge. Challenge with id {dto.ChallengeId} returned null");
                return false;
            }

            //Set challenge
            challenge.IsComplete = dto.IsComplete;
            challenge.ResultReps = !dto.IsComplete ? 0 : dto.ResultReps;

            var isSuccess = await _challengeRepository.UpdateAsync(challenge);

            if (isSuccess)
                LogInformation(nameof(SetResultAsync),
                    $"Updated challenge with id {dto.ChallengeId} by user with id {dto.UserId}");
            {
                LogError(nameof(SetResultAsync),
                    $"Failed to update challenge with id {dto.ChallengeId} by user with id {dto.UserId}");
            }
            return isSuccess;
        }

        /// <summary>
        ///     Gets a specific challenge
        /// </summary>
        /// <param name="challengeId">uid of challenge</param>
        /// <returns></returns>
        public async Task<ChallengeDto> GetChallengeAsync(int challengeId)
        {
            //validate data
            if (challengeId < 1)
            {
                LogInvalidIdError(nameof(GetChallengeAsync), challengeId);
                return null;
            }

            var challenge = await _challengeRepository.GetChallenge(challengeId);
            if (challenge == null)
            {
                LogError(nameof(GetChallengeAsync), $"Failed to get challenge with id{challengeId}. Null was received");
                return null;
            }

            //Return challengeDTO
            LogInformation(nameof(GetChallengeAsync), $"Returned challenge with id{challengeId}");
            var dto = _mapper.Map<ChallengeDto>(challenge);
            await SetDisplayNameAsync(dto);

            await SetExerciseNameAsync(dto);
            return dto;
        }

        /// <summary>
        ///     Get received challenges for a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>collection of dtos</returns>
        public async Task<ICollection<ChallengeDto>> GetReceivedChallengesAsync(int userId)
        {
            if (userId < 1)
            {
                LogInvalidIdError(nameof(GetReceivedChallengesAsync), userId);
                return null;
            }
            var user = await _userRepository.GetUser(userId);

            if (user == null)
            {
                LogNonExistingUserError(nameof(GetReceivedChallengesAsync), userId);
                return null;
            }

            LogInformation(nameof(GetReceivedChallengesAsync), $"Returned Received challenges for user id{userId}");
            var dto = _mapper.Map<ICollection<ChallengeDto>>(user.ReceivedChallenges);
            await SetDisplayNameAsync(dto);
            await SetExerciseNameAsync(dto);
            await SetChallengeAbleAsync(dto);
            return dto;
        }

        /// <summary>
        ///     Gets collections of given challenges for a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>collection of dto</returns>
        public async Task<ICollection<ChallengeDto>> GetGivenChallengesAsync(int userId)
        {
            //validate data
            if (userId < 1)
            {
                LogInvalidIdError(nameof(GetGivenChallengesAsync), userId);
                return null;
            }
            var user = await _userRepository.GetUser(userId);

            if (user == null)
            {
                LogNonExistingUserError(nameof(GetGivenChallengesAsync), userId);
                return null;
            }

            //return dtos
            LogInformation(nameof(GetGivenChallengesAsync), $"Returned given challenges for user id{userId}");
            var dto = _mapper.Map<ICollection<ChallengeDto>>(user.GivenChallenges);
            await SetDisplayNameAsync(dto);
            await SetExerciseNameAsync(dto);
            await SetChallengeAbleAsync(dto);
            return dto;
        }

        /// <summary>
        ///     Gets a list of users of is challengeable. That is a user who has given
        ///     challenge consent.
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>collection of userdtos</returns>
        public async Task<ICollection<ChallengeAbleUserDto>> GetChallengeAbleUsersAsync(int userId)
        {
            //validate data
            if (userId < 1)
            {
                LogInvalidIdError(nameof(GetChallengeAbleUsersAsync), userId);
                return null;
            }

            LogInformation(nameof(GetChallengeAbleUsersAsync), $"Returned challengeable users for user id{userId}");

            //Filtering out current user based on consent
            var users = await _userRepository.GetAllAsync();
            var challengees = new List<User>();
            foreach (var user in users)
                if (user.Id != userId && await IsChallenableAsync(user))
                    challengees.Add(user);

            return _mapper.Map<ICollection<ChallengeAbleUserDto>>(challengees);
        }

        /// <summary>
        ///     Deletes a given challenge
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <param name="challengeId">uid of challenge</param>
        /// <returns>if success</returns>
        public async Task<bool> DeleteChallengeAsync(int userId, int challengeId)
        {
            //Validate data
            if (challengeId < 1)
            {
                LogInvalidIdError(nameof(DeleteChallengeAsync), challengeId);
                return false;
            }

            //Delete
            LogInformation("DeleteChallengeAsync",
                $"Challenge with id {challengeId} was deleted by user with Id{userId}");
            return await _challengeRepository.DeleteAsync(challengeId);
        }

        /// <summary>
        ///     Deletes all challenges from a user
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>if success</returns>
        public async Task<bool> DeleteAllChallengesAsync(int userId)
        {
            //validate data
            if (userId < 1)
            {
                LogInvalidIdError(nameof(DeleteAllChallengesAsync), userId);
                return false;
            }

            var user = await _userRepository.GetUser(userId);

            if (user == null)
            {
                LogNonExistingUserError(nameof(DeleteAllChallengesAsync), userId);
                return false;
            }

            //Delete all data
            user.ReceivedChallenges = new List<Challenge>();
            user.GivenChallenges = new List<Challenge>();

            var isSucces = await _userRepository.UpdateAsync(user);

            LogCritical(nameof(DeleteAllChallengesAsync),
                isSucces
                    ? $"Deleted all challenges for User id {userId}"
                    : $"Failed to deleted all challenges for User id {userId}");

            return isSucces;
        }

        /// <summary>
        ///     Creates a new challenge between two users
        /// </summary>
        /// <param name="dto">dto with challenge information</param>
        /// <returns>uid of created challenge</returns>
        public async Task<int> CreateChallengeAsync(NewChallengeDto dto)
        {
            //validate data
            if (dto.ChallengeeId == dto.ChallengerId)
            {
                LogError(nameof(CreateChallengeAsync),
                    $"Failed to create because user ids where the same: {dto.ChallengeeId}{dto.ChallengerId}");
                return -1;
            }

            var challenger = await _userRepository.GetUser(dto.ChallengerId);
            var challengee = await _userRepository.GetUser(dto.ChallengeeId);
            var exercise = await _exerciseRepository.FindAsync(dto.ExerciseId);

            if (challenger == null || challengee == null || exercise == null)
            {
                LogError(nameof(CreateChallengeAsync),
                    $"Failed to create because entities was null{challenger}{challengee}{exercise}");
                return -1;
            }

            //Create challenge
            var challenge = _mapper.Map<Challenge>(dto);
            challenge.ChallengerUser = challenger;
            challenge.ChallengeeUser = challengee;
            challenge.IsComplete = false;
            challenge.CreationDate = DateTime.Now;
            challenge.Exercise = exercise;
            challenge.ResultReps = 0;

            LogInformation(nameof(CreateChallengeAsync),
                $"Challenge was created between challengerId {challenger.Id} and challengeeId{challengee.Id}");
            return await _challengeRepository.CreateAsync(challenge);
        }

        private async Task SetExerciseNameAsync(ICollection<ChallengeDto> dtos)
        {
            foreach (var dto in dtos)
                await SetExerciseNameAsync(dto);
        }

        private async Task<bool> SetExerciseNameAsync(ChallengeDto dto)
        {
            var exercise = await _exerciseRepository.FindAsync(dto.ExerciseId);
            if (exercise == null)
            {
                LogError(nameof(SetExerciseNameAsync), $"No exercises found with id{dto.ExerciseId}. Check Database");
                return false;
            }
            dto.ExerciseName = exercise.Name;
            return true;
        }

        private async Task SetDisplayNameAsync(ICollection<ChallengeDto> dtos)
        {
            foreach (var dto in dtos)
                await SetDisplayNameAsync(dto);
        }

        private async Task<bool> SetDisplayNameAsync(ChallengeDto dto)
        {
            var challenger = await _userRepository.FindAsync(dto.ChallengerId);
            var challengee = await _userRepository.FindAsync(dto.ChallengeeId);

            if (challenger != null)
                dto.ChallengerDisplayName = challenger.DisplayName;
            if (challengee == null)
            {
                LogError(nameof(SetExerciseNameAsync),
                    $"No challengee user found with id{dto.ChallengeeId}. Check Database");
                return false;
            }
            dto.ChallengeeDisplayName = challengee.DisplayName;
            return true;
        }

        private async Task<bool> IsChallenableAsync(User user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in claims)
                if (claim.Type == ClaimType.ChallengeAccess.ToString() && claim.Value == "Enabled")
                    return true;
            return false;
        }

        private async Task<bool> IsShareAbleAsync(User user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in claims)
                if (claim.Type == ClaimType.ShareAccess.ToString() && claim.Value == "Enabled")
                    return true;
            return false;
        }

        private async Task SetChallengeAbleAsync(ICollection<ChallengeDto> dto)
        {
            foreach (var challengeDto in dto)
            {
                var challenger = await _userRepository.FindAsync(challengeDto.ChallengerId);
                var challengee = await _userRepository.FindAsync(challengeDto.ChallengeeId);

                if (challenger == null || challengee == null)
                {
                    challengeDto.IsShareAble = true;
                    continue;
                }

                var isChallengerConsent = await IsShareAbleAsync(challenger);
                var isChallengeeConsent = await IsShareAbleAsync(challengee);

                if (isChallengerConsent && isChallengeeConsent)
                    challengeDto.IsShareAble = true;
                else challengeDto.IsShareAble = false;
            }
        }
    }
}