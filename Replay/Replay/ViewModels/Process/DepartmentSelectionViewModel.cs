using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.Process
{
    /// <summary> ViewModel to check if a Department has been selected as a checkbox in the Start View</summary>
    public class DepartmentSelectionViewModel
    {
        /// <summary>
        /// Name of a Department
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of a Department
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Boolean value indicating whether a Department has been selected as a checkbox in the Start View or not
        /// </summary>
        public bool IsSelected { get; set; }
    }
}