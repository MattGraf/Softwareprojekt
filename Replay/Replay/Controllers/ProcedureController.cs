using Replay.Models;
using Replay.ViewModels;
using Replay.Container;
using Replay.Container.Account;
using Replay.Container.Account.MTM;
using Replay.Models.Account;
using Replay.Models.MTM;
using System.Text.Json;
using System.Linq;
using Replay.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;


namespace Replay.Controllers
{
    /// <summary>
    /// Controller that manages the Views of Procedure
    /// </summary>
    /// <author>Florian Fendt</author>
    public class ProcedureController : Controller
    {
        /// <summary>
        /// Containers that provides the managing of Procedure related database operations
        /// </summary>
        private ProcedureContainer _container;
        private UserContainer _userContainer;
        private MakandraTaskContainer _taskContainer;
        private MakandraTaskStateContainer  _taskStateContainer;
        private ContractTypesContainer _contractTypesContainer;
        private ProcedureDepartmentContainer _procedureDepartmentContainer;
        private DepartmentContainer _departmentContainer;
        private TaskTemplateContainer _taskTemplateContainer;
        private UserRolesContainer _userRoleContainer;
        private MakandraProcessRoleContainer _processRoleContainer;
        private MakandraProcessContainer _processContainer;
        private RoleContainer _roleContainer;
        private MakandraTaskRoleContainer _taskRoleContainer;
        private DuedateContainer _duedateContainer;
        /// <summary>
        /// Contructor of ProcedureController
        /// </summary>
        /// <param name="container">Manages database for Procedures</param>
        /// <param name="userContainer">Manages database for Users</param>
        /// <param name="taskContainer">Manages database for MakandraTasks</param>
        /// <param name="taskStateContainer">Manages database for MakandraTaskStates</param>
        /// <param name="contractTypesContainer">Manages database for ContractTypes</param>
        /// <param name="procedureDepartmentContainer">Manages database for ProcedureDepartments</param>
        /// <param name="departmentContainer">Manages database for Departments</param>
        /// <param name="taskTemplateContainer">Manages database for TaskTemplates</param>
        /// <param name="userRoleContainer">Manages database for UserRoles</param>
        /// <param name="processRoleContainer">Manages database for ProcessRoles</param>
        /// <param name="processContainer">Manages database for Processes</param>
        /// <param name="roleContainer">Manages database for Roles</param>
        /// <param name="taskRoleContainer">Manages database for TaskRoles</param>
        /// <param name="duedateContainer">Manages database for Duedates</param>
        /// <author>Florian Fendt</author>

