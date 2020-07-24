using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;
using LiftLog.Service.Interfaces;
using LiftLog.Web.ViewModels.ChallengeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiftLog.Web.Controllers.Web
{
    /// <summary>
    ///     This controller is responsible for the challenge page and is used to create, modify and delete challenges.
    ///     The challenge page is only accesible to privileged users (i.e. users with workout setting enabled).
    /// </summary>
    [Authorize(Policy = "ChallengeAccessPolicy")]
    public class ChallengeController : Controller
    {
        private static readonly Dictionary<int, ChallengeResultViewModel> Tempdata =
            new Dictionary<int, ChallengeResultViewModel>();

        private readonly IChallengeService _challengeService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public ChallengeController(UserManager<User> userManager, ILoggerFactory logger, IMapper mapper,
            IChallengeService challengeService)
        {
            _challengeService = challengeService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger.CreateLogger<WorkoutController>();
        }

        /// <summary>
        ///     Directs to overview page of challenges for a specific user
        /// </summary>
        /// <returns>workout overview page</returns>
        public async Task<IActionResult> Challenges()
        {
            var user = await GetCurrentUserAsync();
            var receivedChallenges = await _challengeService.GetReceivedChallengesAsync(user.Id);
            var givenChallenges = await _challengeService.GetGivenChallengesAsync(user.Id);

            if (receivedChallenges == null || givenChallenges == null)
                return StatusCode(500);

            var model = new ChallengeViewModel
            {
                ReceivedChallenges = receivedChallenges.AsEnumerable(),
                GivenChallenges = givenChallenges.AsEnumerable()
            };


            return View(model);
        }

        /// <summary>
        ///     Post the result of a challenge to the challenge board.
        /// </summary>
        /// <param name="challengeResultViewModel"> Challenge data. </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostResults(ChallengeResultViewModel challengeResultViewModel)
        {
            if (!ModelState.IsValid)
                return View(challengeResultViewModel); // Retry 

            var user = await GetCurrentUserAsync();

            var dto = _mapper.Map<ChallengeResultDto>(challengeResultViewModel);

            if (Tempdata != null && Tempdata.ContainsKey(user.Id))
            {
                var tempData = Tempdata[user.Id];
                Tempdata.Remove(user.Id); //Dispose data
                dto.ChallengeId = tempData.Id;
                dto.UserId = user.Id;
                dto.IsComplete = true;
            }
            // else return View("/Error"); // TODO unit tests rely on Tempdata 

            var isSuccess = await _challengeService.SetResultAsync(dto);
            if (!isSuccess)
                return StatusCode(500);

            return RedirectToAction(nameof(Challenges));
        }

        /// <summary>
        ///     Show page used to post result of a challenge.
        /// </summary>
        /// <param name="challengeId"> Challenge to post result on. </param>
        /// <returns></returns>
        public async Task<IActionResult> PostResults(int challengeId)
        {
            var user = await GetCurrentUserAsync();
            var challengeDto = await _challengeService.GetChallengeAsync(challengeId);

            if (challengeDto == null)
                return StatusCode(500);

            var model = _mapper.Map<ChallengeResultViewModel>(challengeDto);
            Tempdata[user.Id] = model;
            return View(model);
        }

        /// <summary>
        ///     Delete challenge picked on challenge page.
        /// </summary>
        /// <param name="challengeId"> Challenge to delete. </param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteChallenge(int challengeId)
        {
            var user = await GetCurrentUserAsync();
            var isSuccess = await _challengeService.DeleteChallengeAsync(user.Id, challengeId);
            if (!isSuccess)
                return StatusCode(500);
            return RedirectToAction(nameof(Challenges));
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(User);
        }

        /// <summary>
        ///     Share a challenge on social media. //todo check consent
        /// </summary>
        /// <param name="challengeId"></param>
        /// <returns></returns>
        [Authorize(Policy = "ShareAccessPolicy")]
        public async Task<IActionResult> ShareResult(int challengeId)
        {
            var challengeDto = await _challengeService.GetChallengeAsync(challengeId);

            if (challengeDto == null)
                return StatusCode(500);

            var model = _mapper.Map<ShareChallengeViewModel>(challengeDto);
            model.Message =
                $"A user on LiftLog.dk has challenged me to perform {model.Reps} reps with {model.Weight}kg of {model.ExerciseName}!\nI managed to perform {model.ResultReps} reps! ";
            return View(model);
        }
    }
}