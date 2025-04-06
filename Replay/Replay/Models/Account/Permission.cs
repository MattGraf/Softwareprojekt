using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Replay.Models.Account.MTM;

namespace Replay.Models.Account
{
    /// <summary>
    /// Class <c>Permission</c> models the defining feature of the <c>Permission</c> 
    /// along with the <see cref="Role"/>s that have access to the <c>Permission</c>, mapped through a Many-to-Many relationship with <see cref="Role"/> using
    /// <see cref="MTM.RolePermission"/>.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Permission
    {
        [Key]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
        public string Name { get; set; }                        // Has to be unique
        public virtual List<RolePermission> RolePermission { get; set; } = new List<RolePermission>();  // Many-to-Many Relationship between Role and Site
    }
}
