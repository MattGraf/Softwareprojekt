using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.DuedateViewModels
{
    /// <summary>
    /// ViewModel of a single <see cref="Models.Duedate"/> for the overiew of <see cref="Models.TaskTemplate"/>
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class DuedateViewModel
    {
        public int ID {get; set;}
        public string Name {set; get;}
        public int Days {set; get;}
    }
}