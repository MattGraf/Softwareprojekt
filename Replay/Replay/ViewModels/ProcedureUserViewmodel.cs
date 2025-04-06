using Replay.Models;
using Replay.Models.Account;
using Replay.Container;

namespace Replay.ViewModels
{
    /// <summary>
    /// ViewModel that stores all Procedures belonging to a specific user
    /// </summary>   
    /// <author>Florian Fendt</author>
    public class ProcedureUserViewModel
    {
        /// <summary>
        /// User whose Procedures are stored
        /// </summary>
        public User User;

        /// <summary>
        /// List of Procedures that belong to the user specified
        /// </summary>
        public List<Procedure> Procedures = new List<Procedure>();

        /// <summary>
        /// Constructor to create a ProcedureUserViewModel
        /// </summary>
        /// <param name="user">The user for whom the ProcedureViewModel is created</param>
        /// <author>Florian Fendt</author>
        public ProcedureUserViewModel(User user)
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