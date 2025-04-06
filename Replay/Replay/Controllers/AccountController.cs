using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Replay.Models;
using Replay.Data;
using Microsoft.EntityFrameworkCore;
using Replay.ViewModels;
using System.ComponentModel;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage;
using OpenQA.Selenium.DevTools.V123.Browser;
using Replay.Models.Account;
using Replay.Models.Account.MTM;
using Replay.ViewModels.Account;
using Replay.Container;
using Replay.Container.Account.MTM;
using Replay.Container.Account;
using Replay.Authorization;
using System;
using System.Text.RegularExpressions;

namespace Replay.Controllers
{

    /// <summary>
    /// Controller to change the views for the <see cref="User"/>s
    /// </summary>
    /// <author>Felix Nebel</author>
    public class AccountController : Controller
    {

        private RoleContainer _roleContainer;
        private UserContainer _userContainer;
        private UserRolesContainer _userRoleContainer;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public string? FullName { get; set; }
        }

        /// <summary>
        /// Creates an AccountController with the needed Containers
        /// </summary>
        /// <param name="userContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="userRolesContainer">Container for the connection to the database</param>
        /// <author>Felix Nebel</author>
        public AccountController(
            UserContainer userContainer, RoleContainer roleContainer, UserRolesContainer userRolesContainer)
        {

            _userContainer = userContainer;
            _roleContainer = roleContainer;
            _userRoleContainer = userRolesContainer;
        }


        /// <summary>
        /// Change the View to overview of all the <see cref="User"/>s
        /// </summary>
        /// <returns>View of all the <see cref="User"/>s</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Index()
        {
            List<User> users = _userContainer.GetUsers().ToList();

            List<Role> roles = _roleContainer.GetRoles().ToList();

            List<UserRoleViewModel> userViewModel = new List<UserRoleViewModel>();
            foreach (var item in users)
            {
                List<Role> userRoles = _userRoleContainer.GetRolesFromUser(item).Result.ToList();
                List<RoleViewModel> roleViewModels = new List<RoleViewModel>();
                foreach (var role in userRoles)
                {
                    roleViewModels.Add(new RoleViewModel
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Selected = roles.Contains(role)
                    });
                }

                userViewModel.Add(item.CreateUserViewModel(roleViewModels));
            }
            return View(userViewModel);

        }
        /// <summary>
        /// Change the View to the Login Page for the User
        /// </summary>
        /// <returns>View of the Login Page</returns>
        /// <author>Felix Nebel</author>
        public IActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// Logs the User out
        /// </summary>
        /// <returns>Redirect to the Home Page</returns>
        /// <author>Felix Nebel</author>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Logs the User in and automatically returns them to the in the URL specified page
        /// </summary>
        /// <returns>The Home Page or specified Page</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            //ReturnUrl = HttpContext.Request.Query["ReturnUrl"];
            if (!ModelState.IsValid)
            {
                return View();
            }
            string pattern = @"\..*";
            string result = Regex.Replace(Input.Email, pattern, "");
            User dummyUser = new User
            {
                Email = result
            };

            User? appUser = _userContainer.GetUserFromEmail(Input.Email).Result;

            if (appUser == null)
                return View();
                
            PasswordVerificationResult verify = _passwordHasher.VerifyHashedPassword(dummyUser, appUser.Password, Input.Password);

            if (verify != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Input.Email),
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            foreach (var claim in User.Claims)
            {
                Debug.WriteLine("Claim: " + $"{claim.Value}");
            }
            if (returnUrl != null)
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");

        }
        /// <summary>
        /// Change the View to the Register Page for the User
        /// </summary>
        /// <returns>View of the Register Page</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// Registers the passed user in the application
        /// </summary>
        /// <returns>The Register page if unsuccesful, the Index if succesful</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Register([Bind("FullName,Email,Password")] User applicationUser)
        {
            if (_userContainer.GetUserFromEmail(applicationUser.Email).Result != null)
                ModelState.AddModelError(nameof(applicationUser.Email), "Ein Nutzer mit dieser E-Mail existiert bereits.");
            if (ModelState.IsValid)
            {
                string pattern = @"\..*";
                string result = Regex.Replace(applicationUser.Email, pattern, "");
                User dummyUser = new User
                {
                    Email = result
                };
                applicationUser.Password = _passwordHasher.HashPassword(dummyUser, applicationUser.Password);

                _userContainer.AddUser(applicationUser);

                return RedirectToAction("Index", "Account");
            }

            return View(applicationUser);
        }

        /// <summary>
        /// Change the View to the Edit Page for the selected User with the passed <paramref name="id"/>
        /// </summary>
        /// <returns>View of the Edit Page for the User with <paramref name="id"/></returns>
        /// <author>Felix Nebel</author>
        public IActionResult Edit(int id)
        {
            var user = _userContainer.GetUserFromId(id).Result;
            return View(user.CreateUserRoleViewModel(_roleContainer.GetRoles(), _userRoleContainer.GetRolesFromUser(user).Result));
        }

        /// <summary>
        /// Edits the passed user based on the users id
        /// </summary>
        /// <returns>Returns the Index View of users</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserRoleViewModel user)
        {
            User? previousUser = _userContainer.GetUserFromId(user.Id).Result;
            _userRoleContainer.GetUserRolesFromUser(previousUser).Result.ToList().ForEach(pRole => _userRoleContainer.DeleteUserRole(pRole));
            User newUser = new User()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Password = user.Password,
                Active = user.Active,
            };
            _userContainer.UpdateUser(newUser);
            if (user.Roles == null)
                return RedirectToAction("Index");
            var viewList = user.Roles.Where(rolepermission => rolepermission.Selected == true).ToList();
            viewList.ForEach(async userRole => _userRoleContainer.AddUserRole(new Models.Account.MTM.UserRole()
            {
                UserId = newUser.Id,
                User = await _userContainer.GetUserFromId(user.Id),
                RoleId = userRole.Id,
                Role = await _roleContainer.GetRoleFromId(userRole.Id)
            }));
            return RedirectToAction("Index");
        }

        


    }
}
