
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;


using Replay.Data;
using Replay.Models;
using Replay.Models.MTM;
using Replay.Models.Account;
using Replay.Container;

using Moq;
using Moq.EntityFrameworkCore;
using System.Diagnostics;

using Xunit;

namespace Replay.UnitTests
{
    /// <summary>
    /// UnitTests for Procedure
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class UnitTestProcedure
    {

        /// <summary>
        /// Tests the start of a procedure
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void StartProcedure()
        {

            var mockContext = new Mock<MakandraContext>();

            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var localMakandraTaskList = new List<MakandraTask>();
            var mockMakandraTaskList = new List<MakandraTask>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(localMakandraTaskList);
            mockContext.Setup(c => c.Tasks.Add(It.IsAny<MakandraTask>())).Callback<MakandraTask>((s) => mockMakandraTaskList.Add(s));

            var localMakandraTaskStateList = new List<MakandraTaskState>();
            var mockMakandraTaskStateList = new List<MakandraTaskState>();
            mockContext.Setup(c => c.States).ReturnsDbSet(localMakandraTaskStateList);
            mockContext.Setup(c => c.States.Add(It.IsAny<MakandraTaskState>())).Callback<MakandraTaskState>((s) => mockMakandraTaskStateList.Add(s));

            var localMakandraTaskRoleList = new List<MakandraTaskRole>();
            var mockMakandraTaskRoleList = new List<MakandraTaskRole>();
            mockContext.Setup(c => c.TaskRoles).ReturnsDbSet(localMakandraTaskRoleList);
            mockContext.Setup(c => c.TaskRoles.Add(It.IsAny<MakandraTaskRole>())).Callback<MakandraTaskRole>((s) => mockMakandraTaskRoleList.Add(s));

            var localDuedateList = new List<Duedate>();
            var mockDuedateList = new List<Duedate>();
            mockContext.Setup(c => c.Duedates).ReturnsDbSet(localDuedateList);
            mockContext.Setup(c => c.Duedates.Add(It.IsAny<Duedate>())).Callback<Duedate>((s) => mockDuedateList.Add(s));

            var localProcedureList = new List<Procedure>();
            var mockProcedureList = new List<Procedure>();
            mockContext.Setup(c => c.Procedures).ReturnsDbSet(localProcedureList);
            mockContext.Setup(c => c.Procedures.Add(It.IsAny<Procedure>())).Callback<Procedure>(
                (s) => {
                    mockProcedureList.Add(s);
                    s.Id = 1;
                }
            );

            var localRoleList = new List<Role>();
            var mockRoleList = new List<Role>();
            mockContext.Setup(c => c.Roles).ReturnsDbSet(localRoleList);
            mockContext.Setup(c => c.Roles.Add(It.IsAny<Role>())).Callback<Role>((s) => mockRoleList.Add(s));

            var localUserList = new List<User>();
            var mockUserList = new List<User>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(localUserList);
            mockContext.Setup(c => c.Users.Add(It.IsAny<User>())).Callback<User>((s) => mockUserList.Add(s));

            var localTaskTemplateDepartmentList = new List<TaskTemplateDepartment>();
            var mockTaskTemplateDepartmentList = new List<TaskTemplateDepartment>();
            mockContext.Setup(c => c.TaskTemplateDepartments).ReturnsDbSet(localTaskTemplateDepartmentList);
            mockContext.Setup(c => c.TaskTemplateDepartments.Add(It.IsAny<TaskTemplateDepartment>())).Callback<TaskTemplateDepartment>((s) => mockTaskTemplateDepartmentList.Add(s));

            var localTaskTemplateContractTypeList = new List<TaskTemplateContractType>();
            var mockTaskTemplateContractTypeList = new List<TaskTemplateContractType>();
            mockContext.Setup(c => c.TaskTemplateContractTypes).ReturnsDbSet(localTaskTemplateContractTypeList);
            mockContext.Setup(c => c.TaskTemplateContractTypes.Add(It.IsAny<TaskTemplateContractType>())).Callback<TaskTemplateContractType>((s) => mockTaskTemplateContractTypeList.Add(s));
        
            var localContractTypeList = new List<ContractType>();
            var mockContractTypeList = new List<ContractType>();
            mockContext.Setup(c => c.ContractTypes).ReturnsDbSet(localContractTypeList);
            mockContext.Setup(c => c.ContractTypes.Add(It.IsAny<ContractType>())).Callback<ContractType>((s) => mockContractTypeList.Add(s));

            var localDepartmentList = new List<Department>();
            var mockDepartmentList = new List<Department>();
            mockContext.Setup(c => c.Departments).ReturnsDbSet(localDepartmentList);
            mockContext.Setup(c => c.Departments.Add(It.IsAny<Department>())).Callback<Department>((s) => mockDepartmentList.Add(s));

            var localTaskTemplateList = new List<TaskTemplate>();
            var mockTaskTemplateList = new List<TaskTemplate>();
            mockContext.Setup(c => c.TaskTemplates).ReturnsDbSet(localTaskTemplateList);
            mockContext.Setup(c => c.TaskTemplates.Add(It.IsAny<TaskTemplate>())).Callback<TaskTemplate>((s) => mockTaskTemplateList.Add(s));

            var localProcedureDepartmentList = new List<ProcedureDepartment>();
            var mockProcedureDepartmentList = new List<ProcedureDepartment>();
            mockContext.Setup(c => c.ProcedureDepartments).ReturnsDbSet(localProcedureDepartmentList);
            mockContext.Setup(c => c.ProcedureDepartments.Add(It.IsAny<ProcedureDepartment>())).Callback<ProcedureDepartment>((s) => mockProcedureDepartmentList.Add(s)); 

            MakandraTaskContainer taskContainer = new MakandraTaskContainer(mockContext.Object);
            MakandraTaskStateContainer taskStateContainer = new MakandraTaskStateContainer(mockContext.Object);
            MakandraTaskRoleContainer taskRoles = new MakandraTaskRoleContainer(mockContext.Object);
            DuedateContainer duedates = new DuedateContainer(mockContext.Object);
            ProcedureContainer procedures = new ProcedureContainer(mockContext.Object);
            RoleContainer roleContainer = new RoleContainer(mockContext.Object);
            UserContainer userContainer = new UserContainer(mockContext.Object);
            TaskTemplateDepartmentContainer taskTemplateDepartmentContainer = new TaskTemplateDepartmentContainer(mockContext.Object);
            TaskTemplateContractTypeContainer taskTemplateContractTypeContainer = new TaskTemplateContractTypeContainer(mockContext.Object);
            ProcedureDepartmentContainer procedureDepartmentContainer = new ProcedureDepartmentContainer(mockContext.Object);
            DepartmentContainer departmentContainer = new DepartmentContainer(mockContext.Object);

            User responsible = new User{
                Id = 1,
                Email = "test@makandra.de",
                FullName = "Test Test",
                Password = "Test",
                Active = true
            };

            User reference = new User {
                Id = 2,
                Email = "test2@makandra.de",
                FullName = "Test Test 2",
                Password = "Test2",
                Active = true
            };

            localUserList.Add(responsible);
            localUserList.Add(reference);

            string name = "Test Procedure";

            ContractType contractType = new ContractType {
                ID = 1,
                Name = "Trainee"
            };

            localContractTypeList.Add(contractType);

            Department department1 = new Department {
                Id = 1,
                Name = "Entwicklung"
            };

            Department department2 = new Department {
                Id = 2,
                Name = "Operations"
            };

            localDepartmentList.Add(department1);
            localDepartmentList.Add(department2);

            MakandraProcess makandraProcess = new MakandraProcess {
                Id = 1,
                Name = "TestProcess"
            };

            TaskTemplate taskTemplate1 = new TaskTemplate {
                ID = 1,
                Name = "TaskTemplateTest1",
                DuedateID = 1,
                DefaultResponsible = "Vorgangsverantwortlicher",
                Archived = false,
                MakandraProcessId = 1
            };

            TaskTemplate taskTemplate2 = new TaskTemplate {
                ID = 1,
                Name = "TaskTemplateTest2",
                Instruction = "TestInstruction",
                DuedateID = 2,
                DefaultResponsible = "Bezugsperson",
                Archived = false,
                MakandraProcessId = 1
            };

            TaskTemplate taskTemplate3 = new TaskTemplate {
                ID = 1,
                Name = "TaskTemplateTest3",
                DuedateID = 3,
                DefaultResponsible = "Administrator",
                Archived = false,
                MakandraProcessId = 1
            };

            TaskTemplate taskTemplate4 = new TaskTemplate {
                ID = 1,
                Name = "TaskTemplateTest4",
                DuedateID = 1,
                DefaultResponsible = "Administrator",
                Archived = true,
                MakandraProcessId = 1
            };

            localTaskTemplateList.Add(taskTemplate1);
            localTaskTemplateList.Add(taskTemplate2);
            localTaskTemplateList.Add(taskTemplate3);
            localTaskTemplateList.Add(taskTemplate4);

            makandraProcess.Tasks = new List<TaskTemplate>();
            makandraProcess.Tasks.Add(taskTemplate1);
            makandraProcess.Tasks.Add(taskTemplate2);
            makandraProcess.Tasks.Add(taskTemplate3);
            makandraProcess.Tasks.Add(taskTemplate4);

            Duedate duedate1 = new Duedate {
               ID = 1,
                Name = "ASAP",
                Days = 0
            };

            Duedate duedate2 = new Duedate {
                ID = 2,
                Name = "Am ersten Arbeitstag",
                Days = 0
            };

            Duedate duedate3 = new Duedate {
                ID = 3,
                Name = "2 Monate vor Antritt",
                Days = -60
            };

            localDuedateList.Add(duedate1);
            localDuedateList.Add(duedate2);
            localDuedateList.Add(duedate3);

            Role role1 = new Role{
                Id = 1,
                Name = "Administrator"
            };

            Role role2 = new Role{
                Id = 2,
                Name = "IT"
            };

            localRoleList.Add(role1);
            localRoleList.Add(role2);

            TaskTemplateContractType taskTemplateContractType1 = new TaskTemplateContractType {
                ContractTypeID = 1,
                TaskTemplateID = 1
            };

            TaskTemplateContractType taskTemplateContractType2 = new TaskTemplateContractType {
                ContractTypeID = 1,
                TaskTemplateID = 2
            };

            TaskTemplateContractType taskTemplateContractType3 = new TaskTemplateContractType {
                ContractTypeID = 1,
                TaskTemplateID = 3
            };

            TaskTemplateContractType taskTemplateContractType4 = new TaskTemplateContractType {
                ContractTypeID = 1,
                TaskTemplateID = 4
            };

            localTaskTemplateContractTypeList.Add(taskTemplateContractType1);
            localTaskTemplateContractTypeList.Add(taskTemplateContractType2);
            localTaskTemplateContractTypeList.Add(taskTemplateContractType3);
            localTaskTemplateContractTypeList.Add(taskTemplateContractType4);

            TaskTemplateDepartment taskTemplateDepartment1 = new TaskTemplateDepartment {
                TaskTemplateID = 1,
                DepartmentID = 1
            };
             TaskTemplateDepartment taskTemplateDepartment2 = new TaskTemplateDepartment {
                TaskTemplateID = 1,
                DepartmentID = 2
            };
            TaskTemplateDepartment taskTemplateDepartment3 = new TaskTemplateDepartment {
                TaskTemplateID = 2,
                DepartmentID = 1
            };
            TaskTemplateDepartment taskTemplateDepartment4 = new TaskTemplateDepartment {
                TaskTemplateID = 3,
                DepartmentID = 2
            };

            DateTime dateTime = new DateTime(2024, 10, 1);
            DateTime dateTimeDuedate3 = dateTime.AddDays(-60);

            localTaskTemplateDepartmentList.Add(taskTemplateDepartment1);
            localTaskTemplateDepartmentList.Add(taskTemplateDepartment2);
            localTaskTemplateDepartmentList.Add(taskTemplateDepartment3);
            localTaskTemplateDepartmentList.Add(taskTemplateDepartment4);

            List<Department> targetDepartments = new List<Department>();

            targetDepartments.Add(department1);
            targetDepartments.Add(department2);

            MakandraTaskState makandraTaskState = new MakandraTaskState {
                Id = 1,
                Name = "Offen"
            };

            localMakandraTaskStateList.Add(makandraTaskState);

            Procedure procedure1 = new Procedure {
                Id = 1,
                Deadline = dateTime,
                name = "Test Procedure",
                EstablishingContractType = contractType,
                EstablishingContractTypeId = contractType.ID,
                TargetDepartment = targetDepartments,
                ReferencePersonId = reference.Id,
                ResponsiblePersonId = responsible.Id,
                basedProcessId = makandraProcess.Id,
                Archived = false
            };

            localProcedureList.Add(procedure1);

            Procedure procedure = Procedure.CreateProcedure(makandraProcess, dateTime, reference, responsible, name, contractType, targetDepartments, taskContainer, taskStateContainer, taskRoles, duedates, procedures, roleContainer, userContainer, taskTemplateDepartmentContainer, taskTemplateContractTypeContainer, procedureDepartmentContainer, departmentContainer).Result;

            Assert.Equal("Test Procedure", procedure.name);
            Assert.Equal(dateTime, procedure.Deadline);
            Assert.Equal(contractType, procedure.EstablishingContractType);
            Assert.Equal(contractType.ID, procedure.EstablishingContractTypeId);
            Assert.Equal(targetDepartments.Count, procedure.TargetDepartment.Count);
            foreach(Department department in targetDepartments) {
                bool departmentExists = false;
                foreach (var dept in procedure.TargetDepartment)
                {
                    if (dept.Id == department.Id)
                    {
                        departmentExists = true;
                        break;
                    }
                }
                Assert.True(departmentExists);
            }
            Assert.Equal(reference.Id, procedure.ReferencePersonId);
            Assert.Equal(responsible.Id, procedure.ResponsiblePersonId);
            Assert.Equal(makandraProcess.Id, procedure.basedProcessId);
            Assert.False(procedure.Archived);
            Assert.Equal(0, procedure.completedTasks);
            Assert.Equal(0, procedure.openTasks);
            Assert.Equal(0, procedure.inprogressTasks);
            Assert.Equal(0.0, procedure.progressbar);
            Assert.True(procedure.makandraTasks.Count == makandraProcess.Tasks.Count - 1);

            //1. MakandraTask
            MakandraTask firstTask = mockMakandraTaskList[0];

            Assert.Equal("TaskTemplateTest1", firstTask.Name);
            Assert.Null(firstTask.Instruction);
            Assert.Null(firstTask.ResponsibleRole);
            Assert.Null(firstTask.ResponsibleRoleId);
            Assert.Equal(DateTime.Today, firstTask.TargetDate);
            Assert.Null(firstTask.Notes);
            Assert.Equal("Offen", firstTask.State.Name);
            Assert.Equal(1, firstTask.StateId);
            Assert.Equal(1, firstTask.ProcedureId);
            Assert.Equal(procedure.ResponsiblePersonId, firstTask.AssigneeId);
            Assert.False(firstTask.Archived);

            //2. MakandraTask
            MakandraTask secondTask = mockMakandraTaskList[1];

            Assert.Equal("TaskTemplateTest2", secondTask.Name);
            Assert.Equal( "TestInstruction", secondTask.Instruction);
            Assert.Null(secondTask.ResponsibleRole);
            Assert.Null(secondTask.ResponsibleRoleId);
            Assert.Equal(dateTime, secondTask.TargetDate);
            Assert.Null(secondTask.Notes);
            Assert.Equal("Offen", secondTask.State.Name);
            Assert.Equal(1, secondTask.StateId);
            Assert.Equal(1, secondTask.ProcedureId);
            Assert.Equal(procedure.ReferencePersonId, secondTask.AssigneeId);
            Assert.False(secondTask.Archived);

            //3. MakandraTask

            MakandraTask thirdTask = mockMakandraTaskList[2];
            
            Assert.Equal("TaskTemplateTest3", thirdTask.Name);
            Assert.Null(thirdTask.Instruction);
            Assert.Equal(1, thirdTask.ResponsibleRoleId);
            Assert.Equal(dateTimeDuedate3, thirdTask.TargetDate);
            Assert.Null(thirdTask.Notes);
            Assert.Equal("Offen", thirdTask.State.Name);
            Assert.Equal(1, thirdTask.StateId);
            Assert.Equal(1, thirdTask.ProcedureId);
            Assert.Null(thirdTask.AssigneeId);
            Assert.Null(thirdTask.Assignee);
            Assert.False(thirdTask.Archived);

        }

        /// <summary>
        /// Tests the imports of a procedure
        /// </summary>
        /// <author>Matthias Grafberger</author>
        [Fact]
        public void Procedure_Import()
        {
            var mockContextImport = new Mock<MakandraContext>();

            mockContextImport.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            int id = 0;
            var localProcedureListImport = new List<Procedure>();
            var mockProcedureListImport = new List<Procedure>();
            mockContextImport.Setup(c => c.Procedures).ReturnsDbSet(localProcedureListImport);
            mockContextImport.Setup(c => c.Procedures.Add(It.IsAny<Procedure>())).Callback<Procedure>(
                (s) => {
                    mockProcedureListImport.Add(s);
                    s.Id = ++id;
                }
            );

            var localContractTypeListImport = new List<ContractType>();
            var mockContractTypeListImport = new List<ContractType>();
            mockContextImport.Setup(c => c.ContractTypes).ReturnsDbSet(localContractTypeListImport);
            mockContextImport.Setup(c => c.ContractTypes.Add(It.IsAny<ContractType>())).Callback<ContractType>((s) => mockContractTypeListImport.Add(s));

            var localMakandraProcessListImport = new List<MakandraProcess>();
            var mockMakandraProcessListImport = new List<MakandraProcess>();
            mockContextImport.Setup(c => c.Processes).ReturnsDbSet(localMakandraProcessListImport);
            mockContextImport.Setup(c => c.Processes.Add(It.IsAny<MakandraProcess>())).Callback<MakandraProcess>((s) => mockMakandraProcessListImport.Add(s));

            var localUserListImport = new List<User>();
            var mockUserListImport = new List<User>();
            mockContextImport.Setup(c => c.Users).ReturnsDbSet(localUserListImport);
            mockContextImport.Setup(c => c.Users.Add(It.IsAny<User>())).Callback<User>((s) => mockUserListImport.Add(s));

            var contractTypesContainer = new ContractTypesContainer(mockContextImport.Object);
            var makandraProcessContainer = new MakandraProcessContainer(mockContextImport.Object);
            var userContainer = new UserContainer(mockContextImport.Object);
            var procedureContainer = new ProcedureContainer(mockContextImport.Object);

            var contractType1 = new ContractType
            {
                ID = 1,
                Name = "Festanstellung"
            };

            var contractType2 = new ContractType
            {
                ID = 2,
                Name = "Werkstudent"
            };

            var contractType3 = new ContractType
            {
                ID = 3,
                Name = "Praktikum"
            };

            var contractType4 = new ContractType
            {
                ID = 4,
                Name = "Trainee"
            };

            localContractTypeListImport.Add(contractType1);
            localContractTypeListImport.Add(contractType2);
            localContractTypeListImport.Add(contractType3);
            localContractTypeListImport.Add(contractType4);

            var process1 = new MakandraProcess
            {
                Id = 1,
                Name = "test1"
            };
            
            localMakandraProcessListImport.Add(process1);

            var user1 = new User
            {
                Id = 1,
                Email = "test1@makandra.de",
                FullName = "Test1",
                Password = "Test1",
                Active = true
            };

            var user2 = new User
            {
                Id = 2,
                Email = "test2@makandra.de",
                FullName = "Test2",
                Password = "Test2",
                Active = false
            };

            var user3 = new User
            {
                Id = 3,
                Email = "test3@makandra.de",
                FullName = "Test3",
                Password = "Test3",
                Active = true
            };

            var user4 = new User
            {
                Id = 4,
                Email = "test4@makandra.de",
                FullName = "Test4",
                Password = "Test4",
                Active = false
            };

            var user5 = new User
            {
                Id = 5,
                Email = "test5@makandra.de",
                FullName = "Test5",
                Password = "Test5",
                Active = true
            };

            localUserListImport.Add(user1);
            localUserListImport.Add(user2);
            localUserListImport.Add(user3);
            localUserListImport.Add(user4);
            localUserListImport.Add(user5);

            string json = "[\n" +
                "  {\n" +
                "    \"Deadline\": \"2024-07-27T00:00:00Z\",\n" +
                "    \"name\": \"Neuzugang HR\",\n" +
                "    \"EstablishingContractTypeId\": 4,\n" +
                "    \"ReferencePersonId\": 2,\n" +
                "    \"ResponsiblePersonId\": 1,\n" +
                "    \"basedProcessId\": 1,\n" +
                "    \"Archived\": false,\n" +
                "    \"completedTasks\": 0,\n" +
                "    \"openTasks\": 0,\n" +
                "    \"inprogressTasks\": 0,\n" +
                "    \"progressbar\": 0.0,\n" +
                "    \"TargetDepartment\": [],\n" +
                "    \"makandraTasks\": [],\n" +
                "    \"ProcedureDepartments\": []\n" +
                "  },\n" +
                "  {\n" +
                "    \"Deadline\": \"2024-07-27T00:00:00Z\",\n" +
                "    \"EstablishingContractTypeId\": 4,\n" +
                "    \"ReferencePersonId\": 2,\n" +
                "    \"ResponsiblePersonId\": 1,\n" +
                "    \"basedProcessId\": 1,\n" +
                "    \"Archived\": false,\n" +
                "    \"completedTasks\": 0,\n" +
                "    \"openTasks\": 0,\n" +
                "    \"inprogressTasks\": 0,\n" +
                "    \"progressbar\": 0.0,\n" +
                "    \"TargetDepartment\": [],\n" +
                "    \"makandraTasks\": [],\n" +
                "    \"ProcedureDepartments\": []\n" +
                "  },\n" +
                "  {\n" +
                "    \"Deadline\": \"2024-07-27T00:00:00Z\",\n" +
                "    \"name\": \"Neuzugang HR\",\n" +
                "    \"EstablishingContractTypeId\": 5,\n" +
                "    \"ReferencePersonId\": 2,\n" +
                "    \"ResponsiblePersonId\": 1,\n" +
                "    \"basedProcessId\": 1,\n" +
                "    \"Archived\": false,\n" +
                "    \"completedTasks\": 0,\n" +
                "    \"openTasks\": 0,\n" +
                "    \"inprogressTasks\": 0,\n" +
                "    \"progressbar\": 0.0,\n" +
                "    \"TargetDepartment\": [],\n" +
                "    \"makandraTasks\": [],\n" +
                "    \"ProcedureDepartments\": []\n" +
                "  },\n" +
                "  {\n" +
                "    \"Deadline\": \"2024-07-27T00:00:00Z\",\n" +
                "    \"name\": \"Neuzugang HR\",\n" +
                "    \"EstablishingContractTypeId\": 4,\n" +
                "    \"ReferencePersonId\": 2,\n" +
                "    \"ResponsiblePersonId\": 1,\n" +
                "    \"basedProcessId\": 2,\n" +
                "    \"Archived\": false,\n" +
                "    \"completedTasks\": 0,\n" +
                "    \"openTasks\": 0,\n" +
                "    \"inprogressTasks\": 0,\n" +
                "    \"progressbar\": 0.0,\n" +
                "    \"TargetDepartment\": [],\n" +
                "    \"makandraTasks\": [],\n" +
                "    \"ProcedureDepartments\": []\n" +
                "  },\n" +
                "  {\n" +
                "    \"Deadline\": \"2024-07-27T00:00:00Z\",\n" +
                "    \"name\": \"Neuzugang HR\",\n" +
                "    \"EstablishingContractTypeId\": 4,\n" +
                "    \"ReferencePersonId\": 2,\n" +
                "    \"ResponsiblePersonId\": 6,\n" +
                "    \"basedProcessId\": 1,\n" +
                "    \"Archived\": false,\n" +
                "    \"completedTasks\": 0,\n" +
                "    \"openTasks\": 0,\n" +
                "    \"inprogressTasks\": 0,\n" +
                "    \"progressbar\": 0.0,\n" +
                "    \"TargetDepartment\": [],\n" +
                "    \"makandraTasks\": [],\n" +
                "    \"ProcedureDepartments\": []\n" +
                "  },\n" +
                "  {\n" +
                "    \"Deadline\": \"2024-07-27T00:00:00Z\",\n" +
                "    \"name\": \"Neuzugang HR\",\n" +
                "    \"EstablishingContractTypeId\": 4,\n" +
                "    \"ReferencePersonId\": 6,\n" +
                "    \"ResponsiblePersonId\": 1,\n" +
                "    \"basedProcessId\": 1,\n" +
                "    \"Archived\": false,\n" +
                "    \"completedTasks\": 0,\n" +
                "    \"openTasks\": 0,\n" +
                "    \"inprogressTasks\": 0,\n" +
                "    \"progressbar\": 0.0,\n" +
                "    \"TargetDepartment\": [],\n" +
                "    \"makandraTasks\": [],\n" +
                "    \"ProcedureDepartments\": []\n" +
                "  }\n" +
                "]";

            procedureContainer.Import(contractTypesContainer, makandraProcessContainer, userContainer, json);
           
           Assert.Single(mockProcedureListImport);

           Procedure procedure = mockProcedureListImport[0];

            Assert.Equal("27.07.2024", procedure.Deadline.ToString("dd.MM.yyyy"));
            Assert.Equal("Neuzugang HR", procedure.name);
            Assert.Equal(4, procedure.EstablishingContractTypeId);
            Assert.Equal(1, procedure.ResponsiblePersonId);
            Assert.Equal(1, procedure.basedProcessId);
            Assert.False(procedure.Archived);
            Assert.Equal(0, procedure.completedTasks);
            Assert.Equal(0, procedure.completedTasks);
            Assert.Equal(0, procedure.openTasks);
            Assert.Equal(0, procedure.inprogressTasks);
            Assert.Equal(0.0, procedure.progressbar);
            Assert.Empty(procedure.TargetDepartment);
            Assert.Empty(procedure.makandraTasks);
            Assert.Empty(procedure.ProcedureDepartments);
        }
    
    }
}