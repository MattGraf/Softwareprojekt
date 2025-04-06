using System.ComponentModel.DataAnnotations;

namespace Replay.ViewModels.Account
{
    /// <summary>
    /// <c>RolePermissionViewModel</c> models the necessary data to display for the Role Edit View.
    /// Contains all the necesary information for the Role and the Permission  it has access to.
    /// </summary>
    public class RolePermissionViewModel
    {
        // Role data, can't directly reference the Role here as it is a mapped EF-Entity.
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        // Permission/Actions displayed using the PermissionViewModel as we need to display whether the role has access to a
        // Permission/Action or not and we don't want to save every Role-Permission/Action pair.
        public List<PermissionViewModel> PermissionViews { get; set; }

    }


    /// <summary>
    /// View Model to display whether a role is allowed for a permission or not.
    /// </summary>
    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isAllowed { get; set; }
    }
}
