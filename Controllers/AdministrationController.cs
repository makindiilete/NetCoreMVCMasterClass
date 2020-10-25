using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp_Net_Core_Masterclass.Models;
using Asp_Net_Core_Masterclass.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Asp_Net_Core_Masterclass.Controllers
{
    //A user must belong to both Admin and User role to work with the views of this controller.
    [Authorize(Roles = "Admin")]
     [Authorize(Roles = "User")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            //if the user fills the form with valid role name
            if (ModelState.IsValid)
            {
                //we create a new identityRole varaible
                IdentityRole identityRole = new IdentityRole
                {
                    //we set our role name to the name the user enters
                    Name = model.RoleName
                };
                //then we create the role
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                //if we succeed in creating the role
                if (result.Succeeded)
                {
                    //we redirect the user to the role list view
                    return RedirectToAction("ListRoles", "Administration");
                }

                //else we loop through all the errors we get
                foreach (IdentityError error in result.Errors)
                {
                    //and we add the errors to the model state
                    ModelState.AddModelError("", error.Description);
                }
            }

            //if the user fills the form with invalid data, we return back the form
            return View(model);
        }


        //ds action method retrieve the list of all roles we have in the database
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }


        //This action method is called to display the Edit Role View i.e. When the edit button is clicked on a role
        [HttpGet]
        //ds action method is protected from users not in the edit role policy...We are using custom authorization policy now its no longer needed
