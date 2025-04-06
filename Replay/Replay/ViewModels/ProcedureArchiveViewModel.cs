using Replay.Models;
using Replay.Models.Account;
using Replay.Container;

namespace Replay.ViewModels
{
    /// <summary>
    /// ViewModel that stores all Procedures that are in the Archive and are visible to the ResponsiblePerson or Roles with EditAccess to the Process the Procedure is based upon
    /// </summary>  
    /// <author>Florian Fendt</author>
    public class ProcedureArchiveViewModel
    {
        /// <summary>
        /// User whose Procedures are stored
        /// </summary>
        /// <author>Florian Fendt</author>
        public User User;

        /// <summary>
        /// List of Procedures that belong to the user because he is responsible for them or he has editAccess for the process the procedure is based upon
        /// </summary>
        /// <author>Florian Fendt</author>
        public List<Procedure> Procedures = new List<Procedure>();

        /// <summary>
        /// Constructor to create a ProcedureArchiveViewModel
        /// </summary>
        /// <param name="user">The user for whom the ProcedureViewModel is created</param>
        /// <author>Florian Fendt</author>
        public ProcedureArchiveViewModel(User user)
        {
        
            this.User = user;
        }

        /// <summary>
        /// Adding a Procedure to the ViewModel
        /// </summary>
        /// <param name="procedure">The procedure that should be added</param>
        /// <author>Florian Fendt</author>
        public void addProcedure(Procedure procedure)
        {
            this.Procedures.Add(procedure);
        }

        /// <summary>
        /// Removing a procedure from the Viewmodel
        /// </summary>
        /// <param name="procedure">The procedure that should be removed</param>
        /// <author>Florian Fendt</author>
        public void removeProcedure(Procedure procedure)
        {
            this.Procedures.Remove(procedure);
        }
    }
}