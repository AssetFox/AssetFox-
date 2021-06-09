using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class ScenarioOutputReport : IReport
    {
        private UnitOfDataPersistenceWork _unitofwork;

        public Guid ID { get; set; }
        public Guid? SimulationID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public ScenarioOutputReport(UnitOfDataPersistenceWork uow, string name, ReportIndexEntity results)
        {
            _unitofwork = uow;
            ReportTypeName = name;
            ID = Guid.NewGuid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
        }

        public async Task Run(string parameters)
        {
            // Determine the Guid for the simulation
            if (!Guid.TryParse(parameters, out Guid simulationGuid)) {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IsComplete = true;
                Status = "Simulation output report completed with errors";
                return;
            }
            SimulationID = simulationGuid;

            // Pull the simulation object
            Analysis.SimulationOutput simulationOutput;
            try
            {
                simulationOutput = _unitofwork.SimulationOutputRepo.GetSimulationOutput(simulationGuid);
            }
            catch(Exception e)
            {
                Status = "Simulation output report completed with errors";
                IsComplete = true;
                Errors.Add("Failed to pull simulation output.  Has the simulation been run?");
                Errors.Add(e.Message);
                return;
            }
            var outputJson = JsonConvert.SerializeObject(simulationOutput);

            // Save the output to a file
            File.WriteAllText(@"output.txt", outputJson);

            // Report success with location of file
            Results = "output.txt";
            IsComplete = true;
            Status = "File generated.";
            return;
        }
    }
}
