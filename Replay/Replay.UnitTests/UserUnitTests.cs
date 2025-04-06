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
    /// Unit tests for <see cref="User"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class UserUnitTests
    {
        private readonly ITestOutputHelper output;

        public UserUnitTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async Task User_Register_Correct_Test()
        {
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<MakandraContext>();
            var userListMock = new List<User>();

            mockContext.Setup(x => x.Users).ReturnsDbSet(userListMock);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            mockContext.Setup(c => c.Add(It.IsAny<User>())).Callback<User>((s) => userListMock.Add(s));


            var mockUserContainer = new Mock<UserContainer>(mockContext.Object);
            var mockRoleContainer = new Mock<RoleContainer>(mockContext.Object);
            var mockUserRolesContainer = new Mock<UserRolesContainer>(mockContext.Object);
            mockUserContainer.Setup(m => m.AddUser(It.IsAny<User>())).Callback<User>((s) => userListMock.Add(s));
            var controller = new AccountController(
                       mockUserContainer.Object,
                       mockRoleContainer.Object,
                       mockUserRolesContainer.Object);
            
            var user = new User { FullName = "John Doe", Email = "johndoe@makandra.com", Password = "123" };
            
            // Act
            var result = await controller.Register(user);

            // Assert
            Assert.NotNull(result);
            Assert.True(1 == userListMock.Count);

        }        
    }
}