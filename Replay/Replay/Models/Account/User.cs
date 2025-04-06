using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Replay.Models.Account.MTM;
using Replay.ViewModels.Account;

namespace Replay.Models.Account
{

    /// <summary>
    /// Class <c>User</c> models the defining features of the <c>User</c>
    /// along with the corresponding roles this user has, mapped through a Many-to-Many relationship with <see cref="Role"/> using
    /// <see cref="UserRole"/>.
    /// </summary>
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Es muss eine korrekte E-Mail angegeben werden.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Der Anzeigename darf nicht leer sein.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Das Passwort darf nicht leer sein.")]
        public string Password { get; set; }
        public bool Active { get; set; }                     // Determines whether the user is disabled or not.
        public virtual List<UserRole> UserRoles { get; set; } = new List<UserRole>();  // Many-to-Many Relationship between User and Roles

        /// <summary>
        /// Creates the ViewModel for the calling User, <paramref name="roles"/> needed to visualise what roles are assigned.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>UserRoleViewModel for this User using the passed <paramref name="roles"/>.</returns>
        /// <author>Felix Nebel</author>
        public UserRoleViewModel CreateUserViewModel(IEnumerable<RoleViewModel> roles)
        {
            return new UserRoleViewModel()
            {
                Id = Id,
                Email = Email,
                FullName = FullName,
                Password = Password,
                Active = Active,
                Roles = roles.ToList()
            };
        }
        /// <summary>
        /// Creates the ViewModel for the calling User, <paramref name="roles"/> needed to visualise what roles are assigned and which are not.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>UserRoleViewModel for this User using the passed <paramref name="roles"/> and <paramref name="selectedRoles"/>.</returns>
        /// <author>Felix Nebel</author>
        public UserRoleViewModel CreateUserRoleViewModel(IEnumerable<Role> roles, IEnumerable<Role> selectedRoles)
        {
            List<RoleViewModel> roleViewModels = new List<RoleViewModel>();
            roles.ToList().ForEach(role => {
                roleViewModels.Add(new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Selected = selectedRoles.Contains(role)
                });
            });
            return new UserRoleViewModel() { Id = Id, Email = Email, FullName = FullName, Active = Active, Roles = roleViewModels, Password = Password};
        }

        public User() {}
    }
}