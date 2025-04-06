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
    /// Unit Tests for Duedate
    /// </summary>
    public class UnitTestDuedate
    {
        /// <summary>
        /// Tests the Import of a Duedate
        /// </summary>
        /// <author>Arian Scheremet</author>
        [Fact]
        public void DuedateImport()
        {
            var mockContextImport = new Mock<MakandraContext>();
            mockContextImport.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var localDuedateListImport = new List<Duedate>();
            var mockDuedateListImport = new List<Duedate>();
            mockContextImport.Setup(c => c.Duedates).ReturnsDbSet(localDuedateListImport);
            mockContextImport.Setup(c => c.Duedates.Add(It.IsAny<Duedate>())).Callback<Duedate>((s) => mockDuedateListImport.Add(s));
            var duedateContainer = new DuedateContainer(mockContextImport.Object);

            string json = "[\n" +
                    "  {\n" +
                    "    \"ID\": 8,\n" +
                    "    \"Name\": \"ValidTest\",\n" +
                    "    \"Days\": 365\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 9\n" +
                    "  }\n" +
                    "]";
            duedateContainer.Import(json);

            Assert.Equal(1, mockDuedateListImport.Count());
            Assert.Equal("ValidTest", mockDuedateListImport[0].Name);
            Assert.Equal(365, mockDuedateListImport[0].Days);
            Assert.Equal(8, mockDuedateListImport[0].ID);
        }
    }
}