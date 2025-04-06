using Replay.Data;
using Replay.Models;
using Replay.Models.Account;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Replay.Container
{
    /// <summary>
    /// Container class that manages database queries
    /// </summary>
    /// <author>Florian Fendt</author>
    public class ProcedureContainer
    {
        /// <summary>
        /// The context used to connect with database
        /// </summary>
        /// <author>Florian Fendt</author>
        private MakandraContext context;

        /// <summary>
        /// Constructor to create a ProcedureContainer
        /// <param name="context">The context provided to interact with database</param>
        /// </summary>
        /// <author>Florian Fendt</author>
        public ProcedureContainer(MakandraContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Method to add a Procedure to the database
        /// </summary>
        /// <param name="procedure">The Procedure that should be added to the database</param>
        /// <returns></returns>
        /// <author>Florian Fendt</author>
        public async Task<int> addProcedure(Procedure procedure)
        {
            context.Procedures.Add(procedure);
            await context.SaveChangesAsync();

            return procedure.Id;
        }
        
        /// <summary>
        /// Method to update an already existing Procedure
        /// If the Procedure cannot be found in the database a new one is created
        /// </summary>
        /// <param name="procedure">The Procedure that needs to be updated with already updated attributes</param>
        /// <returns></returns>
        /// <author>Florian Fendt</author>
        public async Task updateProcedure(Procedure procedure)
        {
            var procedureToUpdate = await context.Procedures
                .FirstOrDefaultAsync<Procedure>(x => x.Id == procedure.Id);
            if(procedureToUpdate is null){
                addProcedure(procedure);
            } else {
            procedureToUpdate.name = procedure.name;
            procedureToUpdate.Deadline = procedure.Deadline;
            procedureToUpdate.EstablishingContractTypeId = procedure.EstablishingContractTypeId;
            procedureToUpdate.ResponsiblePersonId = procedure.ResponsiblePersonId;
            procedureToUpdate.ReferencePersonId = procedure.ReferencePersonId;
            procedureToUpdate.completedTasks = procedure.completedTasks;
            procedureToUpdate.openTasks = procedure.openTasks;
            procedureToUpdate.inprogressTasks = procedure.inprogressTasks;
            procedureToUpdate.progressbar = procedure.progressbar;
            await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Returns all Procedures currently stored in database
        /// </summary>
        /// <returns>List of Procedures</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<Procedure>> getAllProcedures()
        {
            var result = await context.Procedures.ToListAsync();
            foreach(var element in result){
                Procedure temp = await UpdateCompletionState(element);
            }
            return result;
        }

        /// <summary>
        /// Returns a specific Procedure based on a provided id
        /// </summary>
        /// <param name="id">The id of the Procedure wanted</param>
        /// <returns>Procedure-Object</returns>
        /// <exception cref="KeyNotFoundException">Exception thrown if no Procedure with the provided id could be found</exception>
        /// <author>Florian Fendt</author>
        public async Task<Procedure> getProcedureFromId(int id)
        {
            Procedure result = await context.Procedures
                .FirstOrDefaultAsync(x => x.Id == id);
            var final = await UpdateCompletionState(result);
            if(final == null){
                throw new KeyNotFoundException($"Procedure with ID {id} not found.");
            }
            return final;
        }

        /// <summary>
        /// Returns all Procedures a specific user is respsonsible for
        /// </summary>
        /// <param name="user">The user whose Procedures are wanted</param>
        /// <returns>List of Procedures</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<Procedure>> getProceduresFromUser(User user)
        {
            List<Procedure> resultTemporary = await context.Procedures
                .Where(x => x.ResponsiblePersonId == user.Id)
                .ToListAsync();
            List<Procedure> resultFinal = new List<Procedure>();
            foreach(Procedure t in resultTemporary){
                Procedure temp = await UpdateCompletionState(t);
                resultFinal.Add(temp);
            }
            return resultFinal;
        }
        /// <summary>
        /// Updates the attributes: completedTasks, openTasks, inprogressTasks and progressbar
        /// </summary>
        /// <param name="procedure">the Procedure that is updated</param>
        /// <returns>updated procedure</returns>
        /// <author>Florian Fendt</author>

        public async Task<Procedure> UpdateCompletionState(Procedure procedure)
        {   
            int completedTasks = 0;
            int openTasks = 0;
            int inprogressTasks = 0;
            double progressbar = 0;
            var taskContainer = new MakandraTaskContainer(context);
            var stateContainer = new MakandraTaskStateContainer(context);
            procedure.makandraTasks = await taskContainer.GetMakandraTasksFromProcedureId(procedure.Id);
            if(procedure.makandraTasks == null || procedure.makandraTasks.Count == 0){
                procedure.openTasks = 0;
                procedure.completedTasks = 0;
                procedure.inprogressTasks = 0;
                procedure.progressbar = 0;
                return procedure;
            }
            foreach(MakandraTask t in procedure.makandraTasks){
                t.State = await stateContainer.GetMakandraTaskStateFromId(t.StateId);
                MakandraTaskState state = t.State;
                if(state.Name.Equals("Erledigt"))
                {
                    completedTasks++;
                } else if(state.Name.Equals("In Bearbeitung")){
                    inprogressTasks++;
                } else {
                    openTasks++;
                }
            }
            procedure.openTasks = openTasks;
            procedure.completedTasks = completedTasks;
            procedure.inprogressTasks = inprogressTasks;
            procedure.progressbar = (double) completedTasks / (double) procedure.makandraTasks.Count;
            updateProcedure(procedure);
            return procedure;
        }
        /// <summary>
        /// Gets all Procedures that are based on the provided process
        /// </summary>
        /// <param name="id">Id of the process</param>
        /// <returns>List of Procedures</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<Procedure>> GetProceduresFromProcessId(int id){
            var Procedures = await context.Procedures
                .Where(x => x.basedProcessId == id)
                .ToListAsync();
            return Procedures;
        }
        /// <summary>
        /// Gets all Procedures that are based on the provided List of Processes
        /// </summary>
        /// <param name="Processes">List of Processes</param>
        /// <returns>List of Procedures</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<Procedure>> GetProceduresFromProcessess(List<MakandraProcess> Processes){
            List<Procedure> result = new List<Procedure>();
            foreach(var process in Processes){
                var temp = await GetProceduresFromProcessId(process.Id);
                result.Concat(temp).Distinct().ToList();
            }
            return result;
        }
        /// <summary>
        /// Imports a Json String that has the PRocedures for the start
        /// </summary>
        /// <param name="contractTypesContainer">Container required for IsValid function</param>
        /// <param name="makandraProcessContainer">Container required for IsValid function</param>
        /// <param name="userContainer">Container required for IsValid function</param>
        /// <param name="json">String that contains the Procedures to be added</param>
        /// <author>Florian Fendt</author>
        public async void Import(ContractTypesContainer contractTypesContainer, MakandraProcessContainer makandraProcessContainer, UserContainer userContainer, string json){
            if(json == null){
                return;
            }
            List<Procedure> procedureList = new List<Procedure>();
            try {
                procedureList = JsonSerializer.Deserialize<List<Procedure>>(json);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return;
            }
            foreach(Procedure t in procedureList){
                int result = t.IsValid(contractTypesContainer, makandraProcessContainer, userContainer);
                if(result != 0){
                    break;
                } else {
                    await addProcedure(t);
                }
            }
        }
    }
}
