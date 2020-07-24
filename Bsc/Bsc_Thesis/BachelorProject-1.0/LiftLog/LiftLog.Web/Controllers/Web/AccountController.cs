using System;
using System.Collections.Generic;
using System.Linq;
using LiftLog.Core.Dto;
using LiftLog.Data.Entities;
using LiftLog.Service.EmailService;
using LiftLog.Service.Interfaces;
using LiftLog.Web.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using LiftLog.Core.Enums;

namespace LiftLog.Web.Controllers.Web
{

    /// <summary>
    /// The account controller performs registration, login and logout operations using the Identity framework
    /// for creating user accounts and signing the user in and out of the application. 
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager; 
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger; 
        private readonly IOptions<IdentityCookieOptions> _identityCookieOptions;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;

        // Inject SignInManager providing API for user log in by taking type of User that is does assigning for 
        public AccountController(SignInManager<User> signInManager, // Log users in and out 
            UserManager<User> userManager, // Create and manage users 
            IOptions<IdentityCookieOptions> identityCookieOptions, 
            ILoggerFactory loggerFactory,
            IEmailSender emailSender, 
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityCookieOptions = identityCookieOptions;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _emailSender = emailSender;
            _userService = userService;
        }

        /// <summary>
        /// Default action checking if user is authenticated.
        /// If so, should authorize access to user private pages(i.e.workouts and user profile).  
        /// GET: /Account/Login 
        /// </summary>
        /// <returns> Login page. </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null) 
        {
            ViewData["ReturnUrl"] = returnUrl; // Set redirect to page user originally wished for 
            return View(); 
        }

