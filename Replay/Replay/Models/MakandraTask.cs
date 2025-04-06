using Replay.Models.Account;
using Replay.Models;
using Replay.Models.MTM;
using Replay.Container;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualBasic;
using Replay.ViewModels.Task;

namespace Replay.Models
{
    /// <summary>
    /// Model of a specific task.
    /// Provides methods <see cref="InitializeTask"/> to create a task
    /// based on a <see cref="Procedure"/> and <see cref="TaskTemplate"/>,
    /// <see cref="UpdateTaskDetails"/> to update attribute values,
    /// additionally helper methods are provided:
    /// <see cref="UpdateTaskRoles"/> and <see cref="SelectTargetDate"/> 
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Instruction { get; set; }
        public Role? ResponsibleRole { get; set; }
        public int? ResponsibleRoleId { get; set; }
        public virtual List<MakandraTaskRole> EditAccess { get; set; } = new List<MakandraTaskRole>();
        public DateTime TargetDate { get; set; }
        public string? Notes { get; set; }
        [Required]
        public MakandraTaskState State { get; set; }
        public int StateId { get; set; }
        public Procedure Procedure { get; set; }
        public int ProcedureId { get; set; }
        public User? Assignee { get; set; }
        public int? AssigneeId { get; set; }
        [Required]
        public bool Archived { get; set; }

        /// <summary>
        /// Initialises a new <see cref="MakandraTask"/> based on a <see cref="TaskTemplate"/> and a <see cref="Procedure"/>
        /// It checks the following details:
        /// If the <see cref="DueDate"/> of the template is set to <c>asap</c>, the target date is set to the current date.
        /// Otherwise the date is calculated in relation to the <see cref="DueDate"/> specifics.
        /// The initial <see cref="MakandraTaskState"/> of the new task is automatically set to <c>Offen</c>
        /// The task's <c>Archived</c> status is set to <c>false</c>
        /// The name, instruction, procedure and assignee are taken from the input data.
        /// As soon as this task has been added to the database, the many-to-many tables are created and updated
        /// to correctly associate <see cref="ContractType"/> and <see cref="Role"/> with the task
        /// </summary>
        /// <param name="procedure">Input <see cref="Procedure"/> that this task should be part of. It contains the information for <c>Assignee</c>, <c>ResponsibleRole</c></param>
        /// <param name="template">Input <see cref="TaskTemplate"/> that this task is based on. It contains the information for <c>Name</c>, <c>Instruction</c></param>
        /// <param name="deadlineDate"></param>
        /// <param name="assignee"></param>
        /// <author>Thomas Dworschak</author>
        public async Task InitializeTask(Procedure procedure, TaskTemplate template, MakandraTaskContainer container, MakandraTaskStateContainer states, MakandraTaskRoleContainer roles, DuedateContainer duedates, ProcedureContainer procedures, RoleContainer roleContainer, UserContainer userContainer)
        {
            int newAssigneeId = 0;

            int newRoleId = 0;

            switch (template.DefaultResponsible)
            {
                case "Vorgangsverantwortlicher":
                    newAssigneeId = procedure.ResponsiblePersonId;
                    break;
                case "Bezugsperson":
                    newAssigneeId = procedure.ReferencePersonId;
                    break;
                default:
                    newRoleId = roleContainer.GetRoleFromName(template.DefaultResponsible).Id;
                    break;
            }

            User? user = await userContainer.GetUserFromId(newAssigneeId);
            Role? role = await roleContainer.GetRoleFromId(newRoleId);
            MakandraTaskState state = await states.GetMakandraTaskStateFromName("Offen");
            Procedure procedureDatabase = await procedures.getProcedureFromId(procedure.Id);

            MakandraTask task = new MakandraTask
            {
                Name = template.Name,
                Instruction = template.Instruction,
                State = state,
                StateId = state.Id,
                Procedure = procedureDatabase,
                ProcedureId = procedureDatabase.Id,
                TargetDate = await SelectTargetDate(template.DuedateID, procedure.Deadline, duedates),
                Archived = false,
                Assignee = user,
                AssigneeId = (user is not null) ? newAssigneeId : null,
                ResponsibleRole = role,
                ResponsibleRoleId = (role is not null) ? newRoleId : null
            };

            await container.AddMakandraTask(task);


            if (template.EditAccess.Count != 0)
            {
                foreach (TaskTemplateRole templateRole in template.EditAccess)
                {
                    MakandraTaskRole taskRole = new MakandraTaskRole
                    {
                        TaskId = task.Id,
                        Task = task,
                        RoleId = templateRole.RoleID,
                        Role = templateRole.Role
                    };

                    roles.AddTaskRole(taskRole);
                }
            }
        }
       
