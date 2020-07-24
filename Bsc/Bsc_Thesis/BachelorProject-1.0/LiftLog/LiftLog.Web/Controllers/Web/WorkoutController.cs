using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using LiftLog.Service.Interfaces;
using LiftLog.Web.ViewModels.WorkoutViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace LiftLog.Web.Controllers.Web
{
    /// <summary>
    ///     This controller is responsible for the workout page and is used to add, modify and delete workouts and their
    ///     entries.
    ///     The workout page is only accesible to privileged users (i.e. users with workout setting enabled).
    /// </summary>
    [Authorize(Policy = "WorkoutAccessPolicy")]
    public class WorkoutController : Controller
    {
        private static readonly Dictionary<int, NewChallengeViewModel> Tempdata =
            new Dictionary<int, NewChallengeViewModel>();

        private readonly IChallengeService _challengeService;
        private readonly IExerciseService _exerciseService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IWorkoutRepository _workoutRepository;
        private readonly IWorkoutService _workoutService;


        public WorkoutController(ILoggerFactory logger,
            IMapper mapper,
            IWorkoutService workoutService,
            IExerciseService exerciseService,
            UserManager<User> userManager,
            IWorkoutRepository workoutRepository,
            IChallengeService challengeService)
        {
            _logger = logger.CreateLogger<WorkoutController>();
            _mapper = mapper;
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _userManager = userManager;
            _workoutRepository = workoutRepository;
            _challengeService = challengeService;
        }

        /// <summary>
        ///     Directs to overview page of workouts for a specific user
        /// </summary>
        /// <returns>workout overview page</returns>
        public async Task<IActionResult> Workouts()
        {
            var user = await GetCurrentUserAsync();
            var workouts = await _workoutService.GetAllWorkoutsAsync(user.Id);

            if (workouts == null)
                return View("/Error");

            var model = _mapper.Map<ICollection<WorkoutDto>, IEnumerable<WorkoutViewModel>>(workouts);

            return View(model);
        }

        /// <summary>
        ///     Directs to a page to edit a specific workout.
        /// </summary>
        /// <param name="id"> Id of workout</param>
        /// <returns> Edit page of workout</returns>
        public async Task<IActionResult> EditWorkout(int id)
        {
            var workoutToUpdate = await _workoutService.GetWorkoutAsync(id);
            var exercises = await _exerciseService.GetAllExercisesAsync();

            if (workoutToUpdate == null || exercises == null)
                return View("/Error");

            var model = _mapper.Map<WorkoutViewModel>(workoutToUpdate);
            model.Exercises = exercises.ToList() ?? new List<ExerciseDto>(); // Add exercises or empty list if null
            model.WorkoutEntries =
                _mapper.Map<ICollection<WorkoutEntryDto>, IEnumerable<WorkoutEntryViewModel>>(workoutToUpdate
                    .WorkoutEntryDtos);

            return View(model);
        }

        /// <summary>
        ///     This action handles POST call requests used to save changed for a given workout.
        /// </summary>
        /// <param name="workoutViewModel"> Edited workout data, </param>
        /// <returns> Edit page of workout with updated data. </returns>
        [HttpPost]
        public async Task<IActionResult> EditWorkout(WorkoutViewModel workoutViewModel)
        {
            var user = await GetCurrentUserAsync();

            if (!ModelState.IsValid)
                return View(workoutViewModel);

            if (workoutViewModel == null)
                return View("/Error");

            //Creating new entry
            var newWorkoutEntry = new NewWorkoutEntryDto
            {
                ExerciseId = workoutViewModel.ExerciseId,
                Set = workoutViewModel.Set,
                Reps = workoutViewModel.Reps,
                Weight = workoutViewModel.Weight
            };

            //Adding to DTO
            var dto = new AddWorkoutEntryToWorkoutDto
            {
                WorkoutId = workoutViewModel.Id,
                UserId = user.Id,
                WorkoutEntryDtos = new List<NewWorkoutEntryDto>()
            };
            dto.WorkoutEntryDtos.Add(newWorkoutEntry);

            //Post to database
            var result = await _workoutService.AddWorkoutEntryAsync(dto);

            if (result < 1)
                return View("/Error");


            //Return to page with added entry
            var workoutToUpdate = await _workoutService.GetWorkoutAsync(workoutViewModel.Id);
            var exercises = await _exerciseService.GetAllExercisesAsync();

            var model = _mapper.Map<WorkoutViewModel>(workoutToUpdate);
            model.Exercises = exercises.ToList() ?? new List<ExerciseDto>();
            model.WorkoutEntries =
                _mapper.Map<ICollection<WorkoutEntryDto>, IEnumerable<WorkoutEntryViewModel>>(workoutToUpdate
                    .WorkoutEntryDtos);

            return View(model);
        }

        /// <summary>
        ///     Add a workout on workout page.
        /// </summary>
        /// <returns>
        ///     Edit page of new workout if success.
        /// </returns>
        public async Task<IActionResult> AddWorkout()
        {
            var user = await GetCurrentUserAsync();
            var exercises = await _exerciseService.GetAllExercisesAsync();
            var workouts = await _workoutService.GetAllWorkoutsAsync(user.Id);

            if (workouts == null || exercises == null)
                return View("/Error");


            var workoutSize = workouts.Count;
            var addWorkoutDto = new NewWorkoutDto {UserId = user.Id, Name = string.Format("Workout {0}", workoutSize)};
            var workoutId = await _workoutService.AddWorkoutAsync(addWorkoutDto);
            if (workoutId < 1)
                return View("/Error"); // Error 
            var workoutDto = await _workoutService.GetWorkoutAsync(workoutId);
            var model = _mapper.Map<WorkoutDto, WorkoutViewModel>(workoutDto);

            model.WorkoutEntries = new List<WorkoutEntryViewModel>();
            model.Exercises = exercises.ToList();

            return RedirectToAction(nameof(EditWorkout), model);
        }

        /// <summary>
        ///     Deletes a workout.
        /// </summary>
        /// <param name="id"> Id of workout. </param>
        /// <returns> Workout page with remaining workouts. </returns>
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var user = await GetCurrentUserAsync();
            var isSuccess = await _workoutService.DeleteWorkoutAsync(user.Id, id);
            if (isSuccess)
                return RedirectToAction(nameof(Workouts));
            return View("/Error"); // Error 
        }

        /// <summary>
        ///     Delete an entry part of a workout in the edit page.
        /// </summary>
        /// <param name="entryId"> Id of workout entry. </param>
        /// <param name="workoutId"> Id of workout. </param>
        /// <returns> Edit page of workout. </returns>
        public async Task<IActionResult> DeleteEntry(int entryId, int workoutId)
        {
            var user = await GetCurrentUserAsync();
            var isSuccess = await _workoutService.DeleteWorkoutEntryAsync(user.Id, entryId);

            if (!isSuccess)
                return View("/Error"); // Error 

            return RedirectToAction(nameof(EditWorkout), new RouteValueDictionary(
                new {controller = "Workout", action = nameof(EditWorkout), id = workoutId}));
            // new { controller = "Workout", action = nameof(EditWorkout), id = workoutId }));
        }


        [Authorize(Policy = "ChallengeAccessPolicy")]
        public async Task<IActionResult> Challenge(int entryId, int workoutId)
        {
            var user = await GetCurrentUserAsync();
            var dto = await _workoutService.GetWorkoutEntryAsync(entryId);

            if (dto == null || user == null)
                return View("/Error"); // Error 


            var model = _mapper.Map<NewChallengeViewModel>(dto);
            Tempdata[user.Id] = model;
            model.ChallengeAbleUserDtos = await _challengeService.GetChallengeAbleUsersAsync(user.Id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "ChallengeAccessPolicy")]
        public async Task<IActionResult> Challenge(NewChallengeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel); // Retry 

            var user = await GetCurrentUserAsync();

            NewChallengeViewModel tempData;
            var isSuccess = Tempdata.TryGetValue(user.Id, out tempData);

            if (!isSuccess || tempData == null)
                return View("/Error"); // Error 
            Tempdata.Remove(user.Id); //dispose data

            var dto = _mapper.Map<NewChallengeDto>(tempData);
            dto.ChallengerId = user.Id;
            dto.ChallengeeId = viewModel.ChallengeeId;

            var result = await _challengeService.CreateChallengeAsync(dto);
            if (result < 1)
                return View("/Error"); // Error 

            return RedirectToAction("Challenges", "Challenge");
        }


        [Authorize(Policy = "ShareAccessPolicy")]
        public async Task<IActionResult> ShareWorkoutEntry(int entryId)
        {
            var entry = await _workoutService.GetWorkoutEntryAsync(entryId);

            if (entry == null)
                return StatusCode(500);
            var model = _mapper.Map<ShareWorkoutEntryViewModel>(entry);
            model.Message = $"I just performed {entry.Reps} reps with {entry.Weight}kg of {entry.ExerciseName} ";
            return View(model);
        }

        #region Helpers

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(User);
        }

        #endregion
    }
}