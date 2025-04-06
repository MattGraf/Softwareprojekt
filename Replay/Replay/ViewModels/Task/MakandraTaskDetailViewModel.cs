using Replay.Models.Account;
using Replay.Models.MTM;
using Replay.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Replay.ViewModels.Task
{
    /// <summary>
    /// Detail view model that holds string values of attributes related to a <see cref="MakandraTask"/>.
    /// Contains a helper method to create a view model
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Anleitung")]
        public string? Instruction { get; set; }
        [DisplayName("Zugriffsberechtigt")]
        public List<string> EditAccess { get; set; } = new List<string>();
        [DisplayName("Zieldatum")]
        [DisplayFormat(DataFormatString = "{0: d MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TargetDate { get; set; }
        [DisplayName("Notizen")]
        public string? Notes { get; set; }
        [DisplayName("Aufgabenstatus")]
        public string State { get; set; }
        [DisplayName("Aufgabenzuständiger")]
        public string? Assignee { get; set; }
        [DisplayName("Verantwortliche Rolle")]
        public string? ResponsibleRole { get; set; }
        [DisplayName("Vorgang")]
        public string? Procedure { get; set; }
        [DisplayName("Archiviert")]
        public bool Archived { get; set; }

        /// <summary>
        /// Creates a new <see cref="MakandraTaskDetailViewModel"/> based on the attribute values of an input <see cref="MakandraTask"/>
        /// It first sets the simple string attributes to the values in the input task
        /// and then fills the lists by looping through the individual value to extract the strings
        /// </summary>
        /// <param name="task"><see cref="MakandraTask"/> input task that this view model is based upon</param>
        /// <param name="roleContainer">Container for <see cref="Role"/> that is needed to retrieve the role based on the id listed in the input task</param>
        /// <param name="userContainer">Container for <see cref="User"/> that is needed to retrieve the assignee based on the id listed in the input task</param>
        /// <param name="procedureContainer">Container for <see cref="Prcoedure"/> that is needed to retrieve the procedure based on the id listed in the input task</param>
        /// <param name="stateContainer">Container for <see cref="MakandraTaskState"/> that is needed to retrieve the states based on the ids listed in the input task</param>
        /// <returns><see cref="MakandraTaskDetailViewModel"/> with string values for each attribute</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<MakandraTaskDetailViewModel> CreateDetailViewModel(MakandraTask task, RoleContainer roleContainer, UserContainer userContainer, ProcedureContainer procedureContainer, MakandraTaskStateContainer stateContainer)
        {
            Role role = task.ResponsibleRoleId.HasValue ? await roleContainer.GetRoleFromId(task.ResponsibleRoleId.Value) : null;
            User user = task.AssigneeId.HasValue ? await userContainer.GetUserFromId(task.AssigneeId.Value) : null;
            Procedure procedure = await procedureContainer.getProcedureFromId(task.ProcedureId);
            MakandraTaskState state = await stateContainer.GetMakandraTaskStateFromId(task.StateId);

            MakandraTaskDetailViewModel detailViewModel = new MakandraTaskDetailViewModel()
            {
                Id = task.Id,
                Name = task.Name,
                Instruction = task.Instruction,
                ResponsibleRole = role?.Name,
                TargetDate = task.TargetDate,
                Notes = task.Notes,
                State = state.Name,
                Assignee = user?.FullName,
                Procedure = procedure.name,
                Archived = task.Archived
            };

            foreach (MakandraTaskRole taskRole in task.EditAccess)
            {
                role = await roleContainer.GetRoleFromId(taskRole.RoleId);
                detailViewModel.EditAccess.Add(role.Name);
            }

            return detailViewModel;
         }
    }

}