using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace Replay.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="Role"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class RoleUnitTests
    {
        private readonly ITestOutputHelper output;

        public RoleUnitTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async Task Role_Created_Correct_Test()
        {
            var mockSet = new Mock<DbSet<Role>>();
            var mockContext = new Mock<MakandraContext>();
            var roleListMock = new List<Role>();

            mockContext.Setup(x => x.Roles).ReturnsDbSet(roleListMock);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            mockContext.Setup(c => c.Add(It.IsAny<Role>())).Callback<Role>((s) => roleListMock.Add(s));


            var mockRoleContainer = new Mock<RoleContainer>(mockContext.Object);
            var mockPermissionContainer = new Mock<PermissionContainer>(mockContext.Object);
            var mockRolePermissionsContainer = new Mock<RolePermissionContainer>(mockContext.Object);
            mockRoleContainer.Setup(m => m.AddRole(It.IsAny<Role>())).Callback<Role>((s) => roleListMock.Add(s));
            var controller = new RoleController(mockRoleContainer.Object, mockRolePermissionsContainer.Object, mockPermissionContainer.Object);

            var role = new Role { Name = "Administrator"};

            // Act
            var result = await controller.Create(role);
            output.WriteLine(roleListMock.First().Name);

            // Assert
            Assert.NotNull(result);
            Assert.True(1 == roleListMock.Count);

        }        
    }
}