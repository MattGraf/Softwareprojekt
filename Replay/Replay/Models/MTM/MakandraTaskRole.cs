using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models.Account;
using Replay.ViewModels.Task;

namespace Replay.Models.MTM
{
    /// <summary>
    /// Model for monaging the Many-to-Many-Relation between
    /// <see cref="MakandraTask"/> and <see cref="Role"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskRole
    {
        [Range(0, int.MaxValue)]
        public int TaskId { get; set; }
        public MakandraTask Task { get; set; }
        [Range(0, int.MaxValue)]
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var item = (MakandraTaskRole) obj;
            
            return TaskId == item.TaskId && RoleId == item.RoleId;
        }
    }
}