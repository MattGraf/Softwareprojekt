using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.Task
{
    /// <summary>
    /// ViewModel to store reduced information
    /// of a <see cref="Role"/> in association with
    /// a <see cref="MakandraTask"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskRoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}