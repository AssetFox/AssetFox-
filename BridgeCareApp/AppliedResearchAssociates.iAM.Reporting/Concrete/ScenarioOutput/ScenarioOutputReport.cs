using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Newtonsoft.Json;

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

        public ScenarioOutputReport(UnitOfDataPersistenceWork unitOfWork, string name, ReportIndexDTO results)
        {
            _unitofwork = unitOfWork;
            ReportTypeName = name;
            ID = Guid.NewGuid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
        }

        public async Task Run(string parameters)
        {
            // TODO:  Don't regenerate the report if it has already been generated AND the date on the file was after the LastRun date of the
            // scenario.

            // Determine the Guid for the simulation
            if (!Guid.TryParse(parameters, out Guid simulationGuid))
            {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IndicateError();
                return;
            }
            SimulationID = simulationGuid;

            // Check for simulation existence
            string reportFileName;
            var simulationName = _unitofwork.SimulationRepo.GetSimulationName(simulationGuid);
            if (simulationName == null)
            {
                IndicateError();
                Errors.Add($"Failed to find simulation ID {SimulationID}.");
                return;
            }

            if (!string.IsNullOrEmpty(simulationName))
            {
                reportFileName = $"Reports\\{simulationName}-{SimulationID}.json";
            }
            else
            {
                reportFileName = $"Reports\\{SimulationID}.json";
            }

            // Pull the simulation object
            Analysis.Engine.SimulationOutput simulationOutput;
            try
            {
                simulationOutput = _unitofwork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationGuid);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to pull simulation output.  Has the simulation been run?");
                Errors.Add(e.Message);
                return;
            }

            // Save the output to a file
            try
            {
                using var reportFileWriter = File.CreateText(reportFileName);
                JsonSerializer serializer = new();
                serializer.Serialize(reportFileWriter, simulationOutput);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to write file to server");
                Errors.Add(e.Message);
                return;
            }

            // Report success with location of file
            Results = reportFileName;  // This is not set until here to ensure the file was created correctly
            IsComplete = true;
            Status = "File generated.";
            return;
        }

        private void IndicateError()
        {
            Status = "Simulation output report completed with errors";
            IsComplete = true;
        }
    }
}
