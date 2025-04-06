using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.ViewModels.Process
{
    public class MakandraProcessDetailsViewModel
    {
        /// <summary>
        /// The name of the MakandraProcess to be viewed
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of Many To Manys between the MakandraProcess to be viewed and the Roles permitted to edit/start it
        /// </summary>
        public List<MakandraProcessRole> EditAccess = new List<MakandraProcessRole>();

        /// <summary>
        /// List of non-archived TaskTemplates the MakandraProcess to be viewed consists of
        /// </summary>
        public List<TaskTemplate> Tasks = new List<TaskTemplate>();
        /// <summary>
        /// List of archived TaskTemplates the MakandraProcess to be viewed consists of
        /// </summary>
        public List<TaskTemplate> ArchivedTasks = new List<TaskTemplate>();

        /// <summary>
        /// An Array of RoleSelectionViewModels responsible for displaying checkboxes in the corresponding Details View
        /// </summary>
        public RoleSelectionViewModel[] RoleSelection { get; set; } = new RoleSelectionViewModel[new RoleContainer(new MakandraContext()).GetRoles().Count()];

        /// <summary>
        /// Id of the MakandraProcess to be viewed
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of the TaskTemplate whose details are to be viewed from the MakandraProcess Details View. Needed for the Details button and a successful redirect to the MakandraProcess Details page from which the TaskTemplate details were viewed
        /// </summary>
        public int TaskTemplateId { get; set; }
    }
}