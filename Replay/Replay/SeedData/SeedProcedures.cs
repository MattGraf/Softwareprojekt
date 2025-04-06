using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;
using Replay.Models.MTM;
using Replay.Models.Account;
using Replay.Container;
using System.Text.Json;

namespace Replay.SeedData
{
    public class SeedProcedures
    {
        private static DepartmentContainer _departmentContainer;
        private static ProcedureContainer _procedureContainer;
        private static ProcedureDepartmentContainer _procedureDepartmentContainer;
        private static ContractTypesContainer _contractTypesContainer;
        private static UserContainer _userContainer;
        private static MakandraProcessContainer _makandraProcessContainer;

        public static async void InitializeProcedures(IServiceProvider serviceProvider)
        {        
            using(var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if(db.Procedures.Any())
                {
                    return;
                }

                _departmentContainer = serviceProvider.GetRequiredService<DepartmentContainer>();
                _procedureContainer = serviceProvider.GetRequiredService<ProcedureContainer>();
                _procedureDepartmentContainer = serviceProvider.GetRequiredService<ProcedureDepartmentContainer>();
                _contractTypesContainer = serviceProvider.GetRequiredService<ContractTypesContainer>();
                _userContainer = serviceProvider.GetRequiredService<UserContainer>();
                _makandraProcessContainer = serviceProvider.GetRequiredService<MakandraProcessContainer>();

                CreateProcedures();
                InitializeProcedureDepartments();

            }
            
        }
        public static async void CreateProcedures() {
            int koenig = _userContainer.GetUserFromEmail("koenig@replay.de").Result.Id;
            int bill = _userContainer.GetUserFromEmail("bill@replay.de").Result.Id;
            int karl = _userContainer.GetUserFromEmail("karl@replay.de").Result.Id;
            int gerhard = _userContainer.GetUserFromEmail("gerhard@replay.de").Result.Id;
            int wilma = _userContainer.GetUserFromEmail("wilma@replay.de").Result.Id;
            DateTime date1 = new DateTime(2024, 9, 13);
            DateTime date2 = new DateTime(2024, 7, 27);
            DateTime date3 = new DateTime(2024, 10, 13);
            Procedure procedure1 = new Procedure {
                name = "Neuzugang HR",
                ResponsiblePersonId = koenig,
                ReferencePersonId = bill,
                Deadline = date1,
                EstablishingContractTypeId = 4,
                basedProcessId = 1,
                Archived = false,
                completedTasks = 0,
                openTasks = 0,
                inprogressTasks = 0,
                progressbar = 0
            };
            Procedure procedure2 = new Procedure {
                name = "Neuzugang IT",
                ResponsiblePersonId = gerhard,
                ReferencePersonId = karl,
                Deadline = date2,
                EstablishingContractTypeId = 1,
                basedProcessId = 1,
                Archived = false,
                completedTasks = 0,
                openTasks = 0,
                inprogressTasks = 0,
                progressbar = 0
            };
            Procedure procedure3 = new Procedure {
                name = "Neuzugang IT/Trainee",
                ResponsiblePersonId = bill,
                ReferencePersonId = wilma,
                Deadline = date3,
                EstablishingContractTypeId = 4,
                basedProcessId = 1,
                Archived = false,
                completedTasks = 0,
                openTasks = 0,
                inprogressTasks = 0,
                progressbar = 0
            };
            List<Procedure> Procedurelist= new List<Procedure>();
            Procedurelist.Add(procedure1);
            Procedurelist.Add(procedure2);
            Procedurelist.Add(procedure3);
            string json = JsonSerializer.Serialize(Procedurelist);

            _procedureContainer.Import(_contractTypesContainer, _makandraProcessContainer, _userContainer, json);

        }
        public static async void InitializeProcedureDepartments() 
        {
            ProcedureDepartment procedureDepartment1 = new ProcedureDepartment {
                ProcedureID = 1,
                DepartmentID = 7
            };
            ProcedureDepartment procedureDepartment2 = new ProcedureDepartment {
                ProcedureID = 2,
                DepartmentID = 1
            };
            ProcedureDepartment procedureDepartment3 = new ProcedureDepartment {
                ProcedureID = 3,
                DepartmentID = 1
            };

            List<ProcedureDepartment> proceduredepartmentslist = new List<ProcedureDepartment>();
            proceduredepartmentslist.Add(procedureDepartment1);
            proceduredepartmentslist.Add(procedureDepartment2);
            proceduredepartmentslist.Add(procedureDepartment3);
            string json = JsonSerializer.Serialize(proceduredepartmentslist);

            _procedureDepartmentContainer.Import(_procedureContainer, _departmentContainer, json);
        }
    }
}



             