/*        [Authorize(Policy = "EditRolePolicy")]*/
        //ds action method takes an id params which is passed to it from the url by "ListRoles.cshtml" Edit button
        public async Task<IActionResult> EditRole(string id)
        {
            //we look for the role with the given id
            var role = await _roleManager.FindByIdAsync(id);
            //if no match found
            if (role == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"Role with the id : {id} cannot be found";
                return View("NotFound");
            }

            //else if a match is found we create an instance of the EditRoleViewModel and supply all the model properties
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,

            };

            //we loop through all the users of our app via the userManager
            foreach (var user in _userManager.Users)
            {
                //we check if any of the users is in the particular role
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //if yes we add the user to the "Users" list property
                    model.Users.Add(user.UserName);
                }
            }

            // we then return the view to be displayed and pass our model containing the data needed to be rendered
            return View(model);
        }

        //This action method is called to update edited role
        [HttpPost]
        //ds action method is protected from users not in the edit role policy
        //ds action method takes an id params which is passed to it from the url by "ListRoles.cshtml" Edit button
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            //we look for the role with the given id
            var role = await _roleManager.FindByIdAsync(model.Id);
            //if no match found
            if (role == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"Role with the id : {model.Id} cannot be found";
                return View("NotFound");
            }

            //else we find a match, we update the name of the found role with the new name in the form
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            //if we succeed in updating the role
            if (result.Succeeded)
            {
                //we redirect the user to the list of roles
                return RedirectToAction("ListRoles");
            }


            //else if we have errors, we add them to model state and return the view.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // we then return the view to be displayed and pass our model containing the data needed to be rendered
            return View(model);
        }


        //This is the httpGet part of adding and removing users from role
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            //if no match found
            if (role == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"Role with the id : {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        //This is the httpPost part of adding and removing users from role
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            //We search the database for the given role using the passed roleId from url query string
            var role = await _roleManager.FindByIdAsync(roleId);
            //if no match found
            if (role == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"Role with the id : {roleId} cannot be found";
                return View("NotFound");
            }

            //we loop through all the users in our app
            foreach (var user in model)
            {
                //we find each user by their id and store them in "userResult"
                var userResult = await _userManager.FindByIdAsync(user.UserId);
                //if we select a user by ticking the checkbox and that selected user is not already in the specified role (We want to add new user to the role)
                if (user.IsSelected && !(await _userManager.IsInRoleAsync(userResult, role.Name)))
                {

                    //we then go ahead and add them to the role
                    await _userManager.AddToRoleAsync(userResult, role.Name);
                }
                //if a user is not selected and the user is already in the specified role (We want to remove users dt were in a role)
                else if (!(user.IsSelected) && await _userManager.IsInRoleAsync(userResult, role.Name))
                {
                    //we remove the user from the role
                    await _userManager.RemoveFromRoleAsync(userResult, role.Name);
                }
            }

            return RedirectToAction("EditRole", new {Id = roleId});
        }

        //ds action method retrieve the list of all users we have in the database
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        //This action method is called to display the Edit User View i.e. When the edit button is clicked on a user
        [HttpGet]
        //ds action method takes an id params which is passed to it from the url by "ListRoles.cshtml" Edit button
        public async Task<IActionResult> EditUser(string id)
        {
            //we look for the role with the given id
            var findUser = await _userManager.FindByIdAsync(id);
            //if no match found
            if (findUser == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the id : {id} cannot be found";
                return View("NotFound");
            }

            //We retrieve all the claims & roles for this user
            var userClaims = await _userManager.GetClaimsAsync(findUser);
            var userRoles = await _userManager.GetRolesAsync(findUser);
            //else if a match is found we create an instance of the EditRoleViewModel and supply all the model properties
            var model = new EditUserViewModel
            {
                Id = findUser.Id,
                Email = findUser.Email,
                UserName = findUser.UserName,
                City = findUser.City,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList()//here we return the claim type & value
            };

            // we then return the view to be displayed and pass our model containing the data needed to be rendered
            return View(model);
        }

        //edit a user
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            //we look for the role with the given id
            var user = await _userManager.FindByIdAsync(model.Id);
            //if no match found
            if (user == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the Id : {model.Id} cannot be found";
                return View("NotFound");
            }

            user.Id = model.Id;
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.City = model.City;

            var result = await _userManager.UpdateAsync(user);

            //if we succeed in updating the role
            if (result.Succeeded)
            {
                //we redirect the user to the list of roles
                return RedirectToAction("ListUsers");
            }


            //else if we have errors, we add them to model state and return the view.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // we then return the view to be displayed and pass our model containing the data needed to be rendered
            return View(model);
        }

        //deletes a user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            //if no match found
            if (user == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the id : {id} cannot be found";
                return View("NotFound");
            }

            var result = await _userManager.DeleteAsync(user);
            //if we succeed in updating the role
            if (result.Succeeded)
            {
                //we redirect the user to the list of users
                return RedirectToAction("ListUsers");
            }


            //else if we have errors, we add them to model state and return the view.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // we then return the view to be displayed and pass our model containing the data needed to be rendered
            return View("ListUsers");
        }

        //deletes a role
        [HttpPost]
        //We implement our claim based authorization for delete using the "DeleteRolePolicy" we defined in the startUp.cs class
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            //if the id match no roll
            if (role == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"Role with the id : {id} cannot be found";
                return View("NotFound");
            }

            try
            {
                var result = await _roleManager.DeleteAsync(role);
                //if we succeed in updating the role
                if (result.Succeeded)
                {
                    //we redirect the user to the list of roles
                    return RedirectToAction("ListRoles");
                }


                //else if we have errors, we add them to model state and return the view.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // we then return the view to be displayed and pass our model containing the data needed to be rendered
                return View("ListRoles");
            }
            // catch (Exception e) //ds will catch all exception but we only want to catch errors specific to database update
            catch (DbUpdateException e
            ) //here we are using the "DbUpdateException as the type so it can only work for errors specific to database update
            {
                //we log the error
                _logger.LogError($"Error deleting role {e}");
                ViewBag.ErrorTitle = $"{role.Name} role is in use";
                ViewBag.ErrorMessage =
                    $"{role.Name} cannot be deleted as there are users in this role. If you want to delete this, role, please remove the users from the role and try again";

            }

            return View("Error");
        }

        //This action method manages user roles... it is called when "Manage Roles" is clicked on EditUser route
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            //if the id match no roll
            if (user == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the id : {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            //we loop through all the roles in our app
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        //This is the httpPost part of managing users roles
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            //We search the user by its id
            var user = await _userManager.FindByIdAsync(userId);
            //if no match found
            if (user == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the id : {userId} cannot be found";
                return View("NotFound");
            }

            //Get all roles for the specified user
            var roles = await _userManager.GetRolesAsync(user);
            //remove the user from all existing roles
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user from existing role");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add user to selected roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new {Id = userId});

        }


        //this method render the view to manage user claims
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            //We search the user by its id
            var user = await _userManager.FindByIdAsync(userId);
            //if no match found
            if (user == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the id : {userId} cannot be found";
                return View("NotFound");
            }

            //if we have found a user, we retrieve all his claims
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            //AllClaims is a list of claims inside ClaimStore class
            foreach (var claim in ClaimsStore.AllClaims)
            {
                //We create an instance of the UserClaim class and populating its properties
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                //We tick the checkbox for all the claims set to true in the "ClaimType" column in the database
                if (existingUserClaims.Any(c => c.Value == claim.Value))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        //this method render the view to manage user claims
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            //We search the user by its id
            var user = await _userManager.FindByIdAsync(model.UserId);
            //if no match found
            if (user == null)
            {
                //we render the NotFound view with this msg
                ViewBag.ErrorMessage = $"User with the id : {model.UserId} cannot be found";
                return View("NotFound");
            }

            //if we have found a user, we retrieve all his claims
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, existingUserClaims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI to the database (We have adjusted this... so now we are going to add all claims to the database whether they are selected or not)
            result = await _userManager.AddClaimsAsync(user,
               // model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));

               //we are going to add all claims to the database whether they are selected or not, the twist to this is that the claims that are selected on the UI will take a claimValue of "true" and the ones we deselected takes a claimValue of "false"
                model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }

        //Responds to unauthorized users
        [HttpGet]
        [AllowAnonymous]

        public IActionResult AccessDenied()
        {
            return View();
        }

        }
    }
