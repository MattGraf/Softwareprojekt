using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.Process
{
    /// <summary> ViewModel to check if a Role has been selected as a checkbox</summary>
    public class RoleSelectionViewModel
    {
        /// <summary>
        /// Name of a Role
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of a Role
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Boolean value indicating whether a Role has been selected as a checkbox in a Create/Edit/Details View or not
        /// </summary>
        public bool IsSelected { get; set; }
    }
}