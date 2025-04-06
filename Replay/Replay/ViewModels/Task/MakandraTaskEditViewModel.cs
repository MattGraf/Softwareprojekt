using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V123.Network;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.ViewModels.Task
{
    /// <summary>
    /// View model for the editing process of a <see cref="MakandraTask"/>
    /// It contains additional selected ids to track which options
    /// have been chosen in the editing process.
    /// Helper methods are provided to get all selected and non selected options
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ein Name muss angegeben werden"), DisplayName("Aufgabename")]
        public string Name { get; set; }
        [DisplayName("Anleitung")]
        public string? Instruction { get; set; }
        public List<MakandraTaskRoleViewModel> PossibleRoles { get; set; } = new List<MakandraTaskRoleViewModel>();
        public int? SelectedRoleId { get; set; }
        public List<MakandraTaskRoleViewModel> PossibleEditors { get; set; } = new List<MakandraTaskRoleViewModel>();
        public List<int> SelectedEditorIds { get; set; } = new List<int>();
        [Required(ErrorMessage = "Ein Zieldatum muss angegeben werden")]
        [DisplayName("Zieldatum")]
        [DisplayFormat(DataFormatString = "{0: dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TargetDate { get; set; }
        [DisplayName("Notizen")]
        public string? Notes { get; set; }
        public List<MakandraTaskStateViewModel> States { get; set; } = new List<MakandraTaskStateViewModel>();
        public int SelectedStateId { get; set; }
        [DisplayName("Aufgabenzuständiger")]
        public List<MakandraTaskAssigneeViewModel> PossibleAssignees { get; set; } = new List<MakandraTaskAssigneeViewModel>();
        public int? SelectedAssigneeId { get; set; }
        public List<MakandraTaskProcedureViewModel> PossibleProcedures { get; set; } = new List<MakandraTaskProcedureViewModel>();
        public int SelectedProcedureId { get; set; }
        [Required(ErrorMessage = "Der Archivierungszustand muss angegeben werden"), DisplayName("Archivieren")]
        public bool Archived { get; set; }

        /// <summary>
        /// Creates a new <see cref="MakandraTaskEditViewModel"/> based on an input <see cref="MakandraTask"/> task.
        /// Additionally, <see cref="Role"/> with ids are provided to fill the lists
        /// of selected instances of the attributes.
        /// The attributes of the task are assigned to that of the view model with the lists being created
        /// by calling the helper methods.
        /// The containers are handed down for consistency reasons as the helper methods should access
        /// the same database context.
        /// </summary>
        /// <param name="task">Input <see cref="MakandraTask"/> task that contains all information</param>
        /// <param name="roles">Input <see cref="Role"/> list of roles</param>
        /// <param name="roleIds">Integer id values of these roles</param>
        /// <param name="roleContainer"><see cref="RoleContainer"/> that is needed when calling <see cref="GetPossibleRoles"/></param>
        /// <param name="statesContainer"><see cref="MakandraTaskStateContainer"/> that is needed when calling <see cref="GetPossibleStates"/></param>
        /// <param name="userContainer"><see cref="UserContainer"/>that is needed when calling <see cref="GetPossibleAssignees"/></param>
        /// <returns><see cref="MakandraTaskEditViewModel"/> filled with the attribute values of the input task</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<MakandraTaskEditViewModel> CreateEditViewModel(MakandraTask task, List<MakandraTaskRoleViewModel> roles, List<int> roleIds, RoleContainer roleContainer, MakandraTaskStateContainer statesContainer, UserContainer userContainer)
        {
            MakandraTaskEditViewModel editViewModel = new MakandraTaskEditViewModel
            {
                Name = task.Name,
                Instruction = task.Instruction,
                PossibleRoles = await GetPossibleRoles(roleContainer),
                SelectedRoleId = task.ResponsibleRoleId.HasValue ? task.ResponsibleRoleId : 0,
                PossibleEditors = roles,
                SelectedEditorIds = roleIds,
                TargetDate = task.TargetDate,
                Notes = task.Notes,
                SelectedStateId = task.StateId,
                States = await GetPossibleStates(statesContainer),
                PossibleAssignees = await GetPossibleAssignees(userContainer),
                SelectedAssigneeId = task.AssigneeId.HasValue ? task.AssigneeId : 0,
                Archived = task.Archived
            };
            return editViewModel;
        }

        public async void SetPossibleTaskStates(MakandraTaskStateContainer makandraTaskStateContainer) {
            this.States = await GetPossibleStates(makandraTaskStateContainer);
        }

        /// <summary>
        /// Retrieves all <see cref="Role"/> roles from the database
        /// that have an association with this <see cref="MakandraTaskEditViewModel"/>
        /// Id
        /// </summary>
        /// <param name="roleContainer"><see cref="RoleContainer"/> to retrieve all <see cref="Role"/> roles from the database</param>
        /// <returns>List of associated <see cref="Role"/></returns>
        /// <author>Thomas Dworschak</author>
        private async Task<List<MakandraTaskRoleViewModel>> GetPossibleRoles(RoleContainer roleContainer)
        {
            var roles = roleContainer.GetRoles();

            return roles.Select(r => new MakandraTaskRoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToList();
        }

        /// <summary>
        /// Retrieves all <see cref="MakandraTaskState"/> states from the database
        /// that have an association with this <see cref="MakandraTaskEditViewModel"/>
        /// Id
        /// </summary>
        /// <param name="statesContainer"><see cref="MakandraTaskStateContainer"/> to retrieve all <see cref="MakandraTaskState"/> states from the database</param>
        /// <returns>List of associated <see cref="MakandraTaskState"/></returns>
        /// <author>Thomas Dworschak</author>
        private async Task<List<MakandraTaskStateViewModel>> GetPossibleStates(MakandraTaskStateContainer statesContainer)
        {
            var states = await statesContainer.GetMakandraTaskStates();

            return states.Select(s => new MakandraTaskStateViewModel
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToList();
        }

        /// <summary>
        /// Retrieves all <see cref="User"/> states from the database
        /// that have an association with this <see cref="MakandraTaskEditViewModel"/>
        /// Id
        /// </summary>
        /// <param name="userContainer"><see cref="UserContainer"/> to retrieve all <see cref="User"/> users from the database</param>
        /// <returns>List of associated <see cref="User"/></returns>
        /// <author>Thomas Dworschak</author>
        private async Task<List<MakandraTaskAssigneeViewModel>> GetPossibleAssignees(UserContainer userContainer)
        {
            userContainer = new UserContainer(new MakandraContext());
            var users = userContainer.GetUsers();

            return users.Select(u => new MakandraTaskAssigneeViewModel
            {
                Id = u.Id,
                Name = u.FullName
            })
            .ToList();
        }
    }
}