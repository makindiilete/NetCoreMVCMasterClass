using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp_Net_Core_Masterclass.Models;
using Asp_Net_Core_Masterclass.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asp_Net_Core_Masterclass.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        // GET : - /acount/register
        [HttpGet]
        [AllowAnonymous] //we allow anonymous to be able to view d index page
        public IActionResult Register()
        {
            return View();
        }

        // We use this to verify if the email user is trying to register with has been taken
        //[HttpPost][HttpGet]
        [AcceptVerbs("Get", "Post")]//ds method respond to both Get and Post request
        [AllowAnonymous] //we allow anonymous to be able to view d index page
    public async Task<IActionResult> IsEmailInUse(string email)
    {
     var user = await _userManager.FindByEmailAsync(email);
     if (user == null)
     {
         return Json(true);
     }
     return Json($"Email {email} is already in use");
    }


    //This action method submits a post request to register a new user
        [HttpPost]
        [AllowAnonymous] //we allow anonymous to be able to view d index page
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //if the details the user fills in the form is not valid, we return the same view
            if (!ModelState.IsValid) return View();
            //else we declare a user variable dt takes an instance of our IdentityUser class (remember the identityUser class contains alot of properties common to a user) so here we give value to each of the properties we are using currently to register our user. So here we set the IdentityUser's Username & Email
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                City = model.City //from ApplicationUser.cs
            };
            //we create the user using the "user" variable and the user password
         var result =  await _userManager.CreateAsync(user, model.Password);
         //if we succeed in creating the user
         if (result.Succeeded)
         {
             //We generate confirmation token to confirm the user's email.
             var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

             //We generate the email confirmation link containing the userId, token and Request.Scheme for absolute link that will be clickable... Url.Action assigned this to the "ConfirmEmail" action of this "Account" controller.
             var confirmationLink = Url.Action("ConfirmEmail", "Account",
                 new { userId = user.Id, token = token }, Request.Scheme);

             //We log the confirmation link to our configured loggers....
             _logger.Log(LogLevel.Warning, confirmationLink);
                //we then check if we are already logged in as an admin before performing this registration
             if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
             {
                 //and in that case we redirect to the user list page bcos it is an admin dt is creating a user so we do not want to automatically log the admin out and login d created new user
                 return RedirectToAction("ListUsers", "Administration");
             }

             //Instead of using the Error view, you can create a separate view for this...
             ViewBag.ErrorTitle = "Registration successful";
             ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                                    "email, by clicking on the confirmation link we have emailed you";
             return View("Error");
         }
         //if we didnt succeed in creating the user, we loop through all the errors we have
         foreach (var error in result.Errors)
         {
             //for each error, we add it to our ModelState which will now be rendered in the view
             ModelState.AddModelError("", error.Description);
         }
            return View();
        }


        //This method is called to confirm user's email addresses. when user clicks on the email confirmation link in their email address
        [AllowAnonymous]
        //the userId and token comes from the query string params
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //if the userId and token is null, which means no id and token exist
            if (userId == null || token == null)
            {
                //we redirect to Home
                return RedirectToAction("index", "home");
            }
            //else we find the user using their id
            var user = await _userManager.FindByIdAsync(userId);
            //if we could not find a matching user
            if (user == null)
            {
                //we display this error on the NotFound page
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }
            //else we call the "ConfirmEmailAsync" method to confirm the mail
            var result = await _userManager.ConfirmEmailAsync(user, token);
            //if we succeed in confirming the mail
            if (result.Succeeded)
            {
                //we return the view
                return View();
            }
            //else we return an error that email cannot be confirmed
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }




        //This action method handles logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
         await _signInManager.SignOutAsync();
         return RedirectToAction("Index", "Home");
        }




        // GET : - /account/login : - This display the login form when user clicks on "login" button
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl) // d return url will be passed to this action method from the url query string.
        {
            //we create an instance of our LoginViewModel
            LoginViewModel model = new LoginViewModel
            {
                //we get the returnUrl from the url query string
                ReturnUrl = returnUrl,
                //to get the list of all available External logins configured for our app, we call the "GetExternalAuthenticationSchemesAsync()" and convert the result to a list
                ExternalLogins =
                    (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }



        // POST : - /account/login : - This handles the login process when use feels the form and submit....
        [HttpPost]
        [AllowAnonymous] //we allow anonymous to be able to view d index page
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //ds return the authentication for our external login providers.
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
/*            //if the details the user fills in the form to sign in is valid
            if (!ModelState.IsValid) return View();*/

            //if the email and password submitted is valid
            if (ModelState.IsValid)
            {
                //we use the email to find the user
                var user = await _userManager.FindByEmailAsync(model.Email);
                //if the user is not null but the user email is not confirmed
                if (user != null && !user.EmailConfirmed &&
                    //we also check if the provided email and password combination is correct
                    (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    //we add the custom error message "Email not confirmed yet" to the ModelState and display it
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }


                //we signIn the user using "PasswordSignInAsync()" and ds takes Email, Password,
                // The last boolean parameter lockoutOnFailure indicates if the account
                // should be locked on failed logon attempt. On every failed logon
                // attempt AccessFailedCount column value in AspNetUsers table is
                // incremented by 1. When the AccessFailedCount reaches the configured
                // MaxFailedAccessAttempts which in our case is 5, the account will be
                // locked and LockoutEnd column is populated. After the account is
                // lockedout, even if we provide the correct username and password,
                // PasswordSignInAsync() method returns Lockedout result and the login
                // will not be allowed for the duration the account is locked.
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                //if we succeed in signing in the user
                if (result.Succeeded)
                {
                    //we check if we have a returnUrl in the url (i.e. to check if the user tried to access a protected route) and also confirm if it is a local url (to prevent attack)
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        //we redirect them back to the page they tried to access previously
                        return Redirect(returnUrl);
                    }

                    //else we then redirect them to homepage
                    return RedirectToAction("Index", "Home");
                }

                // If account is locked out send the use to AccountLocked view
                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
            }

            //if we cud not sign in the user, we add this error message to the model and display it in our view.
                ModelState.AddModelError("", "Invalid Login Attempt");
                return View(model);
        }




        //This handles redirection to the external login provider on clicking on the External login button
        [AllowAnonymous]
        [HttpPost]
        //this takes the name of the provider and the returnUrl
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
        //Open loggingin with their google credentials, we redirect them back to "ExternalLoginCallback()" action method
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new { ReturnUrl = returnUrl });
            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        //This method is called to process the login request when we fill our info in the external provider login form
 [AllowAnonymous]
