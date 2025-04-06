using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Replay.Models;
using Replay.Models.MTM;

using System.Text.Json;

namespace Replay.Container
{
    /// <summary>
    /// Management of the many-to-many-table between <see cref="Models.TaskTemplate"/> and <see cref="Models.Account.Role"/>
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateRoleContainer
    {
         private MakandraContext MakandraContext;

        /// <summary>
        /// Creates new Container
        /// </summary>
        /// <param name="makandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public TaskTemplateRoleContainer(MakandraContext makandraContext) {
            this.MakandraContext = makandraContext;
        }

        /// <summary>
        /// Adds a new connection between a <see cref="Models.TaskTemplate"/> and a <see cref="Models.Account.Role"/> in the database when it doesn't exist yet
        /// </summary>
        /// <param name="taskTemplateRole">Contain the connection which should be added</param>
        /// <author>Matthias Grafberger</author>
        public async void AddTaskTemplateRole(TaskTemplateRole taskTemplateRole) {
            var taskTemplateRoleWhenExists = await MakandraContext.TaskTemplateRoles
                .FirstOrDefaultAsync<TaskTemplateRole>(s => s.Equals(taskTemplateRole));
            
            if (taskTemplateRoleWhenExists is not null) return;

            MakandraContext.Add(taskTemplateRole);
            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a connection between a <see cref="Models.TaskTemplate"/> and a <see cref="Models.Account.Role"/> in the database
        /// </summary>
        /// <param name="taskTemplateRole">Contains the connection to be deleted</param>
        /// <author>Matthias Grafberger</author>
        public async void DeleteTaskTemplateRole(TaskTemplateRole taskTemplateRole) {
            var taskTemplateRoleToDelete = await MakandraContext.TaskTemplateRoles

                .FirstOrDefaultAsync<TaskTemplateRole>(s => s.Equals(taskTemplateRole));

            if (taskTemplateRoleToDelete is null) return;
            
            MakandraContext.Remove(taskTemplateRole);

            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes all MTM-entries from a specific <see cref="TaskTemplate"/>
        /// </summary>
        /// <param name="taskTemplate"><see cref="TaskTemplate"/> of which all entries are to deleted</param>
        /// <author>Matthias Grafberger</author>
         public void DeleteTaskTemplateRoleWithTaskTemplate(TaskTemplate taskTemplate) {
            List<TaskTemplateRole> taskTemplateRolesToDelete = MakandraContext.TaskTemplateRoles
                .Where(s => s.TaskTemplateID == taskTemplate.ID)
                .ToList();

            if (taskTemplateRolesToDelete is null) return;

            foreach (TaskTemplateRole taskTemplateRole in taskTemplateRolesToDelete) {
                MakandraContext.Remove(taskTemplateRole);
            }

            MakandraContext.SaveChanges();
        }

        /// <summary>
        /// Returns a list of all ids of <see cref="TaskTemplate"/>s which have EditAccess of a specific roleId
        /// </summary>
        /// <param name="roleId">Id of the role of which all <see cref="TaskTemplate"/>s are wanted with EditAccess</param>
        /// <returns>List of all ids of <see cref="TaskTemplate"/>s which have EditAccess of a specific roleId</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<int>> GetAssociatedTaskTemplateIdsFromRoleId(int roleId) {
            List<TaskTemplateRole> taskTemplateRoles = await MakandraContext.TaskTemplateRoles
                .Where(s => s.RoleID == roleId)
                .OrderBy(s => s.TaskTemplateID)
                .ToListAsync();


            List<int> Ids = new List<int>();
            foreach (var v in taskTemplateRoles) {
                Ids.Add(v.TaskTemplateID);
            }
        
            return Ids;
        }


        /// <summary>
        /// Imports a Json-string with <see cref="TaskTemplateRole"/>s in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(TaskTemplateContainer taskTemplateContainer, RoleContainer roleContainer, string jsonFile) {

            if (jsonFile is null) return;

                
            List<TaskTemplateRole> taskTemplateRoles = new List<TaskTemplateRole>();

            try {
                taskTemplateRoles = JsonSerializer.Deserialize<List<TaskTemplateRole>>(jsonFile);
            } catch (InvalidOperationException e) {
                return;
            }

            taskTemplateRoles.ForEach(e => {

                int h = e.IsValid(taskTemplateContainer, roleContainer);
                if (h == 0) {
                    AddTaskTemplateRole(e);
                } else {
                    Console.WriteLine(h);
                    Console.WriteLine("TaskTemplateRole couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}