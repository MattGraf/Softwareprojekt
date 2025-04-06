using System.ComponentModel.DataAnnotations;
using Replay.Models;

namespace Replay.ViewModels.Account
{
    /// <summary>
    /// <c>ApplicationUserRoleViewModel</c> models the necessary data to display for the User Edit View.
    /// Contains all the necesary information for the User and its' Roles.
    /// </summary>
    public class UserRoleViewModel
    {
        // User data, can't directly reference the ApplicationUser here as it is a mapped EF-Entity.
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool Active { get; set; }
        // Roles displayed using the RoleViewModel as we need to display whether the user has a role or not
        // and don't want to save every Role-User pair.
        public List<RoleViewModel> Roles { get; set; }
    }
    /// <summary>
    /// View Model for the Role, so that we can display whether the Role is selected or not.
    /// </summary>
    public class RoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
