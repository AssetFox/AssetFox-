﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class HelloWorldReport : IReport
    {
        private Guid _id;
        private List<string> _errorList;
        private string _reportTypeName;

        public HelloWorldReport(IUnitOfWork repository, string name, ReportIndexDTO results)
        {
            _id = Guid.NewGuid();
            _errorList = new List<string>();
            _reportTypeName = name;
        }

        public Guid ID { get => _id; set { } }
        public Guid? SimulationID { get => null; set { } }

        public Guid? NetworkID { get; set; }

        public string Results => "<p>Hello, world!</p>";

        public ReportType Type => ReportType.HTML;

        public string ReportTypeName => _reportTypeName;

        public List<string> Errors => _errorList;

        public bool IsComplete => true;

        public string Status => "No report to run.";

        public string Suffix => throw new NotImplementedException();
        
        public string Criteria { get => null; set { } }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            try
            {
                var test = JsonConvert.DeserializeObject<TestObject>(parameters);
                if (test != null)
                    Console.WriteLine(test.Something);
            }
            catch(Exception e)
            {
                _errorList.Add($"Unable to parse content due to {e.Message}");
            }

        }
    }

    public class TestObject
    {
        public string Something { get; set; }
    }
}
