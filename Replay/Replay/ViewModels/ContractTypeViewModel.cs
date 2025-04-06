using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Replay.Models.MTM;

namespace Replay.ViewModels
{
    /// <summary>
    /// ViewModel to check for each <see cref="Models.ContractType"/> if its checkbox is selected 
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class ContractTypeViewModel
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}