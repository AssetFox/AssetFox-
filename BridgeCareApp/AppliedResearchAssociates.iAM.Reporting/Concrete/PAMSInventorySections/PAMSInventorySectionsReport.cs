using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSInventorySectionsReport : IReport
    {
        public PAMSInventorySectionsReport(IUnitOfWork uow, string name, ReportIndexDTO results)
        {
            // TODO
        }

        public Guid ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid? SimulationID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Results => throw new NotImplementedException();

        public ReportType Type => throw new NotImplementedException();

        public string ReportTypeName => throw new NotImplementedException();

        public List<string> Errors => throw new NotImplementedException();

        public bool IsComplete => throw new NotImplementedException();

        public string Status => throw new NotImplementedException();

        public Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null) => throw new NotImplementedException();
    }
}
