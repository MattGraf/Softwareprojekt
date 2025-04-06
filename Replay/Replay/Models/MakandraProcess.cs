using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Net.Http.Headers;
using OpenQA.Selenium.DevTools.V123.DOM;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.Models
{
    /// <summary>
    /// Represents a MakandraProcess. Provides methods for MakandraProcess creation and editing.
    /// </summary>
    /// <author>Arian Scheremet</author>

    public class MakandraProcess
    {
        /// <summary>Unique identification for a <c>MakandraProcess</c>, assigned by a database.</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Name for a <c>MakandraProcess</c>.</summary>  
        [Required]
        public string Name { get; set; }

        /// <summary>A List of TaskTemplates a <c>MakandraProcess</c> consists of.</summary>       
        [Required]
        public List<TaskTemplate> Tasks { get; set; }

        /// <summary>A List of Many To Manys between a MakandraProcess and Roles. These Roles are permitted to edit this MakandraProcess and start Procedures from it</summary>
        public virtual List<MakandraProcessRole> MakandraProcessRoles { get; set; } = new List<MakandraProcessRole>();

        /// <summary>
        /// Creates a new MakandraProcess with given values and adds it to the database. Also creates Many To Manys between the MakandraProcess and the Roles and adds them to the database
        /// </summary>
        /// <param name="context">A MakandraContext</param>
        /// <param name="makandraProcessContainer">A MakandraProcessContainer</param>
        /// <param name="makandraProcessRoleContainer">A MakandraProcessRoleContainer</param>
        /// <param name="roleContainer">A RoleContainer</param>
        /// <param name="name">Name for this MakandraProcess</param>
        /// <param name="taskTemplates">A List of TaskTemplates this MakandraProcess consists of</param>
        /// <param name="roles">A List of Roles which have access to editing/starting this MakandraProcess</param>
        /// <author>Arian Scheremet</author>
        public static void CreateMakandraProcess(MakandraProcessContainer makandraProcessContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, RoleContainer roleContainer, string name, List<TaskTemplate> taskTemplates, List<Role> roles)
        {
            MakandraProcess makandraProcess = new MakandraProcess
            {
                Name = name ?? throw new ArgumentNullException(nameof(name)),
                Tasks = taskTemplates ?? throw new ArgumentNullException(nameof(taskTemplates))
            };

            makandraProcess.Id = makandraProcessContainer.AddProcess(makandraProcess).Result;
            foreach (Role r in roles)
            {
                MakandraProcessRole makandraProcessRole = new MakandraProcessRole
                {
                    MakandraProcessId = makandraProcess.Id,
                    MakandraProcess = makandraProcessContainer.GetProcessFromId(makandraProcess.Id).Result,
                    RoleID = r.Id,
                    Role = roleContainer.GetRoleFromId(r.Id).Result
                };
                makandraProcessRoleContainer.AddMakandraProcessRole(makandraProcessRole);
            }
        }

        /// <summary>
        /// Empty Constructor for Model Binding
        /// </summary>
        /// <author>Arian Scheremet</author>
        public MakandraProcess() { }

        /// <summary>
        /// Updates the MakandraProcess with the given Id with given values for roles and name. Overwrites Many To Manys between this MakandraProcess and its Roles with new connections. Saves all changes in the database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="makandraProcessContainer"></param>
        /// <param name="makandraProcessRoleContainer"></param>
        /// <param name="roleContainer"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="roles"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void EditMakandraProcess(MakandraProcessContainer makandraProcessContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, RoleContainer roleContainer, int? id, string? name, List<Role>? roles)
        {
            MakandraProcess makandraProcess = new MakandraProcess
            {
                Name = name ?? throw new ArgumentNullException(nameof(name)),
                Id = id ?? throw new ArgumentNullException(nameof(id))
            };

            makandraProcess = makandraProcessContainer.UpdateProcess(makandraProcess).Result;
            makandraProcessRoleContainer.DeleteMakandraProcessRoleWithMakandraProcess(makandraProcess);

            foreach (Role r in roles)
            {
                MakandraProcessRole makandraProcessRole = new MakandraProcessRole
                {
                    MakandraProcessId = makandraProcess.Id,
                    MakandraProcess = makandraProcessContainer.GetProcessFromId(makandraProcess.Id).Result,
                    RoleID = r.Id,
                    Role = roleContainer.GetRoleFromId(r.Id).Result
                };

                makandraProcess.MakandraProcessRoles.Add(makandraProcessRole);
                makandraProcessRoleContainer.AddMakandraProcessRole(makandraProcessRole);
            }
        }

        /// <summary>
        /// Checks if a MakandraProcess is valid
        /// </summary>
        /// <returns>0 if valid, 1 otherwise</returns>
        /// <author>Arian Scheremet</author>
        public int IsValid()
        {
            if (Name is null || Tasks is null || MakandraProcessRoles is null) return 1;
            return 0;
        }
    }
}