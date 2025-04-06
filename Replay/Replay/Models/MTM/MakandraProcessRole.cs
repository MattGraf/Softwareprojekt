using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Replay.Models.Account;

namespace Replay.Models.MTM
{
    /// <summary>
    /// Model for connection between the <see cref="Models.MakandraProcess"/>s and <see cref="Account.Role"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class MakandraProcessRole
    {
        [Range(0, int.MaxValue)]
        public int MakandraProcessId {get; set;}
        public MakandraProcess MakandraProcess {get; set;}
        [Range(0, int.MaxValue)]
        public int RoleID {get; set;}
        public Role Role {get; set;}

        /// <summary>
        /// Overwrites the equals method
        /// Equals when the IDs are the same
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>If the Object equals this one</returns>
        /// <author>Matthias Grafberger</author>
        public override bool Equals(object obj)
        {
            var item = obj as MakandraProcessRole;

            if (item == null) return false;

            return this.MakandraProcessId == item.MakandraProcessId && this.RoleID == item.RoleID;
        }

        /// <summary>
        /// Checks if a MakandraProcessRole is valid
        /// </summary>
        /// <returns>0 if valid, 1 or 2 otherwise</returns>
        /// <param name="makandraProcessContainer">A MakandraProcessContainer</param>
        /// <param name="roleContainer">A RoleContainer</param>
        /// <author>Arian Scheremet, Matthias Grafberger</author>
        public int IsValid(MakandraProcessContainer makandraProcessContainer, RoleContainer roleContainer)
        {
            MakandraProcess makandraProcess = makandraProcessContainer.GetProcessFromId(MakandraProcessId).Result;
            if (makandraProcess is null) {
                return 1;
            } else {
                MakandraProcess = makandraProcess;
            }
            Role role = roleContainer.GetRoleFromId(RoleID).Result;
            if (role is null) {
                return 2;
            } else {
                Role = role;
            } 

            return 0;
        }
    }
}