using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public static class AttributeTestSetup
    {
        public static string ValidAttributeName() => "A" + RandomStrings.Length11();

        public static NumericAttribute Numeric(Guid? id = null, string name = null, Guid? dataSourceId = null, ConnectionType connectionType = ConnectionType.MSSQL)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = name ?? ValidAttributeName();
            var attribute = new NumericAttribute(2, 3, 1, resolvedId, randomName, "AVERAGE", "Command", connectionType, "connectionString", false, false, dataSourceId);
            return attribute;
        }

        public static AttributeDTO NumericDto(BaseDataSourceDTO dataSourceDTO, Guid? id = null, string name = null, ConnectionType connectionType = ConnectionType.MSSQL)
        {
            var attribute = Numeric(id, name, dataSourceDTO.Id, connectionType);
            var dto = AttributeDtoDomainMapper.ToDto(attribute, dataSourceDTO);
            return dto;
        }

        public static TextAttribute Text(Guid? id = null, string name = null, bool calculated = false)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = name ?? ValidAttributeName();
            var attribute = new TextAttribute("defaultValue", resolvedId, randomName, "PREDOMINANT", "command", Data.ConnectionType.MSSQL, "connectionString", calculated, true, Guid.Empty);
            return attribute;
        }


        private static bool AttributesHaveBeenCreated = false;
        private static readonly object AttributeLock = new object();
        private static SQLDataSourceDTO CacheDataSourceForAttributes { get; set; }

        public static SQLDataSourceDTO CreateAttributes(IUnitOfWork unitOfWork)
        {
            if (!AttributesHaveBeenCreated)
            {
                lock (AttributeLock)  // Necessary as long as there is a chance that some tests may run in paralell. Can we eliminate that possiblity?
                {
                    if (!AttributesHaveBeenCreated)
                    {
                        var config = TestConfiguration.Get();
                        SQLDataSourceDTO dataSourceToApply = null;
                        if (!unitOfWork.DataSourceRepo.GetDataSources().Any(_ => _.Type == "SQL"))
                        {
                            dataSourceToApply = new SQLDataSourceDTO
                            {
                                Id = Guid.NewGuid(),
                                Name = "Test SQL DataSource",
                                ConnectionString = config.GetConnectionString("BridgeCareConnex")
                            };
                            unitOfWork.DataSourceRepo.UpsertDatasource(dataSourceToApply);
                        }
                        else
                        {
                            dataSourceToApply = (SQLDataSourceDTO)unitOfWork.DataSourceRepo.GetDataSources().First(_ => _.Type == "SQL");
                        }
                        CacheDataSourceForAttributes = dataSourceToApply;
                        var attributesToInsert = AttributeDtoLists.AttributeSetupDtos();
                        foreach (var attribute in attributesToInsert)
                        {
                            attribute.DataSource = dataSourceToApply;
                        }
                        unitOfWork.AttributeRepo.UpsertAttributes(attributesToInsert);
                        AttributesHaveBeenCreated = true;
                    }
                }
            }
            return CacheDataSourceForAttributes;
        }

    }
}
