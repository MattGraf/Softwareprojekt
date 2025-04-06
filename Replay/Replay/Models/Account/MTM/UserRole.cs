using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Replay.Models.Account.MTM
{
    public class UserRole
    {
        /// <summary>
        /// Models the Many-to-Many Relationship between <see cref="Account.User"/> and <see cref="Account.Role"/>.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int UserId { get; set; }             // The Id of the User (this gets saved in the database)
        public User User { get; set; }   // The corresponding object to the UserId (does NOT get saved in the database)
        [Range(0, int.MaxValue)]
        public int RoleId { get; set; }             // The Id of the Role (this gets saved in the database)
        public Role Role { get; set; }              // The corresponding object to the RoleId (does NOT get saved in the database)


    }
}
