using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public string Results => "<p>Hello, world!</p>";

        public ReportType Type => ReportType.HTML;

        public string ReportTypeName => _reportTypeName;

        public List<string> Errors => _errorList;

        public bool IsComplete => true;

        public string Status => "No report to run.";

        public async Task Run(string parameters)
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