        /// <summary>
        /// Updates current instance of <see cref="MakandraTask"/> according to the input parameters.
        /// To reestablish the many-to-many-association between <see cref="MakandraTask"/> and
        /// <see cref="Role"/>, a helper method is called.
        /// </summary>
        /// <param name="roleContainer"><see cref="RoleContainer"/> that is needed to access the <see cref="Role"> that the input <c>roleId</c> and <c>editorIds</c> refer to. Additionally <see cref="UpdateTaskRoles"/> needs this container too</param>
        /// <param name="taskRoleContainer"><see cref="TaskRoleContainer"/> that the <see cref="UpdateTaskRoles"/> method needs</param>
        /// <param name="stateContainer"><see cref="MakandraTaskStateContainer"/> that is needed to access the <see cref="MakandraTaskState"/> that the input <c>stateId</c> refers to</param>
        /// <param name="userContainer"><see cref="UserContainer"/> that is needed to access the <see cref="User"/> that the input <c>assigneeId</c> refers to</param>
        /// <param name="name">Name that the task is supposed to be updated to</param>
        /// <param name="instruction">Instruction that the task is supposed to be updated to</param>
        /// <param name="targetDate">TargetDate that the task is supposed to be updated to</param>
        /// <param name="notes">Notes that the task is supposed to be updated to</param>
        /// <param name="assigneeId">Id of the <see cref="User"/> that should be now assigned to the task</param>
        /// <param name="archived">Bool value that the archive value of the task is supposed to be updated to</param>
        /// <param name="roleId">Id of the <see cref="Role"/> that should be responsible now for the task</param>
        /// <param name="stateId">Id of the <see cref="MakandraTaskState"/> that the task should now be in</param>
        /// <param name="roles">List of <see cref="Role"/> roles that currently have edit access of the task</param>
        /// <param name="editorIds">List of Ids of these roles that have currently edit acces of the task</param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <author>Thomas Dworschak</author>
        public async Task UpdateTaskDetails(RoleContainer roleContainer, MakandraTaskRoleContainer taskRoleContainer, MakandraTaskStateContainer stateContainer, UserContainer userContainer, string name, string? instruction, DateTime targetDate, string? notes, int? assigneeId, bool archived, int? roleId, int stateId, List<Role?> roles, List<int> editorIds)
        {
            Name = name;
            Instruction = instruction;
            TargetDate = targetDate;
            Notes = notes;
            Archived = archived;

            if (roleId.HasValue)
            {
                Role? role = await roleContainer.GetRoleFromId((int) roleId);
                ResponsibleRole = role;
                ResponsibleRoleId = roleId;
            }
            else
            {
                ResponsibleRole = null;
            }

            MakandraTaskState state = await stateContainer.GetMakandraTaskStateFromId(stateId);
            
            State = state;
            StateId = stateId;

            await UpdateTaskRoles(taskRoleContainer, roles, editorIds, roleContainer);

            if (assigneeId == null)
            {
                Assignee = null;
                AssigneeId = null;
            }
            else
            {
                var assignee = await userContainer.GetUserFromId((int) assigneeId);
                if (assignee == null)
                {
                    throw new KeyNotFoundException($"User with ID {assigneeId} not found in the database.");
                }
                Assignee = assignee;
                AssigneeId = assignee.Id;
            }
        }

        /// <summary>
        /// This method updated the many-to-many relation between <see cref="MakandraTask"/> and <see cref="Role"/>
        /// to manage who has edit acces to a task.
        /// It takes a list of <see cref="Role"/> roles with their ids and compares these with
        /// already associated roles in the database.
        /// Then, the method deletes all relations that refer to a role that is not in the list of the input roles.
        /// Afterwards any new role is added to the relation
        /// The final list of roles is added to the List of roles of this task
        /// </summary>
        /// <param name="taskRoleContainer"><see cref="MakandraTaskRoleContainer"/> that is needed to manage the database entries of the many to many relation</param>
        /// <param name="roles">List of <see cref="Role"/> roles that currently have access to a <see cref="MakandraTask"/></param>
        /// <param name="editorIds">List of ids of these <see cref="Role"/> roles that currently have access to a <see cref="MakandraTask"/></param>
        /// <param name="roleContainer"><see cref="RoleContainer"/> to retrieve any <see cref="Role"/> corresponding to an id</param>
        /// <author>Thomas Dworschak</author>        
        public async Task UpdateTaskRoles(MakandraTaskRoleContainer taskRoleContainer, List<Role?> roles, List<int> editorIds, RoleContainer roleContainer)
        {
            List<MakandraTaskRole> existingTaskRoles = await taskRoleContainer.GetRolesFromTask(this);

            foreach (MakandraTaskRole existingTaskRole in existingTaskRoles)
            {
                if (!editorIds.Contains(existingTaskRole.RoleId))
                {
                    taskRoleContainer.DeleteTaskRole(existingTaskRole);
                }
            }

            foreach (int editorId in editorIds)
            {
                taskRoleContainer.AddTaskRole(new MakandraTaskRole
                {
                    TaskId = Id,
                    Task = this,
                    RoleId = editorId,
                    Role = await roleContainer.GetRoleFromId(editorId)
                });
            }

            foreach (Role role in roles)
            {
                MakandraTaskRole taskRole = new MakandraTaskRole
                {
                    TaskId = Id,
                    Task = this,
                    RoleId = role.Id,
                    Role = role
                };
                
                EditAccess.Add(taskRole);
            }
        }

        /// <summary>
        /// This method creates a <see cref="DateTime"/> and assigns it to the <c>TargetDate</c> attribute
        /// of this <see cref="MakandraTask"/>.
        /// The method considers a deadline of a <see cref="Procedure"/> and a <see cref="Duedate"/>
        /// specified in a <see cref="TaskTemplate"/>
        /// If the duedate is <c>ASAP</c> the date is set to current date, otherwise
        /// a date in relation to the deadline date specified by the <see cref="Duedate"/>
        /// is created and assigned
        /// </summary>
        /// <param name="duedateId">Id of the <see cref="Duedate"/> that specifies the target date</param>
        /// <param name="deadline"><see cref="DateTime"/> of the <see cref="Procedure"/></param>
        /// <author>Thomas Dworschak</author>
        public async Task<DateTime> SelectTargetDate(int duedateId, DateTime deadline, DuedateContainer duedateContainer)
        {
            DateTime newTargetDate = new DateTime();
            var asap = await duedateContainer.GetDuedateFromId(1);
            var templateDuedate = await duedateContainer.GetDuedateFromId(duedateId);

            if (templateDuedate.Equals(asap))
            {
                newTargetDate = DateTime.Today;
            }
            else
            {
                newTargetDate = deadline.AddDays(templateDuedate.Days);
            }

            return newTargetDate;
        }
    }
}