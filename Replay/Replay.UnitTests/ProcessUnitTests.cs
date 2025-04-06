using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Replay.Data;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Replay.Models;
using Microsoft.AspNetCore.Mvc;
using Replay.Controllers;
using Microsoft.Extensions.Logging;
using Replay.Container.Account.MTM;
using Replay.Container;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit.Abstractions;
using Replay.Container.Account;
using Replay.ViewModels.Process;
using Replay.Models.Account;
using Replay.Models.MTM;
using Microsoft.AspNetCore.Http;
using Replay.Models.Account.MTM;

namespace Replay.UnitTests
{
    public class ProcessUnitTest
    {
        private readonly ITestOutputHelper output;

        public ProcessUnitTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async Task Process_Created_Correct_Test()
        {
            var mockSet = new Mock<DbSet<MakandraProcess>>();
            var mockContext = new Mock<MakandraContext>();
            var processListMock = new List<MakandraProcess>();
            var roleListMock = new List<Role>()
            {
                new Role
                {
                    Id = 1,
                    Name = "Administrator"
                },
                new Role
                {
                    Id = 2,
                    Name = "IT"
                }
            };
            var contractTypeListMock = new List<ContractType>()
            {
                new ContractType
                {
                    ID = 1,
                    Name = "Festanstellung"
                },
                new ContractType
                {
                    ID = 2,
                    Name = "Werkstudent"
                },
                new ContractType
                {
                    ID = 3,
                    Name = "Praktikum"
                },
                new ContractType
                {
                    ID = 4,
                    Name = "Trainee"
                }
        };

            mockContext.Setup(x => x.Processes).ReturnsDbSet(processListMock);
            mockContext.Setup(x => x.Roles).ReturnsDbSet(roleListMock);
            mockContext.Setup(x => x.ContractTypes).ReturnsDbSet(contractTypeListMock);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);



            var mockRoleContainer = new Mock<RoleContainer>(mockContext.Object);
            var mockUserContainer = new Mock<UserContainer>(mockContext.Object);
            var mockProcessContainer = new Mock<MakandraProcessContainer>(mockContext.Object);
            var mockPermissionContainer = new Mock<PermissionContainer>(mockContext.Object);
            var mockTaskTemplateContainer = new Mock<TaskTemplateContainer>(mockContext.Object);
            var mockProcessRoleContainer = new Mock<MakandraProcessRoleContainer>(mockContext.Object);
            var mockDepartmentContainer = new Mock<DepartmentContainer>(mockContext.Object);
            var mockContractTypeContainer = new Mock<ContractTypesContainer>(mockContext.Object);
            mockProcessContainer.Setup(m => m.AddProcess(It.IsAny<MakandraProcess>())).Callback<MakandraProcess>((s) => processListMock.Add(s));
            var controller = new MakandraProcessController(
                mockContext.Object, mockProcessContainer.Object, mockUserContainer.Object, mockRoleContainer.Object,
                mockTaskTemplateContainer.Object, mockProcessRoleContainer.Object, mockDepartmentContainer.Object,
                mockContractTypeContainer.Object
            );


            var process = new MakandraProcess {Name = "Administrator"};

            MakandraProcessCreateViewModel processViewModel = new MakandraProcessCreateViewModel();

            processViewModel.Name = "Test";

            processViewModel.AllRoles = mockRoleContainer.Object;
            processViewModel.AllTasks = mockTaskTemplateContainer.Object;
            List<RoleSelectionViewModel> roleSelectionViewModels = new List<RoleSelectionViewModel>();
            roleListMock.ForEach(r => roleSelectionViewModels.Add(new RoleSelectionViewModel
            {
                Name = r.Name,
                Id = r.Id,
                IsSelected = false
            }));
            processViewModel.Roles = roleSelectionViewModels.ToArray();

            // Act
            var result = await controller.Create(processViewModel);
            output.WriteLine(processListMock.First().Name);

            // Assert
            Assert.NotNull(result);
            Assert.True(1 == processListMock.Count);

        }

