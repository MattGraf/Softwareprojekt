using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.ViewModels.Process
{
    public class MakandraProcessEditViewModel
    {
        /// <summary>
        /// Name of the MakandraProcess to be edited
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the MakandraProcess to be edited
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A RoleContainer
        /// </summary>
        public RoleContainer? Roles { get; set; }

        /// <summary>
        /// A TaskTemplateContainer
        /// </summary>
        public TaskTemplateContainer? AllTaskTemplatesForThisProcess { get; set; }

        /// <summary>
        /// An Array of RoleSelectionViewModels responsible for handling checkboxes in the corresponding Edit View
        /// </summary>
        public RoleSelectionViewModel[] RoleSelection { get; set; }

    }
}