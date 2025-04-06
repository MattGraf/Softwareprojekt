using Replay.Models;
using Replay.Models.Account;
using Replay.Container;

namespace Replay.ViewModels
{
    /// <summary>
    /// ViewModel that stores all Procedures
    /// </summary>              
    public class ProcedureAdminIndexViewModel
    {

        /// <summary>
        /// List of Procedures
        /// </summary>
        public List<Procedure> Procedures = new List<Procedure>();

        /// <summary>
        /// Constructor to create a ProcedureUserViewModel
        /// </summary>
        /// <param name="user">The user for whom the ProcedureViewModel is created</param>
        public ProcedureAdminIndexViewModel()
        {
        }



        /// <summary>
        /// Adding a Procedure to the ViewModel
        /// </summary>
        /// <param name="procedure">The procedure that should be added</param>
        public void addProcedure(Procedure procedure)
        {
            this.Procedures.Add(procedure);
        }

        /// <summary>
        /// Removing a procedure from the Viewmodel
        /// </summary>
        /// <param name="procedure">The procedure that should be removed</param>
        public void removeProcedure(Procedure procedure)
        {
            this.Procedures.Remove(procedure);
        }
    }
}