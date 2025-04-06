using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels
{
    /// <summary>
        /// ViewModel to check for each <see cref="Models.Department"/> if its checkbox is selected 
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public class DepartmentViewModel
        {
            public string Name { get; set; }
            public bool IsSelected { get; set; }
        }
}