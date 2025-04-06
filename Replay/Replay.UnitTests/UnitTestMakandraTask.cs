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
using Xunit.Abstractions;


namespace Replay.UnitTests
{
    /// <author>Florian Fendt</author>

    public class UnitTestMakandraTask
    {
        /// <summary>
        /// Checks if the TargetDate is correctly calculated based on the Duedate
        /// </summary>
        [Fact]
        public async void TestSelectTargetDate()
        {
            var mockContext = new Mock<MakandraContext>();
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            var localDuedateList = new List<Duedate>();
            mockContext.Setup(x => x.Duedates).ReturnsDbSet(localDuedateList);
            Duedate duedateasap = new Duedate {
                ID = 1,
                Name = "ASAP",
                Days = 0
            };
            Duedate duedate2mvA = new Duedate {
                ID = 2,
                Name = "2 Monate vor Antritt",
                Days = -60
            };
            Duedate duedate2wvS = new Duedate {
                ID = 3,
                Name = "2 Wochen vor Start",
                Days = -14
            };
            Duedate duedateaeAT = new Duedate {
                ID = 4,
                Name = "Am ersten Arbeitstag",
                Days = 0
            };
            Duedate duedate3WnAB = new Duedate {
                ID = 5,
                Name = "3 Wochen nach Arbeitsbeginn",
                Days = 21
            };
            Duedate duedate3MnAB = new Duedate {
                ID = 6,
                Name = "3 Monate nach Arbeitsbeginn",
                Days = 90
            };
            Duedate duedate6MnAb = new Duedate {
                ID = 7,
                Name = "6 Monate nach Arbeitsbeginn",
                Days = 180
            };
            localDuedateList.Add(duedateasap);
            localDuedateList.Add(duedate2mvA);
            localDuedateList.Add(duedate2wvS);
            localDuedateList.Add(duedateaeAT);
            localDuedateList.Add(duedate3WnAB);
            localDuedateList.Add(duedate3MnAB);
            localDuedateList.Add(duedate6MnAb);
            DateTime testdate = new DateTime(2024, 8, 1);
            MakandraTaskState testState = new MakandraTaskState {
                Id = 1,
                Name = "Test123"
            };
            MakandraTask testTask = new MakandraTask{
                Id = 5,
                Name = "TestTask",
                TargetDate = testdate,
                State = testState,
                StateId = 1,
                ProcedureId = 5,
                Archived = false
            };
            DateTime datetimeToday = DateTime.Today;
            DuedateContainer duedateContainer = new DuedateContainer(mockContext.Object);
            DateTime result = await testTask.SelectTargetDate(duedateasap.ID, testdate, duedateContainer);
            Assert.Equal(datetimeToday, result);
            result = await testTask.SelectTargetDate(duedate2mvA.ID, testdate, duedateContainer);
            Assert.Equal(testdate.AddDays(-60), result);
            result = await testTask.SelectTargetDate(duedate2wvS.ID, testdate, duedateContainer);
            Assert.Equal(testdate.AddDays(-14), result);
            result = await testTask.SelectTargetDate(duedateaeAT.ID, testdate, duedateContainer);
            Assert.Equal(testdate, result);
            result = await testTask.SelectTargetDate(duedate3WnAB.ID, testdate, duedateContainer);
            Assert.Equal(testdate.AddDays(21), result);
            result = await testTask.SelectTargetDate(duedate3MnAB.ID, testdate, duedateContainer);
            Assert.Equal(testdate.AddDays(90), result);
            result = await testTask.SelectTargetDate(duedate6MnAb.ID, testdate, duedateContainer);
            Assert.Equal(testdate.AddDays(180), result);
        }
        /// <summary>
        /// Checks if the roles are correctly changes 
        /// </summary>
        [Fact]
        public async void TestUpdateTaskRoles()
        {
            var mockContext = new Mock<MakandraContext>();
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            var localTaskRoleList = new List<MakandraTaskRole>();
            var mockTaskRoleList = new List<MakandraTaskRole>();
            var localRoleList = new List<Role>();
            mockContext.Setup(x => x.TaskRoles).ReturnsDbSet(localTaskRoleList);
            mockContext.Setup(x => x.Add(It.IsAny<MakandraTaskRole>())).Callback<MakandraTaskRole>((s) => mockTaskRoleList.Add(s));
            mockContext.Setup(x => x.Remove(It.IsAny<MakandraTaskRole>())).Callback<MakandraTaskRole>((s) => mockTaskRoleList.Remove(s));
            mockContext.Setup(x => x.Roles).ReturnsDbSet(localRoleList);
            List<Role> rolesList = new List<Role>();
            MakandraTaskState testState = new MakandraTaskState {
                Id = 1,
                Name = "Test123"
            };
            DateTime testdate = new DateTime(2024, 8, 1);
            MakandraTask testTask = new MakandraTask{
                Id = 5,
                Name = "TestTask",
                TargetDate = testdate,
                State = testState,
                StateId = 1,
                ProcedureId = 5,
                Archived = false
            };
            Role role1 = new Role {
                Id = 1,
                Name = "testRole1"
            };
            Role role2 = new Role {
                Id = 2,
                Name = "testRole2"
            };
            Role role3 = new Role {
                Id = 3,
                Name = "testRole3"
            };
            MakandraTaskRole taskrole1 = new MakandraTaskRole {
                TaskId = 5,
                RoleId = 1,
                Task = testTask,
                Role = role1
            };
            MakandraTaskRole taskrole2 = new MakandraTaskRole {
                TaskId = 5,
                RoleId = 2,
                Task = testTask,
                Role = role2
            };
            MakandraTaskRole taskrole3 = new MakandraTaskRole {
                TaskId = 5,
                RoleId = 3,
                Task = testTask,
                Role = role3
            };
            localRoleList.Add(role1);
            localRoleList.Add(role2);
            localRoleList.Add(role3);
            rolesList.Add(role2);
            rolesList.Add(role3);
            localTaskRoleList.Add(taskrole1);
            localTaskRoleList.Add(taskrole2);
            List<int> editorIds = new List<int>();
            editorIds.Add(2);
            editorIds.Add(3);
            mockTaskRoleList.Add(taskrole2);
            MakandraTaskRoleContainer taskrolecontainer= new MakandraTaskRoleContainer(mockContext.Object);
            RoleContainer rolecontainer= new RoleContainer(mockContext.Object);
            await testTask.UpdateTaskRoles(taskrolecontainer, rolesList, editorIds, rolecontainer);
            Assert.Equal(2, mockTaskRoleList.Count);
            Assert.Equal(2, testTask.EditAccess.Count);
            Assert.True(!mockTaskRoleList.Contains(taskrole1));
            Assert.True(!testTask.EditAccess.Contains(taskrole1));
            Assert.True(mockTaskRoleList.Contains(taskrole2));
            Assert.True(testTask.EditAccess.Contains(taskrole2));
            Assert.True(mockTaskRoleList.Contains(taskrole3));
            Assert.True(testTask.EditAccess.Contains(taskrole3));
        }
    }
}