using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IDataSourceRepository
    {
        List<BaseDataSourceDTO> GetDataSources();

        BaseDataSourceDTO GetDataSource(Guid id);

        void UpsertDatasource(BaseDataSourceDTO dataSource);

        void DeleteDataSource(Guid id);

        Dictionary<string, string> GetRawData(Dictionary<AttributeDTO, string> dictionary);
    }
}
