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
    /// Unit Tests for ContractType
    /// </summary>
    public class UnitTestContractType
    {
        /// <summary>
        /// Tests the Import of a ContractType
        /// </summary>
        /// <author>Arian Scheremet</author>
        [Fact]
        public void ContractTypeImport()
        {
            var mockContextImport = new Mock<MakandraContext>();
            mockContextImport.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var localContractTypeListImport = new List<ContractType>();
            var mockContractTypeListImport = new List<ContractType>();
            mockContextImport.Setup(c => c.ContractTypes).ReturnsDbSet(localContractTypeListImport);
            mockContextImport.Setup(c => c.ContractTypes.Add(It.IsAny<ContractType>())).Callback<ContractType>((s) => mockContractTypeListImport.Add(s));
            var contractTypesContainer = new ContractTypesContainer(mockContextImport.Object);

            string json = "[\n" +
                    "  {\n" +
                    "    \"ID\": 5,\n" +
                    "    \"Name\": \"ValidTest\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 6\n" +
                    "  }\n" +
                    "]";
            contractTypesContainer.Import(json);

            Assert.Equal(5, mockContractTypeListImport.Count());
            Assert.Equal("ValidTest", mockContractTypeListImport[4].Name);
            Assert.Equal(5, mockContractTypeListImport[4].ID);
        }
    }
}








