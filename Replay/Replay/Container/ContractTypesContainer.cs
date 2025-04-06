using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Replay.Models;

using System.Text.Json;

namespace Replay.Container
{

    /// <summary>
    /// Managing the connection to the database of the <see cref="ContractType"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class ContractTypesContainer
    {

        private MakandraContext MakandraContext;

        /// <summary>
        /// Create new Container
        /// </summary>
        /// <param name="makandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public ContractTypesContainer(MakandraContext makandraContext) {
            this.MakandraContext = makandraContext;

            
            AddContractType(new ContractType {
                ID = 1,
                Name = "Festanstellung"
            });
            AddContractType(new ContractType{
                    ID = 2,
                    Name = "Werkstudent"
            });
            AddContractType( new ContractType{
                    ID = 3,
                    Name = "Praktikum"
            });
            AddContractType(new ContractType {
                ID = 4,
                Name = "Trainee"
            });
            

            for (int i = 1; i < 5; i++) {
                ContractType contractType = MakandraContext.ContractTypes.FirstOrDefault<ContractType>(s => s.ID == i);
                ContractTypesList[i - 1] = contractType;
            }
            
        }

        public static ContractType[] ContractTypesList = new ContractType[4];

             /// <summary>
        /// Add a ContractType in the database
        /// </summary>
        /// <param name="contractType">ContractType to be added</param>
        /// <author>Matthias Grafberger</author>
        public async void AddContractType(ContractType contractType) {
             var contractTypeWhenExists = await MakandraContext.ContractTypes
                .FirstOrDefaultAsync<ContractType>(s => s.Name == contractType.Name);
            
            if (contractTypeWhenExists is not null) return;

            MakandraContext.ContractTypes.Add(contractType);
            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get <see cref="ContractType"/> from its id from the database
        /// </summary>
        /// <param name="ID">Id of the wanted <see cref="ContractType"/></param>
        /// <returns>Wanted <see cref="ContractType"/></returns>
        /// <author>Matthias Grafberger</author>
        public async Task<ContractType> GetContractTypeFromID(int ID) {
             var ContractTypeWhenExists = await MakandraContext.ContractTypes
                .FirstOrDefaultAsync<ContractType>(s => s.ID == ID);
            
            return ContractTypeWhenExists;
        }

        /// <summary>
        /// Get a list of all <see cref="ContractType"/> sorted by Id
        /// </summary>
        /// <returns>List of <see cref="ContractType"/></returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<ContractType>> GetContractTypes()
        {
            var contractTypes = new List<ContractType>();

            contractTypes = await MakandraContext.ContractTypes
                .OrderBy(c => c.ID)
                .ToListAsync();

            return contractTypes;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="ContractType"/>s in the database
        /// </summary>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(string jsonFile) {

            if (jsonFile is null) return;
                
            List<ContractType> contractTypes = new List<ContractType>();

            try {
                contractTypes = JsonSerializer.Deserialize<List<ContractType>>(jsonFile);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            contractTypes.ForEach(e => {
                int h = e.IsValid();
                if (h == 0)
                {
                    AddContractType(e);
                } else {
                    Console.WriteLine("ContractType couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}