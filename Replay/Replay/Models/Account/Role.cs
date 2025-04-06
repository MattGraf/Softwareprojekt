using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Replay.Models.Account.MTM;
using Replay.Models.MTM;
using Replay.ViewModels.Account;


namespace Replay.Models.Account
{
    /// <summary>
    /// Class <c>Role</c> models the defining feature of the <c>Role</c>.
    /// This just has the name, actual permissions are saved as the Many-to-Many relatioship with <see cref="Permisson"/>
    /// using <see cref="RolePermission"/>.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        [Key]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Role name is required.")]
        public string Name { get; set; }                                                                // The Name of the Role
        public virtual List<UserRole> UserRoles { get; set; } = new List<UserRole>();                   // Needed for correct Migration creation of Many-to-Many tables
        public virtual List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); // Needed for correct Migration creation of Many-to-Many tables
        public virtual List<TaskTemplateRole> TaskTemplateRoles {get; set;} = new List<TaskTemplateRole>();             // Needed for correct Migration creation of Many-to-Many tables
        public virtual List<MakandraTaskRole> TaskRoles { get; set; } = new List<MakandraTaskRole>();                   // Needed for correct Migration creation of Many-to-Many tables
        public virtual List<MakandraProcessRole> MakandraProcessRoles {get; set;} = new List<MakandraProcessRole>();    // Needed for correct Migration creation of Many-to-Many tables
        /// <summary>
        /// Creates the ViewModel for the calling User, <paramref name="roles"/> needed to visualise what roles are assigned and which are not.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>UserRoleViewModel for this User using the passed <paramref name="roles"/> and <paramref name="selectedRoles"/>.</returns>
        /// <author>Felix Nebel</author>
        public RolePermissionViewModel CreateRolePermissionViewModel(IEnumerable<Permission> permissions, IEnumerable<Permission> selectedPermissions)
        {
            List<PermissionViewModel> permissionViewModels = new List<PermissionViewModel>();
            permissions.ToList().ForEach(permission => {
                permissionViewModels.Add(new PermissionViewModel()
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    isAllowed = selectedPermissions.Contains(permission)
                });
            });
            return new RolePermissionViewModel() { Id = Id, Name = Name, PermissionViews = permissionViewModels };
        }
    }
}
