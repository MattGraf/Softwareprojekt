using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.ViewModels.DuedateViewModels
{
    /// <summary>
    /// ViewModel for the editing of a <see cref="Models.Duedate"/>
    /// Contains only the attributes which are necessary
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class DuedateEditViewModel
    {
        public int ID {get; set;}

        [Required(ErrorMessage = "Ein Name muss angegeben werden")]
        public string Name {set; get;}
        
        [Required(ErrorMessage = "Die Tage müssen angegeben werden")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Es muss eine ganze Zahl eingegeben werden")]
        public string Days {set; get;}

        public bool Exists {get; set;} = false;

        public bool ParseError {get; set;} = false;
    }
}