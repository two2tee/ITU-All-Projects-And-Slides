using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using LiftLog.Core.Dto;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Core.Enums;
using LiftLog.Data.Entities;
using LiftLog.Service.EmailService;
using LiftLog.Service.Interfaces;
using LiftLog.Web.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiftLog.Web.Controllers.Web
{
    /// <summary>
    ///     Controller handles requests for managing users as an administrator.
    ///     The controller is used to handle all tasks related to user management.
    ///     Authorization is done in the AccountController.
    /// </summary>
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IChallengeService _challengeService;
        private IEmailSender _emailSender;
        private readonly IExportService _exportService;
        private readonly string _externalCookieScheme;
        private readonly ILogger<ManageController> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IWorkoutService _workoutService;

        public ManageController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            IMapper mapper,
            IUserService userService,
            IExportService exportService,
            IWorkoutService workoutService,
            IChallengeService challengeService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _mapper = mapper;
            _userService = userService;
            _exportService = exportService;
            _workoutService = workoutService;
            _challengeService = challengeService;
        }

        /// <summary>
        ///     Handles requests when managing users.
        ///     GET: Manage/Index
        /// </summary>
        /// <param name="message"> Message denoting change in user profile. </param>
        /// <param name="paramsDto"> Dto containing external provider link. </param>
        /// <returns> Index page showing logins. </returns>
        [HttpGet]
        public async Task<IActionResult> Index(ParamsDto paramsDto, ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess
                    ? "Your password has been changed."
                    : message == ManageMessageId.SetPasswordSuccess
                        ? "Your password has been set."
                        : message == ManageMessageId.Error
                            ? "An error has occurred."
                            : message == ManageMessageId.ChangeAccountSuccess
                                ? "Your account details have been updated."
                                : message == ManageMessageId.DeleteAccountSuccess
                                    ? "Your account was successfully deleted."
                                    : message == ManageMessageId.DeleteAccountError
                                        ? "Your account was not deleted. Please try again."
                                        : message == ManageMessageId.ConsentUpdateSuccess
                                            ? "Your consent settings were successfully updated."
                                            : message == ManageMessageId.ConsentUpdateError
                                                ? "Your consent settings were not updated. Please try again."
                                                : message == ManageMessageId.DeleteAllWorkoutDataSuccess
                                                    ? "All your workout data has been successfully deleted. Now please disable the workout setting if you do not want to use it anymore."
                                                    : message == ManageMessageId.DeleteAllWorkoutDataError
                                                        ? "Your workout data was not deleted. Please try again."
                                                        : message == ManageMessageId.DeleteAllChallengeDataSuccess
                                                            ? "All your challenge data has been successfully deleted. Now please disable the challenge setting if you do not want to use it anymore."
                                                            : message == ManageMessageId.DeleteAllChallengeDataError
                                                                ? "Your challenge data was not deleted. Please try again."
                                                                : "";

            var user = await GetCurrentUserAsync();

            if (user == null)
                return View("Error");

            var domainUser = await _userService.GetUserAsync(user.Id);

            var model = _mapper.Map<UserDto, IndexViewModel>(domainUser);
            model.HasPassword = await _userManager.HasPasswordAsync(user);
            model.Logins = await _userManager.GetLoginsAsync(user);
            model.ExportLink = paramsDto.ApiPath;

            return View(model);
        }


        /// <summary>
        ///     Handles POST requests when user tries to update his personal account information.
        /// </summary>
        /// <param name="model">Personal details</param>
        /// <returns>Account page with updated details or error </returns>
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return View("Error");

            model.HasPassword = await _userManager.HasPasswordAsync(user);
            model.Logins = await _userManager.GetLoginsAsync(user);

            if (!ModelState.IsValid)
                return View(model);

            var updatedUser = _mapper.Map<IndexViewModel, ModifiedUserDto>(model);
            updatedUser.Id = user.Id;

            var isSuccess = await _userService.ModifyUserAsync(updatedUser);

            if (isSuccess)
            {
                _logger.LogInformation(2, "Account details were successfuly updated.");
                return RedirectToAction(nameof(Index), new {Message = ManageMessageId.ChangeAccountSuccess});
            }
            _logger.LogError(3, "Account details were not updated");
            return View(model); // Try again 
        }

        /// <summary>
        ///     POST request used to remove a login for a given user when managing users.
        /// </summary>
        /// <param name="account">The account to be removed. </param>
        /// <returns> Page with logins and message on result of removal. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            if (!ModelState.IsValid)
            {
                return View("/Error");
            }

            ManageMessageId ? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }
            return RedirectToAction(nameof(ManageLogins), new {Message = message});
        }

        /// <summary>
        ///     GET request handles requests to page where password can be changed.
        ///     GET: /Manage/ChangePassword
        /// </summary>
        /// <returns>Page showing optiosn for changing password. </returns>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        ///     POST request used to change password of a given user.
        ///     POST: /Manage/ChangePassword
        /// </summary>
        /// <param name="model"> User information. </param>
        /// <returns> Default user manage page with message on result of change. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation(3, "User changed their password successfully");
                    return RedirectToAction(nameof(Index), new {Message = ManageMessageId.ChangePasswordSuccess});
                }
                AddErrors(result);

                return View(model);
            }

            return RedirectToAction(nameof(Index), new {Message = ManageMessageId.Error});
        }

        /// <summary>
        ///     Redirect requests for the page used to set the user password.
        ///     GET: /Manage/SetPassword
        /// </summary>
        /// <returns>Set Password page. </returns>
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        /// <summary>
        ///     Post request used to change a user password.
        ///     POST: /Manage/SetPassword
        /// </summary>
        /// <param name="model"> New password details. </param>
        /// <returns> User Management home page. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction(nameof(Index), new {Message = ManageMessageId.SetPasswordSuccess});
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new {Message = ManageMessageId.Error});
        }

        /// <summary>
        ///     Get request handles requests when trying to add or remove user logins.
        /// </summary>
        /// <param name="message">Message denoting if login was added or removed or error happened. </param>
        /// <returns> Page showing logins of user (e.g. local and FB). </returns>
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess
                    ? "The external login was removed."
                    : message == ManageMessageId.AddLoginSuccess
                        ? "The external login was added."
                        : message == ManageMessageId.Error
                            ? "An error has occurred."
                            : "";
            var user = await GetCurrentUserAsync();
            if (user == null)
                return View("Error");
            var userLogins = await _userManager.GetLoginsAsync(user); // Get login of specified user 
            // Check if user has other external logins 
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes()
                .Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider))
                .ToList();
            ViewData["ShowRemoveButton"] =
                user.PasswordHash != null ||
                userLogins.Count > 1; // Only show remove button if user login exists with password 
            return View(new ManageLoginsViewModel // Retrieves logins of user 
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        /// <summary>
        ///     Post request used to fetch external logins associated with a given user.
        ///     POST: /Manage/LinkLogin
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback), "Manage");
            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl,
                    _userManager.GetUserId(User));
            return
                Challenge(properties,
                    provider); // Get authentication from external provider using special "Challenge" action 
        }

        /// <summary>
        ///     Callback received from external login provider when trying to associate logins with user.
        ///     GET: /Manage/LinkLoginCallback
        /// </summary>
        /// <returns> User management page with message on result of action. </returns>
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return View("Error");
            var info =
                await _signInManager.GetExternalLoginInfoAsync(await _userManager
                    .GetUserIdAsync(user)); // Get login information 
            if (info == null)
                return RedirectToAction(nameof(ManageLogins), new {Message = ManageMessageId.Error});
            var result =
                await _userManager.AddLoginAsync(user,
                    info); // Add login for user and information from external provider 
            var message = ManageMessageId.Error;
            if (result.Succeeded)
            {
                message = ManageMessageId.AddLoginSuccess;
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);
            }
            return RedirectToAction(nameof(ManageLogins), new {Message = message});
        }

        /// <summary>
        ///     Handles Post request used to delete account on the user management page.
        /// </summary>
        /// <returns> Home page if success or management page to retry again if error. </returns>
        //[HttpPost]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await GetCurrentUserAsync();
            await DeleteAllChallengeData();
            await DeleteAllWorkoutData();
            await _signInManager.SignOutAsync(); // Log out user before deleting account 
            var result = await _userManager.DeleteAsync(user);
            var message = ManageMessageId.DeleteAccountError;

            if (result.Succeeded)
            {
                // Redirect back to home page and display status message 
                message = ManageMessageId.DeleteAccountSuccess;
                return RedirectToAction("Index", "Home", new {message = "Account was successfully deleted."});
            }
            return RedirectToAction(nameof(Index), new {Message = message}); // Try again 
        }

        /// <summary>
        ///     Exports user data to a pdf
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ExportPdf()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                //Use export service to retrieve a fileDto
                var fileDto = await _exportService.ExportToFileAsync(user.Id);

                return File(fileDto.FileContent, $"application/{fileDto.FileExtention}", fileDto.FileName);
            }
            catch (Exception e)
            {
                return View("/Error"); // Error 
            }
        }

        /// <summary>
        ///     Retrieve link to export personal data of user to another provider using an API controller and a unique token.
        /// </summary>
        /// <returns> Link to personal data in JSON format. </returns>
        public async Task<IActionResult> GetExportLink()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var token = await _exportService.GetExportTokenAsync(user.Id);

                var pageAddress = HttpContext.Request.Host;
                var apiPath = $"https://{pageAddress}/api/export?token={token}";

                return RedirectToAction(nameof(Index), new ParamsDto {ApiPath = apiPath});
            }
            catch (Exception e)
            {
                return View("/Error"); // Error 
            }
        }

        /// <summary>
        ///     Deletes all workout data associated with a given user in the workout setting.
        /// </summary>
        /// <returns> View with message describing if workouts were removed successfully or not. </returns>
        public async Task<IActionResult> DeleteAllWorkoutData()
        {
            var user = await GetCurrentUserAsync();
            var isDeleted = await _workoutService.DeleteAllWorkoutAsync(user.Id);
            var message = ManageMessageId.DeleteAllWorkoutDataError;

            if (isDeleted)
                message = ManageMessageId.DeleteAllWorkoutDataSuccess;

            return RedirectToAction(nameof(Index), new {Message = message}); // Try again 
        }

        /// <summary>
        ///     Deletes all challenge data associated with a given user in the challenge setting.
        /// </summary>
        /// <returns> View with message describing if challenges were removed successfully or not. </returns>
        public async Task<IActionResult> DeleteAllChallengeData()
        {
            var user = await GetCurrentUserAsync();
            var isDeleted = await _challengeService.DeleteAllChallengesAsync(user.Id);
            var message = ManageMessageId.DeleteAllChallengeDataError;

            if (isDeleted)
                message = ManageMessageId.DeleteAllChallengeDataSuccess;

            return RedirectToAction(nameof(Index), new {Message = message}); // Try again 
        }

        /// <summary>
        ///     Updates the consent settings in the user management page.
        /// </summary>
        /// <param name="accountConsent"> If checked, gives access to Account. </param>
        /// <param name="workoutConsent"> If checked, gives access to workout page. </param>
        /// <param name="challengeConsent"> If checked, gives access to challenge page. </param>
        /// <param name="shareConsent">If checked, gives access to share functionality</param>
        /// <returns> Updated settings page. </returns>
        public async Task<IActionResult> UpdateConsentSettings(string accountConsent, string workoutConsent,
            string challengeConsent, string shareConsent)
        {
            var user = await GetCurrentUserAsync();
            var claimsIdentity = GetCurrentClaimsIdentity();
            var claims = new List<Claim>();
            var message = ManageMessageId.ConsentUpdateError;
            const string isChecked = "check";

            //Set claim
            await SetClaim(claims, user, accountConsent == isChecked, ClaimType.AccountAccess);
            await SetClaim(claims, user, workoutConsent == isChecked, ClaimType.WorkoutAccess);
            await SetClaim(claims, user, challengeConsent == isChecked, ClaimType.ChallengeAccess);
            await SetClaim(claims, user, shareConsent == isChecked, ClaimType.ShareAccess);


            // Create checked claims 
            claimsIdentity.AddClaims(claims);
            var result = await _userManager.AddClaimsAsync(user, claims);

            // Reset user cookie 

            await _signInManager.RefreshSignInAsync(user);

            if (result.Succeeded)
                message = ManageMessageId.ConsentUpdateSuccess;

            return RedirectToAction(nameof(Index), new {Message = message}); // Try again 
        }


        #region Helpers

        private async Task SetClaim(IList<Claim> claims, User user, bool isEnabled, ClaimType claimtype)
        {
            var claimsIdentity = GetCurrentClaimsIdentity();
            var claimType = claimtype.ToString();
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            
            if (isEnabled && claim == null)
            {
                claims.Add(new Claim(claimType, "Enabled"));
            }
            else
            {
                if (claim != null && !isEnabled)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                    claimsIdentity.RemoveClaim(claim);
                }
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Description != null)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        public enum ManageMessageId
        {
            AddLoginSuccess,
            ChangePasswordSuccess,
            ChangeAccountSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error,
            DeleteAccountSuccess,
            DeleteAccountError,
            ConsentUpdateSuccess,
            ConsentUpdateError,
            DeleteAllWorkoutDataSuccess,
            DeleteAllWorkoutDataError,
            DeleteAllChallengeDataSuccess,
            DeleteAllChallengeDataError
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        private ClaimsIdentity GetCurrentClaimsIdentity()
        {
            var user = User;
            var identity = user.Identity as ClaimsIdentity;
            return identity;
        }

        #endregion
    }
}