        public ProcedureController(ProcedureContainer container, UserContainer userContainer, 
        MakandraTaskContainer taskContainer, MakandraTaskStateContainer taskStateContainer, 
        ContractTypesContainer contractTypesContainer, ProcedureDepartmentContainer procedureDepartmentContainer,
        DepartmentContainer departmentContainer, TaskTemplateContainer taskTemplateContainer, UserRolesContainer userRoleContainer,
        MakandraProcessRoleContainer processRoleContainer, MakandraProcessContainer processContainer, RoleContainer roleContainer, MakandraTaskRoleContainer taskRoleContainer, DuedateContainer duedateContainer)
        {
            _container = container;
            _userContainer = userContainer;
            _taskContainer = taskContainer;
            _taskStateContainer = taskStateContainer;
            _contractTypesContainer = contractTypesContainer;
            _procedureDepartmentContainer = procedureDepartmentContainer;
            _departmentContainer = departmentContainer;
            _taskTemplateContainer = taskTemplateContainer;
            _userRoleContainer = userRoleContainer;
            _processRoleContainer = processRoleContainer;
            _processContainer = processContainer;
            _duedateContainer = duedateContainer;
            _roleContainer = roleContainer;
            _taskRoleContainer = taskRoleContainer;
        }
        /// <summary>
        /// Provides an overview over all Procedures the logged in User is responsible for
        /// </summary>
        /// <returns>View to overview the Procedures</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Index()
        {

            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);
            List<Procedure> allprocedures = await _container.getAllProcedures();
            List<Procedure> Procedures = new List<Procedure>();
            Procedures = await _container.getProceduresFromUser(loggedInUser);
            ProcedureUserViewModel vm = new ProcedureUserViewModel(loggedInUser);
            foreach(Procedure t in Procedures)
            {
                if(t.Archived == false){
                    vm.addProcedure(t);
                }
            }
            return View(vm);

        }
        /// <summary>
        /// Provides an overview over all Procedures, only accessible by Administrators
        /// </summary>
        /// <returns>View to overview the Procedures</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> AdminIndex()
        {
            List<Procedure> allprocedures = await _container.getAllProcedures();
            ProcedureAdminIndexViewModel vm = new ProcedureAdminIndexViewModel();
            foreach(Procedure t in allprocedures)
            {
                if(t.Archived == false){
                    vm.addProcedure(t);
                }
            }
            return View(vm);

        }
        /// <summary>
        /// The HttpGet Method of the Edit Operation, it creates the edit view for the provided Procedure
        /// </summary>
        /// <param name="id">id of provided Procedure</param>
        /// <returns>View to edit the Procedure</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Procedure editProcedure = await _container.getProcedureFromId(id);
            List<ContractType> contractTypes = await _contractTypesContainer.GetContractTypes();
            List<Department> DepartmentList = await _departmentContainer.GetDepartments();
            List<ProcedureDepartment> ProcedureDepartments = await _procedureDepartmentContainer.GetProcedureDepartmentfromProcedureId(id);
            editProcedure.ProcedureDepartments = ProcedureDepartments;
            List<int> departments = new List<int>();
            foreach (var dep in editProcedure.ProcedureDepartments)
            {
                departments.Add(dep.DepartmentID);
            }
            var departmentoptions = new List<DepartmentViewModel>();
            foreach (var dp in DepartmentList)
            {
                bool selected = false;
                if(departments.Contains(dp.Id)){
                    selected = true;
                }
                var option = new DepartmentViewModel
                {
                    Name = dp.Name,
                    IsSelected = selected
                };
                departmentoptions.Add(option);
            }
            List<TaskTemplate> TaskTemplates = new List<TaskTemplate>();
            editProcedure.makandraTasks = await _taskContainer.GetMakandraTasksFromProcedureId(id);
            foreach(var t in editProcedure.makandraTasks){
                t.State = await _taskStateContainer.GetMakandraTaskStateFromId(t.StateId);
            }
            ProcedureEditViewModel vm = new ProcedureEditViewModel
            {
                Id = editProcedure.Id,
                Deadline = editProcedure.Deadline,
                name = editProcedure.name,
                EstablishingContractTypeId = editProcedure.EstablishingContractTypeId,
                ContractTypes = contractTypes,
                TargetDepartments = departmentoptions,
                ResponsiblePersonId = editProcedure.ResponsiblePersonId,
                ReferencePersonId = editProcedure.ReferencePersonId,
                PossibleResponsiblePersons = _userContainer.GetUsers().ToList(),
                PossibleReferencePersons = _userContainer.GetUsers().ToList(),
                TaskTemplates = await _taskTemplateContainer.GetAllTaskTemplates(),
                makandraTasks = editProcedure.makandraTasks,
                completedTasks = editProcedure.completedTasks,
                openTasks = editProcedure.openTasks,
                inprogressTasks = editProcedure.inprogressTasks
                
            };
            return View(vm);
        }
        /// <summary>
        /// HttpPost Method of the Edit Operation, changes all binded parameters
        /// </summary>
        /// <param name="model">The ProcedureEditViewModel that has the su´bmitted changes</param>
        /// <returns>returs to Index view if everything was valid, otherwise to the edit view again</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("")]
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id, name, Deadline, EstablishingContractTypeId, TargetDepartments, ResponsiblePersonId, ReferencePersonId, NewTasks, RemovedTasks")]ProcedureEditViewModel model)
        {
            var editprocedure = await _container.getProcedureFromId(model.Id);
            List<ProcedureDepartment> ProcedureDepartments = await _procedureDepartmentContainer.GetProcedureDepartmentfromProcedureId(model.Id);
            List<Department> DepartmentList = new List<Department>();
            List<int> departments = new List<int>();
            var departmentoptions = new List<DepartmentViewModel>();
            if (ModelState.IsValid)
            {
                try
                {
                    if (editprocedure == null)
                    {
                        return NotFound();
                    }
                    
                    editprocedure.name = model.name;
                    editprocedure.Deadline = model.Deadline;
                    editprocedure.ResponsiblePersonId = model.ResponsiblePersonId;
                    editprocedure.ReferencePersonId = model.ReferencePersonId;
                    editprocedure.EstablishingContractTypeId = model.EstablishingContractTypeId;
                    editprocedure.ProcedureDepartments = await _procedureDepartmentContainer.GetProcedureDepartmentfromProcedureId(model.Id);
                    for(int i = 0; i < editprocedure.ProcedureDepartments.Count; i++)
                    {
                        await _procedureDepartmentContainer.DeleteProcedureDepartment(editprocedure.ProcedureDepartments[i]);
                    }
                    
                    editprocedure.TargetDepartment = await model.GetDepartments(_departmentContainer);
                    if(editprocedure.TargetDepartment.Count == 0){
                        model.makandraTasks = editprocedure.makandraTasks;
                        model.ContractTypes =  await _contractTypesContainer.GetContractTypes();
                        model.PossibleResponsiblePersons = _userContainer.GetUsers().ToList();
                        model.PossibleReferencePersons =  _userContainer.GetUsers().ToList();
                        DepartmentList = await _departmentContainer.GetDepartments();
                            foreach (var dep in ProcedureDepartments)
                            {
                                departments.Add(dep.DepartmentID);
                            }
                            foreach (var dp in DepartmentList)
                            {
                                bool selected = false;
                                if(departments.Contains(dp.Id)){
                                    selected = true;
                                }
                                var option = new DepartmentViewModel
                                {
                                    Name = dp.Name,
                                    IsSelected = selected
                                };
                                departmentoptions.Add(option);
                            }
                        model.TargetDepartments = departmentoptions;
                        model.Departmenterror = true;
                        return View(model);
                    }
                    var newTasks = JsonSerializer.Deserialize<List<TaskDto>>(model.NewTasks);
                    if (!string.IsNullOrEmpty(model.NewTasks))
                    {
                        foreach (var task in newTasks)
                        {
                            var template = await _taskTemplateContainer.GetTaskTemplateFromId(task.TaskTemplateId);
                            MakandraTask tempTask =  new MakandraTask();
                            await tempTask.InitializeTask(editprocedure, template, _taskContainer, _taskStateContainer, _taskRoleContainer, _duedateContainer, _container, _roleContainer, _userContainer);
                        }
                    }
                    var removedTasks = JsonSerializer.Deserialize<List<RemovedTaskDto>>(model.RemovedTasks);
                    if (!string.IsNullOrEmpty(model.RemovedTasks))
                    {
                        foreach (var task in removedTasks)
                        {
                            MakandraTask tempTask;
                            tempTask = await _taskContainer.GetMakandraTaskFromId((int)task.Id);
                            await _taskContainer.DeleteMakandraTask(tempTask);
                        }
                    }
                    await _container.updateProcedure(editprocedure);
                    foreach (var department in editprocedure.TargetDepartment)
                    {
                            ProcedureDepartment temp = new ProcedureDepartment
                            {
                                ProcedureID = editprocedure.Id,
                                Procedure = await _container.getProcedureFromId(editprocedure.Id),
                                DepartmentID = department.Id,
                                Department = await _departmentContainer.GetDepartmentFromId(department.Id)

                            };
                            await _procedureDepartmentContainer.AddProcedureDepartment(temp);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError(string.Empty, "Datenbank konnte nicht erfolgreich aktualisiert werden");
                }
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return RedirectToAction("Index");
            }
                model.makandraTasks = editprocedure.makandraTasks;
                model.ContractTypes =  await _contractTypesContainer.GetContractTypes();
                model.PossibleResponsiblePersons = _userContainer.GetUsers().ToList();
                model.PossibleReferencePersons =  _userContainer.GetUsers().ToList();
                DepartmentList = await _departmentContainer.GetDepartments();
                    foreach (var dep in ProcedureDepartments)
                    {
                        departments.Add(dep.DepartmentID);
                    }
                    foreach (var dp in DepartmentList)
                    {
                        bool selected = false;
                        if(departments.Contains(dp.Id)){
                            selected = true;
                        }
                        var option = new DepartmentViewModel
                        {
                            Name = dp.Name,
                            IsSelected = selected
                        };
                        departmentoptions.Add(option);
                    }
                model.TargetDepartments = departmentoptions;
                return View(model);
        }
        /// <summary>
        /// Provides the DetailsView of the specified Procedure
        /// </summary>
        /// <param name="id">the Id of the specified Procedure</param>
        /// <returns>Detail View</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Procedure detailsProcedure = await _container.getProcedureFromId(id);
            detailsProcedure.EstablishingContractType = await _contractTypesContainer.GetContractTypeFromID(detailsProcedure.EstablishingContractTypeId);
            if (detailsProcedure.EstablishingContractType == null)
            {
                throw new ArgumentNullException(nameof(detailsProcedure.EstablishingContractType), "EstablishingContractType is null");
            }
            List<Department> departments = new List<Department>();
            List<ProcedureDepartment> ProcedureDepartments = await _procedureDepartmentContainer.GetProcedureDepartmentfromProcedureId(id);
            detailsProcedure.ProcedureDepartments = ProcedureDepartments;
            foreach(var department in detailsProcedure.ProcedureDepartments){
                var temp = await _departmentContainer.GetDepartmentFromId(department.DepartmentID);
                departments.Add(temp);
            }
            List<MakandraTask> Tasks = new List<MakandraTask>();
            detailsProcedure.makandraTasks = await _taskContainer.GetMakandraTasksFromProcedureId(id);
            foreach(var task in detailsProcedure.makandraTasks){
                task.State = await _taskStateContainer.GetMakandraTaskStateFromId(task.StateId);
            }
            ProcedureDetailsViewModel vm = new ProcedureDetailsViewModel
            {
                Id = detailsProcedure.Id,
                Deadline = detailsProcedure.Deadline,
                name = detailsProcedure.name,
                EstablishingContractType = await _contractTypesContainer.GetContractTypeFromID(detailsProcedure.EstablishingContractTypeId),
                TargetDepartment = departments,
                ReferencePerson = await _userContainer.GetUserFromId(detailsProcedure.ReferencePersonId),
                ResponsiblePerson = await _userContainer.GetUserFromId(detailsProcedure.ResponsiblePersonId),
                makandraTasks = detailsProcedure.makandraTasks,
                basedProcess = await _processContainer.GetProcessFromId(detailsProcedure.basedProcessId),
                completedTasks = detailsProcedure.completedTasks,
                openTasks = detailsProcedure.openTasks,
                inprogressTasks = detailsProcedure.inprogressTasks
            };
            return View(vm);
        }
        /// <summary>
        /// Provides an overview over all archived Procedures the logged in user has access to
        /// </summary>
        /// <returns>View to overview the Procedures</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("")]
        public async Task<IActionResult> ProcedureArchive()
        {
            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);
            IEnumerable<Role> userRoles = await _userRoleContainer.GetRolesFromUser(loggedInUser);
            userRoles = userRoles.ToList();
            List<MakandraProcess> userProcesses = new List<MakandraProcess>();
            foreach(var role in userRoles){
                var temp = await _processRoleContainer.GetProcessFromRoles(role);
                userProcesses.Concat(temp).Distinct().ToList();
            }
            List<Procedure> ProceduresUser = await _container.getProceduresFromUser(loggedInUser);
            List<Procedure> ProceduresRole = await _container.GetProceduresFromProcessess(userProcesses);
            ProceduresUser.Concat(ProceduresRole).Distinct().ToList();
            ProcedureArchiveViewModel vm = new ProcedureArchiveViewModel(loggedInUser);
            foreach(Procedure t in ProceduresUser)
            {
                if(t.Archived == true){
                    vm.addProcedure(t);
                }
            }
            return View(vm);
        }
        /// <summary>
        /// Provides an overview over all archived Procedures
        /// </summary>
        /// <returns>View to overview the Procedures</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> AdminArchive()
        {
            List<Procedure> allprocedures = await _container.getAllProcedures();
            ProcedureAdminIndexViewModel vm = new ProcedureAdminIndexViewModel();
            foreach(Procedure t in allprocedures)
            {
                if(t.Archived == true){
                    vm.addProcedure(t);
                }
            }
            return View(vm);

        }
        /// <summary>
        /// Archives the provided Procedure and all Tasks it includes
        /// </summary>
        /// <param name="id">Id of the Procedure that shall be archived</param>
        /// <returns>view index</returns>
        /// <author>Florian Fendt</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Archive(int id)
        {
            if(id == null){
                return NotFound();
            }
            Procedure proceduretoarchive = await _container.getProcedureFromId(id);
            proceduretoarchive.Archived = true;
            proceduretoarchive.makandraTasks = await _taskContainer.GetMakandraTasksFromProcedureId(id);
            foreach(var task in proceduretoarchive.makandraTasks){
                task.Archived = true;
                await _taskContainer.UpdateMakandraTask(task);
            }
            await _container.updateProcedure(proceduretoarchive);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Restores the archived Procedure and all its tasks
        /// </summary>
        /// <param name="id">id of the Procedure that shall be restored</param>
        /// <returns>view Procedurearchive</returns>
        /// <author>Florian Fendt</author>
        public async Task<IActionResult> Restore(int id)
        {
            if(id == null){
                return NotFound();
            }
            Procedure proceduretorestore = await _container.getProcedureFromId(id);
            proceduretorestore.Archived = false;
            proceduretorestore.makandraTasks = await _taskContainer.GetMakandraTasksFromProcedureId(id);
            foreach(var task in proceduretorestore.makandraTasks){
                task.Archived = false;
                await _taskContainer.UpdateMakandraTask(task);
            }
            await _container.updateProcedure(proceduretorestore);
            return RedirectToAction("ProcedureArchive");
        }
    }
    /// <summary>
    /// Class that is needed for the Deserializing of the Json string newTasks provided by the Edit view
    /// </summary>
    /// <author>Florian Fendt</author>
    public class TaskDto
    {
    public long Id { get; set; }
    public int TaskTemplateId { get; set; }

    public string TaskTemplateName {get; set; }
    }
    /// <summary>
    /// Class that is needed for the Deserializing of the Json string removedTasks provided by the Edit view
    /// </summary>
    /// <author>Florian Fendt</author>
     public class RemovedTaskDto
    {
    public long Id { get; set; }
    }
}
