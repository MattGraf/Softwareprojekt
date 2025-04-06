using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Container.Account.MTM;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.ViewModels.TaskTemplateViewModels
{
    /// <summary>
    /// ViewModel for standard-overview of <see cref="TaskTemplate"/> of the <see cref="Models.Account.User"/> who wants to see his list
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateUserIndexViewModel
    {
        public User? User {get; set;}
        public List<TaskTemplateViewModel> TaskTemplateViewModelList {get; set;} = new List<TaskTemplateViewModel>();
        public int SortId {get; set;}
        public int NewSortId {get; set;}
        public int ProcessId {get; set;}
        public int NewProcessId {get; set;}
        public bool Filter {get; set;}
        public List<TaskTemplateMakandraProcessViewModel> ProcessList {get; set;} = new List<TaskTemplateMakandraProcessViewModel>();
        public int ArchiveId {get; set;}
        public int DetailsId {get; set;}
        public int EditId {get; set;}


        /// <summary>
        /// Initialize the filter-bar with the processes of which the user has EditAccess
        /// </summary>
        /// <param name="user">Logged user</param>
        /// <param name="userRolesContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessRoleContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateRoleContainer">Container for the connection to the database</param>
        /// <author>Matthias Grafberger</author>
        public void SetTaskTemplateUserIndexViewModelListWithUser(User user, UserRolesContainer userRolesContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, MakandraProcessContainer makandraProcessContainer, TaskTemplateContainer taskTemplateContainer, TaskTemplateRoleContainer taskTemplateRoleContainer) {
            this.User = user;

            //Processes

            List<Role> roles = userRolesContainer.GetRolesFromUser(user).Result.ToList();

            List<int> makandraProcessIds = new List<int>();

            foreach (Role role in roles) {
                foreach(int id in makandraProcessRoleContainer.GetAssociatedMakandraProcessIdsFromRoleId(role.Id).Result) {
                    if (!makandraProcessIds.Contains(id)) makandraProcessIds.Add(id);
                }
            }

            List<MakandraProcess> makandraProcesses = new List<MakandraProcess>();

            foreach (int id in makandraProcessIds) {
                MakandraProcess makandraProcess = makandraProcessContainer.GetProcessFromId(id).Result;
                if (!makandraProcesses.Contains(makandraProcess)) makandraProcesses.Add(makandraProcess);
            }

            foreach (MakandraProcess makandraProcess in makandraProcesses) {
                ProcessList.Add(new TaskTemplateMakandraProcessViewModel{
                    ID = makandraProcess.Id,
                    Name = makandraProcess.Name
                });
            }

            ProcessList = ProcessList.OrderBy(e => e.Name).ToList();
    
            ProcessList.Insert(0, new TaskTemplateMakandraProcessViewModel {
                ID = 0,
                Name = "Kein Filter"
            });

            FilterTaskTemplateViewModelList(taskTemplateContainer, makandraProcessContainer, userRolesContainer, taskTemplateRoleContainer);
        }

        /// <summary>
        /// Filters the database of the <see cref="TaskTemplate"/> after <see cref="TaskTemplate"/>s from a specific <see cref="MakandraProcess"/> which is given in the processId-attribute
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="userRolesContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateRoleContainer">Container for the connection to the database</param>
        /// <author>Matthias Grafberger</author>
        public void FilterTaskTemplateViewModelList(TaskTemplateContainer taskTemplateContainer, MakandraProcessContainer makandraProcessContainer, UserRolesContainer userRolesContainer, TaskTemplateRoleContainer taskTemplateRoleContainer) {
            TaskTemplateViewModelList = new List<TaskTemplateViewModel>();

            List<TaskTemplate> taskTemplates = new List<TaskTemplate>();

            if (ProcessId == 0) {
                //taskTemplates = taskTemplateContainer.GetTaskTemplatesNotArchived().Result;

                List<Role> roles = userRolesContainer.GetRolesFromUser(this.User).Result.ToList();   

                List<int> taskTemplateIds = new List<int>();

                foreach (Role role in roles) {
                    foreach(int id in taskTemplateRoleContainer.GetAssociatedTaskTemplateIdsFromRoleId(role.Id).Result) {
                        if (!taskTemplateIds.Contains(id)) taskTemplateIds.Add(id);
                    }
                }

                foreach (int id in taskTemplateIds) {
                    TaskTemplate taskTemplate = taskTemplateContainer.GetTaskTemplateFromId(id).Result;
                    if (!taskTemplates.Contains(taskTemplate) && !taskTemplate.Archived) taskTemplates.Add(taskTemplate);
                }
            } else {
                taskTemplates = taskTemplateContainer.GetTaskTemplatesNotArchivedWithProcessId(ProcessId).Result;
            }

            foreach (TaskTemplate taskTemplate in taskTemplates) {
                TaskTemplateViewModelList.Add(new TaskTemplateViewModel {
                        ID = taskTemplate.ID,
                        Name = taskTemplate.Name,
                        ProcessName = makandraProcessContainer.GetProcessFromId(taskTemplate.MakandraProcessId).Result.Name
                });
            }
            SortTaskTemplateViewModelList();
        }

        /// <summary>
        /// Sort the list of not-archived <see cref="TaskTemplate"/>s  new after the search-attribute
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public void SortTaskTemplateViewModelList() {
            if (SortId == 1) {
                TaskTemplateViewModelList = TaskTemplateViewModelList
                    .OrderBy(t => t.ProcessName)
                    .ThenBy(t => t.Name)
                    .ToList();
            } else {
                TaskTemplateViewModelList = TaskTemplateViewModelList
                    .OrderBy(t => t.Name)
                    .ThenBy(t => t.ProcessName)
                    .ToList();
            }
        }
    }
}