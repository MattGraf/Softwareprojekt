using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.Task
{
    /// <summary>
    /// ViewModel to store information of a
    /// <see cref="MakandraTaskState"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskStateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}