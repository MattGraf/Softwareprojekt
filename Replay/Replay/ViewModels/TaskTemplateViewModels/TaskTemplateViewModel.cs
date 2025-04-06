using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.TaskTemplateViewModels
{
    /// <summary>
    /// ViewModel for overviews of <see cref="Models.TaskTemplate"/>
    /// ID and Name only attributes which are necessary
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateViewModel
    {
        public int ID {get; set;}
        public string Name {get; set;}
        
        public string ProcessName {get; set;}
    }
}