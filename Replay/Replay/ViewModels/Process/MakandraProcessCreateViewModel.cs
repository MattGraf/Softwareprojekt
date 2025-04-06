using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Replay.Models;
using Replay.Authorization;
using Replay.ViewModels;
using Replay.ViewModels.Process;
using Replay.Models.Account;

namespace Replay.ViewModels.Process
{
    public class MakandraProcessCreateViewModel
    {
        /// <summary>
        /// Name for the new MakandraProcess
        /// </summary>
        [Required(ErrorMessage = "Der Bezeichner darf nicht leer sein.")]
        public string Name { get; set; }

        /// <summary>
        /// List of Roles permitted to edit/start the new MakandraProcess
        /// </summary>
        public List<Role> EditAccess { get; set; } = new List<Role>();

        /// <summary>
        /// List of TaskTemplates the new MakandraProcess consists of
        /// </summary>
        public List<TaskTemplate> Tasks { get; set; } = new List<TaskTemplate>();

        /// <summary>
        /// A TaskTemplateContainer
        /// </summary>
        public TaskTemplateContainer? AllTasks { get; set; }

        /// <summary>
        /// A RoleContainer
        /// </summary>
        public RoleContainer? AllRoles { get; set; }

        /// <summary>
        /// An Array of RoleSelectionViewModels responsible for handling checkboxes in the corresponding Create View
        /// </summary>
        public RoleSelectionViewModel[] Roles { get; set; }
    }
}