        [Fact]
        public async Task Process_Edit_Correct_Test()
        {
            var mockSet = new Mock<DbSet<MakandraProcess>>();
            var mockContext = new Mock<MakandraContext>();
            var processListMock = new List<MakandraProcess>();
            var roleListMock = new List<Role>()
            {
                new Role
                {
                    Id = 1,
                    Name = "Administrator"
                },
                new Role
                {
                    Id = 2,
                    Name = "IT"
                }
            };
            var userListMock = new List<User>() {
                new User()
                {
                    Id = 1,
                    FullName = "Test User",
                    Active = true,
                    Email = "testuser@makandra.de",
                    Password = "password",
                }
            };
            var userRolesListMock = new List<UserRole>()
            {
                new UserRole()
                {
                    RoleId = 1,
                    Role = roleListMock[0],
                    User = userListMock[0],
                    UserId = 1
                }
            };
            userListMock[0].UserRoles = userRolesListMock;
            var contractTypeListMock = new List<ContractType>()
            {
                new ContractType
                {
                    ID = 1,
                    Name = "Festanstellung"
                },
                new ContractType
                {
                    ID = 2,
                    Name = "Werkstudent"
                },
                new ContractType
                {
                    ID = 3,
                    Name = "Praktikum"
                },
                new ContractType
                {
                    ID = 4,
                    Name = "Trainee"
                }
            };

            var processRolesMock = new List<MakandraProcessRole>();

            mockContext.Setup(x => x.Processes).ReturnsDbSet(processListMock);
            mockContext.Setup(x => x.Roles).ReturnsDbSet(roleListMock);
            mockContext.Setup(x => x.ContractTypes).ReturnsDbSet(contractTypeListMock);
            mockContext.Setup(x => x.UserRoles).ReturnsDbSet(userRolesListMock);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            mockContext.Setup(x => x.MakandraProcessRoles).ReturnsDbSet(processRolesMock);


            var mockRoleContainer = new Mock<RoleContainer>(mockContext.Object);
            var mockUserContainer = new Mock<UserContainer>(mockContext.Object);
            var mockProcessContainer = new Mock<MakandraProcessContainer>(mockContext.Object);
            var mockPermissionContainer = new Mock<PermissionContainer>(mockContext.Object);
            var mockTaskTemplateContainer = new Mock<TaskTemplateContainer>(mockContext.Object);
            var mockDepartmentContainer = new Mock<DepartmentContainer>(mockContext.Object);
            var mockContractTypeContainer = new Mock<ContractTypesContainer>(mockContext.Object);
            var mockProcessRolesContainer = new Mock<MakandraProcessRoleContainer>(mockContext.Object);
            var mockUserRolesContainer = new Mock<UserRolesContainer>(mockContext.Object);
            mockProcessRolesContainer.Setup(x => x.GetRolesFromProcess(It.IsAny<MakandraProcess>())).ReturnsAsync(roleListMock);
            mockUserRolesContainer.Setup(x => x.GetRolesFromUser(It.IsAny<User>())).ReturnsAsync(roleListMock);
            mockProcessContainer.Setup(m => m.AddProcess(It.IsAny<MakandraProcess>())).Callback<MakandraProcess>((s) => processListMock.Add(s));
            mockUserContainer.Setup(u => u.GetLoggedInUser(It.IsAny<HttpContext>())).ReturnsAsync(userListMock[0]);
            var controller = new MakandraProcessController(
                mockContext.Object, mockProcessContainer.Object, mockUserContainer.Object, mockRoleContainer.Object,
                mockTaskTemplateContainer.Object, mockProcessRolesContainer.Object, mockDepartmentContainer.Object,
                mockContractTypeContainer.Object
            );

            MakandraProcessCreateViewModel processViewModel = new MakandraProcessCreateViewModel();

            processViewModel.Name = "Test";

            processViewModel.AllRoles = mockRoleContainer.Object;
            processViewModel.AllTasks = mockTaskTemplateContainer.Object;
            List<RoleSelectionViewModel> roleSelectionViewModels = new List<RoleSelectionViewModel>();
            roleListMock.ForEach(r => roleSelectionViewModels.Add(new RoleSelectionViewModel
            {
                Name = r.Name,
                Id = r.Id,
                IsSelected = true
            }));
            processViewModel.Roles = roleSelectionViewModels.ToArray();

            // Act
            var result = await controller.Create(processViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.True(processListMock.First().Name == "Test");
            Assert.True(1 == processListMock.Count);

            MakandraProcessEditViewModel editProcessViewModel = new MakandraProcessEditViewModel();

            editProcessViewModel.Id = processListMock.First().Id;
            editProcessViewModel.Name = "Edited Test";
            editProcessViewModel.Roles = mockRoleContainer.Object;
            editProcessViewModel.AllTaskTemplatesForThisProcess = mockTaskTemplateContainer.Object;
            editProcessViewModel.RoleSelection = roleSelectionViewModels.ToArray();
            var editResult = await controller.Edit(editProcessViewModel);

            Assert.NotNull(result);
            output.WriteLine(result.ToString());
            Assert.True(processListMock.First().Name == "Edited Test");
            Assert.True(1 == processListMock.Count);
        }
    }
}