public async Task<IActionResult>
    ExternalLoginCallback(string returnUrl = null, string remoteError = null)
{
    //if the returnUrl is null then we use our app root url
    returnUrl = returnUrl ?? Url.Content("~/");

    LoginViewModel loginViewModel = new LoginViewModel
    {
        ReturnUrl = returnUrl,
        //we retrieve the list of all configured external login providers
        ExternalLogins =
        (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
    };

    //if we av remote error, we add the error to model state and return the login with the errors for correction
    if (remoteError != null)
    {
        ModelState.AddModelError(string.Empty,
            $"Error from external provider: {remoteError}");

        return View("Login", loginViewModel);
    }

    //we retrieve the client's info from the external login provider
    var info = await _signInManager.GetExternalLoginInfoAsync();
    //if info is null, we add the error message "Error loading external login information." to the ModelState and we return the login view with the errors for correction.
    if (info == null)
    {
        ModelState.AddModelError(string.Empty,
            "Error loading external login information.");

        return View("Login", loginViewModel);
    }

    // Get the email claim from external login provider (Google, Facebook etc)
    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    ApplicationUser user = null;

    //if we ge the email
    if (email != null)
    {
        //we use the email to Find the user
        user = await _userManager.FindByEmailAsync(email);

        // If email is not confirmed, display login view with validation error
        if (user != null && !user.EmailConfirmed)
        {
            ModelState.AddModelError(string.Empty, "Email not confirmed yet");
            return View("Login", loginViewModel);
        }
    }

    //else if email has been confirmed, we try to sign in the user
    var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
        info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

    //if signin succeeds
    if (signInResult.Succeeded)
    {
        //we redirect them to returnurl
        return LocalRedirect(returnUrl);
    }
    //else if email is not null but user does not exist yet in the database
    else
    {
        if (email != null)
        {
            if (user == null)
            {
                //we create the user and store it in the database
                user = new ApplicationUser
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };

                await _userManager.CreateAsync(user);

                // After a local user account is created, we generate and log the email confirmation link
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                _logger.Log(LogLevel.Warning, confirmationLink);

                //We display this msgs for user to confirm their email after registration
                ViewBag.ErrorTitle = "Registration successful";
                ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                                       "email, by clicking on the confirmation link we have emailed you";
                return View("Error");
            }

            //we add the userlogin info to the database and sign-in the user then redirect to returnUrl
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(returnUrl);
        }

        //else if email is null, we display this errors and return the view with the errors received
        ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
        ViewBag.ErrorMessage = "Please contact support on akindiileteforex@gmail.com";

        return View("Error");
    }
}



    //This returns/display the forgot password form
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }


    //This handle the post request after the forgot password is submitted
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            // If the user is found AND Email is confirmed
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                // Generate the reset password token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Generate a url for the password reset
                var passwordResetLink = Url.Action("ResetPassword", "Account",
                    new { email = model.Email, token = token }, Request.Scheme);

                // Log the password reset link
                _logger.Log(LogLevel.Warning, passwordResetLink);

                // Send the user to Forgot Password Confirmation view
                return View("ForgotPasswordConfirmation");
            }

            // To avoid account enumeration and brute force attacks, don't
            // reveal that the user does not exist or is not confirmed
            //so we still render the same view we rendered
            return View("ForgotPasswordConfirmation");
        }

        return View(model);
    }

    //This return the reset password UI
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string email, string token)
    {

            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
    }
        //This handle the post request to reset the password
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // reset the user password
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    // Upon successful password reset and if the account was lockedout before requesting for the new password, set
                    // the account lockout end date to current UTC date time, so the user
                    // can login with the new password
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                    }
                    return View("ResetPasswordConfirmation");
                }
                // Display validation errors. For example, password reset token already
                // used to change the password or password complexity rules not met
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // To avoid account enumeration and brute force attacks, don't
            // reveal that the user does not exist
            return View("ResetPasswordConfirmation");
        }
        // Display validation errors if model state is not valid
        return View(model);
    }

    //This method renders the view to changePassword
    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        //We try to locate the user
        var user = await _userManager.GetUserAsync(User);
        //we check maybe the user has password
        var userHasPassword = await _userManager.HasPasswordAsync(user);
        //If the user does not have a password
        if (!userHasPassword)
        {
            //we redirect him/her to the "AddPassword" view.
            return RedirectToAction("AddPassword");
        }

        return View();
    }


    //This method submit the form to the backend with httpPost request...
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // ChangePasswordAsync changes the user password
            var result = await _userManager.ChangePasswordAsync(user,
                model.CurrentPassword, model.NewPassword);

            // The new password did not meet the complexity rules or
            // the current password is incorrect. Add these errors to
            // the ModelState and rerender ChangePassword view
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            // Upon successfully changing the password refresh sign-in cookie
            await _signInManager.RefreshSignInAsync(user);
            return View("ChangePasswordConfirmation");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddPassword()
    {
        var user = await _userManager.GetUserAsync(User);

        var userHasPassword = await _userManager.HasPasswordAsync(user);

        if (userHasPassword)
        {
            return RedirectToAction("ChangePassword");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);

            return View("AddPasswordConfirmation");
        }

        return View(model);
    }


    }
}
