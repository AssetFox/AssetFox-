using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class InventoryReport : IReport
    {
        private UnitOfDataPersistenceWork _repository;
        private Guid _networkId;

        public Guid ID { get; set; }
        public Guid? SimulationID { get => null; set { } }
        public string Results { get; private set; }
        public ReportType Type => ReportType.HTML;
        public string ReportTypeName { get; private set; }
        public List<string> Errors { get; private set; }
        public bool IsComplete { get; private set; }
        public string Status { get; private set; }

        private InventoryParameters _failedQuery = new InventoryParameters() { BRKey = -1, BMSID = String.Empty };

        public InventoryReport(UnitOfDataPersistenceWork repository, string name, ReportIndex results)
        {
            _repository = repository;
            ReportTypeName = name;
            // results is ignored for this report

            ID = new Guid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
            _networkId = _repository.NetworkRepo.GetPennDotNetwork().Id;
        }

        public Task Run(string parameters) => throw new NotImplementedException();

        private InventoryParameters Parse(string parameters)
        {
            try
            {
                InventoryParameters query = JsonConvert.DeserializeObject<InventoryParameters>(parameters);
                if (query == null)
                {
                    Errors.Add($"Unable to run.  No query parameters provided in request body.");
                    return _failedQuery;
                }
                return query;
            }
            catch(Exception e)
            {
                Errors.Add($"Failed to parse JSON in request body due to {e.Message}");
                return _failedQuery;
            }
        }

        //private bool Validate(InventoryParameters parameters)
        //{
        //    if (parameters.BRKey < 1 && String.IsNullOrEmpty(parameters.BMSID))
        //    {
        //        // No parameters provided
        //        return false;
        //    }

        //    if (parameters.BRKey > 0 && !String.IsNullOrEmpty(parameters.BMSID))
        //    {
        //        // Both parameters provided.  Check to see if they are the same asset
        //        _repository.MaintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(_networkId).Where(_ => _.)
        //    }

        //    // Check if asset actually exists
        //}
    }
}