        /// <summary>
        /// This action handles login requests by validating the user credentials entered.
        /// If the login succeeds, the user is redirected to a user-specific page (e.g. workouts) that is private. 
        /// POST: /Account/Login 
        /// </summary>
        /// <param name="loginViewModel">
        /// View model exposing client data required to login on Login page. 
        /// </param>
        /// <returns>
        /// User-specific page after login succeed or redirects back to login page to try again. 
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                // Confirm user email before logging in 
                var user = await _userManager.FindByNameAsync(loginViewModel.Email);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty,
                                      "You must have a confirmed email to log in.");
                        return View(loginViewModel);
                    }
                }

                // Login user (authenticate) after confirming email 
                var signInResult = await _signInManager.PasswordSignInAsync(
                    loginViewModel.Email,
                    loginViewModel.Password,
                    loginViewModel.RememberMe, // Persist password after login
                    false); // Do not lock out user 

                if (signInResult.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");

                    if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                    else
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                }
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out");
                    return View("Lockout");
                }
                else
                {
                    // Shown in View upon redirect 
                    ModelState.AddModelError("", "Username or password incorrect.");
                }

            }
            return View(loginViewModel); // Go back to Login page to retry 
        }

        /// <summary>
        /// Register actions opens up view for creating user accounts. 
        /// GET: /Account/Register 
        /// </summary>
        /// <returns> Registration page. </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null, string message = null) 
        {
            ViewData["StatusMessage"] = message;
            ViewData["ReturnUrl"] = returnUrl;
            var model = new RegisterViewModel
            {
                ConsentMessage = "Consent information" +
                                 "\n\nMinimum required consent" +
                                 "\nBy signing up to LiftLog, you give consent that LiftLog may use the provided Email address and password to create an account for you." +
                                 "\n\nWhy Consent?" +
                                 "\nLiftLog provides many great features, but to access them you must give your consent for LiftLog to process your data and enable the features for you. " +
                                 "\nYou can give consent now or later on your account page.Your consent can be disabled at any times on your account page after you have signed up and LiftLog will " +
                                 "\nstop processing any of the data required for the feature." +
                                 "\n\nFeatures that requires your Consent" +
                                 "\nWorkout Feature " +
                                 "\nEnabling workouts allows LiftLog to process your workout data and use it in the workout page to give an overview of your workouts. " +
                                 "\nSpecifically, your workout data will be used to create, modify and delete workouts in the application. " +
                                 "\nLiftLog may use the workout data in an anonymized format for statistical purposes. " +
                                 "\n\nChallenge Feature Enabling challenges allows LiftLog to process your challenge data and use to show given and received challenges." +
                                 "\nSpecifically, your display name, and workout entries are used to give and receive challenges from other users in the application" +
                                 "\n\nShare Feature" +
                                 "\n Enabling this feature allows Liftlog to process your workout and challenge data to let you share them on the social."
            };
            return View(model); 
        }

        /// <summary>
        /// Register action creates a user using the data provided in the view model on the register page. 
        /// The user is stored using the UserManager. 
        /// </summary>
        /// <param name="model"> Registration data. </param>
        /// <pram name="returnUrl"> Page originally requested by user. </pram>
        /// <returns> Home page. </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DisplayName = model.DisplayName,
                    CreationDate = DateTime.Today,
                    ExportToken = Guid.NewGuid()
                };
                user.DisplayName = string.IsNullOrWhiteSpace(model.DisplayName) ? $"User#{user.Id + 101}" : model.DisplayName; //Default username if non provided
                var result = await _userManager.CreateAsync(user, model.Password); // Identity framework hashes password 
                

                if (result.Succeeded)
                {
                    var resultUserId = await _userService.SaveNewUserAsync(new NewUserDto { UserId = user.Id}); // Used for domain user 
                    var isConsent = await SetConsent(model, user.Id); //set consent
                    if (isConsent )
                    {
                        // Confirm email - Send an email with this link
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
                            new {userId = user.Id, code = code}, protocol: HttpContext.Request.Scheme);
                        await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                        _logger.LogInformation(3, "User created a new account with password.");
                        return RedirectToAction("Index", "Home",
                            new {message = "Please check your email to confirm your account."});
                    }
                    else
                    {
                        var toDelete = await _userManager.FindByIdAsync(resultUserId.ToString());
                        await _userManager.DeleteAsync(toDelete); //UNDO user creation
                    }
                }
           
                AddErrors(result);

            }
            // Error occurred so redisplay registration form 
            return View(model);
        }

        /// <summary>
        /// This action handles logout requests by signing in the user and redirecting back to the homepage. 
        /// POST: /Account/Logout 
        /// </summary>
        /// <returns> Homepage. </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Get rid of cookie collection and go back to home page
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home"); 
        }

        /// <summary>
        /// Action triggered when trying to login via external provider (i.e. Facebook). 
        /// POST: /Account/ExternalLogin
        /// </summary>
        /// <param name="provider"> Login provider (e.g. Facebook). </param>
        /// <param name="returnUrl"> URL to page originally requested by user. </param>
        /// <returns> Requested page or Login page to retry. </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to external login provider
            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {ReturnUrl = returnUrl});
            AuthenticationProperties properties =  _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Callback action used to sign in user via third party login provider (e.g. Facebook). 
        /// GET: /Account/ExternalLoginCallback
        /// </summary>
        /// <param name="returnUrl"> Requested page of user. </param>
        /// <param name="remoteError">Errors from external provider. </param>
        /// <returns> Requested page, Lockout page if locked out or Login page to retry. </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login)); // Redirect back to login page if error 
            }

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(); 
            if (info == null)
            {
                return RedirectToAction(nameof(Login)); // No external login provider setup 
            }

            // Sign in user with external login provider if user already has login 
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent: false);
            if (result.Succeeded) // If login success 
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl); // Redirect to originally requested page of user 
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If user does not have account, ask user to create an account
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View(nameof(ExternalLoginConfirmation), new ExternalLoginConfirmationViewModel {Email = email});
            }
        }

        /// <summary>
        /// Action triggered after having logged in via an external provider (e.g. Facebook).
        /// Registers user with external login information. 
        /// </summary>
        /// <param name="model"> Email to register with. </param>
        /// <param name="returnUrl"> Requested page of user. </param>
        /// <returns> Requested page or login failure page. </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get user information from external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure"); 
                }
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        /// <summary>
        /// This action is triggered when checking if the user has confirmed his details on email. 
        /// GET: /Account/ConfirmEmail 
        /// </summary>
        /// <param name="userId">
        /// Id of user to be checked for confirmation.
        /// </param>
        /// <param name="code">
        /// Confirmed email token. 
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            var isConfirmOrErrorPage = result.Succeeded ? "ConfirmEmail" : "Error";
            return View(isConfirmOrErrorPage);
        }

        /// <summary>
        /// Action triggered when user goes to page to reset password.
        /// GET: /Account/ForgotPassword 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Action triggered when filling in the forgot password form. 
        /// </summary>
        /// <param name="model"></param> // TODO add documentation 
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                // Account confirmation and password reset 
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Action triggered when user redirects to forgot password confirmation page. 
        /// GET: /Account/ForgotPasswordConfirmation
        /// </summary>
        /// <returns> Forgot password confirmation pager. </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// GET: /Account/ResetPassword
        /// </summary>
        /// <param name="code">Password reset token. </param>
        /// <returns> Page or error. </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            // Return error if password token does not exist 
            return code == null ? View("Error") : View();
        }

        /// <summary>
        ///  Action triggered when trying to reset user password. 
        ///  POST: /Account/ResetPassword
        /// </summary>
        /// <param name="model"> New password data. </param>
        /// <returns> Password confirmation page if succeeded. </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Action triggered when displaying reset password confirmation page. 
        /// GET: /Account/ResetPasswordConfirmation
        /// </summary>
        /// <returns> Page confirming reset password. </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Action triggered when user does not have access. 
        /// GET /Account/AccessDenied
        /// </summary>
        /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


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

        private async Task<bool> SetConsent(RegisterViewModel model, int userId)
        {
            var user = await GetUserAsync(userId);
            var claimsIdentity = GetCurrentClaimsIdentity();
            var claims = new List<Claim>();

            await SetClaim(claims, user, model.IsWorkoutConsent, ClaimType.WorkoutAccess);
            await SetClaim(claims, user, model.IsChallengeConsent, ClaimType.ChallengeAccess);
            await SetClaim(claims, user, model.IsShareConsent, ClaimType.ShareAccess);

            // Create checked claims 
            claimsIdentity.AddClaims(claims);
            var result = await _userManager.AddClaimsAsync(user, claims);

            return result.Succeeded;
        }

        private ClaimsIdentity GetCurrentClaimsIdentity()
        {
            var user = User as ClaimsPrincipal;
            var identity = user.Identity as ClaimsIdentity;
            return identity;
        }

        private async Task<User> GetUserAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }


        #endregion
    }
}







