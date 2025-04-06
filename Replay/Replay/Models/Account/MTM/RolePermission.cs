using System.ComponentModel.DataAnnotations;

namespace Replay.Models.Account.MTM
{
    /// <summary>
    /// Models the Many-to-Many Relationship between <see cref="Account.Role"/> and <see cref="Models.Permisson"/> to save Permissions.
    /// </summary>
    public class RolePermission
    {
        [Range(0, int.MaxValue)]
        public int RoleId { get; set; }     // The Id of the Role (this gets saved in the database)
        public Role Role { get; set; }      // The corresponding object to the RoleId (does NOT get saved in the database)
        [Range(0, int.MaxValue)]
        public int PermissionId { get; set; }     // The Id of the Permission (this gets saved in the database)
        public Permission Permission { get; set; }      // The corresponding object to the SiteId (does NOT get saved in the database)
    }
}
