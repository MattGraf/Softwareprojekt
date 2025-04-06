using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Replay.Models.Account;
    

namespace Replay.Models.MTM
{
    /// <summary>
    /// Model for connection between the <see cref="Models.TaskTemplate"/>s and <see cref="Account.Role"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateRole
    {
        [Range(0, int.MaxValue)]
        public int TaskTemplateID {get; set;}
        public TaskTemplate TaskTemplate {get; set;}
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
            var item = obj as TaskTemplateRole;

            if (item == null) return false;

            return this.TaskTemplateID == item.TaskTemplateID && this.RoleID == item.RoleID;
        }

        /// <summary>
        /// Checks if a TaskTemplateRole is valid to save it in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid(TaskTemplateContainer taskTemplateContainer, RoleContainer roleContainer) {
            TaskTemplate taskTemplate = taskTemplateContainer.GetTaskTemplateFromId(TaskTemplateID).Result;
            if (taskTemplate is null) {
                return 1;
            } else {
                TaskTemplate = taskTemplate;
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