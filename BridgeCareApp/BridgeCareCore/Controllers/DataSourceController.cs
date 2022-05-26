using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Controllers
{
    public class DataSourceController : BridgeCareCoreBaseController
    {
        private readonly IDataSourceRepository _dataSourceRepository;

        public DataSourceController(
            IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor contextAccessor,
            IDataSourceRepository dataSourceRepository)
            : base(esecSecurity,
                  unitOfWork,
                  hubService,
                  contextAccessor) 
        {
            _dataSourceRepository = dataSourceRepository ?? throw new ArgumentNullException(nameof(dataSourceRepository));
        }
    }
}
