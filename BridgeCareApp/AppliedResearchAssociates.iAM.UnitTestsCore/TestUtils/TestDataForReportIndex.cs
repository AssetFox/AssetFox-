using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class TestDataForReportIndex
    {
        public static Dictionary<string, Type> SimpleReportLibrary()
        {
            var testReportLibrary = new Dictionary<string, Type>();
            testReportLibrary.Add("Test Report File", typeof(TestReportFile));
            testReportLibrary.Add("Test HTML File", typeof(TestHTMLFile));
            testReportLibrary.Add("Bad Report", typeof(TestBadReport));
            return testReportLibrary;
        }

        public static List<ReportIndexEntity> SimpleRepo()
        {
            var simpleRepo = new List<ReportIndexEntity>();
            simpleRepo.Add(new ReportIndexEntity()
            {
                Id = new Guid("e7ddde4b-d8cc-45b4-ab9e-dd513a233734"),
                SimulationID = new Guid("be82f095-c108-4ab7-af7e-cb7ecd18ede2"),
                ReportTypeName = "Test HTML File",
                Result = "<p>Hello workd!</p>",
                ExpirationDate = DateTime.Now.AddDays(2)
            });
            simpleRepo.Add(new ReportIndexEntity()
            {
                Id = new Guid("7a406cd1-6857-4288-9d93-9cc7ebd38fdf"),
                SimulationID = new Guid("be82f095-c108-4ab7-af7e-cb7ecd18ede2"),
                ReportTypeName = "Test Report File",
                Result = "<p>Hello workd!</p>",
                ExpirationDate = DateTime.Now.AddDays(2)
            });
            simpleRepo.Add(new ReportIndexEntity()
            {
                Id = new Guid("b32ecb1e-297f-4caa-9608-f28ab61cbd91"),
                SimulationID = new Guid("0951aaad-eddd-462d-ab8d-99ed3829019f"),
                ReportTypeName = "Test HTML File",
                Result = "<p>Hello workd!</p>",
                ExpirationDate = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0))
            });

            return simpleRepo;
        }
    }

    // Test Reports
    public class TestReportFile : IReport
    {
        private List<string> _blankErrorList = new List<string>();
        private Guid _id;
        private Guid? _sid;
        private UnitOfDataPersistenceWork _repo;
        private string _reportName;

        public TestReportFile(UnitOfDataPersistenceWork repository, string name, ReportIndexEntity results)
        {
            _repo = repository;
            _reportName = name;
            Guid? _newSid = new Guid("2319a829-8df7-4ad7-86a1-00dceb1fadaa");

            if (results == null)
            {
                _id = new Guid("344b305f-ba15-4dfe-bb00-ac7b7de84d3c");
                _sid = _newSid;
            }
            else
            {
                _id = results.Id;
                _sid = results.SimulationID == null ? _newSid : results.SimulationID;
            }
        }

        public Guid ID { get => _id; set { } }
        public Guid? SimulationID { get => _sid; set { } }
        public string Results { get => $"C:\\fakepath\\filename.xlsx"; set { } }

        public ReportType Type => ReportType.File;

        public string ReportTypeName => _reportName;

        public List<string> Errors => _blankErrorList;

        public bool IsComplete => true;

        public string Status => "Report finished running";

        public Task Run(string parameters) => throw new NotImplementedException();
    }

    public class TestHTMLFile : IReport
    {
        private List<string> _blankErrorList = new List<string>();
        private Guid _id;
        private Guid? _sid = null;
        private UnitOfDataPersistenceWork _repo;
        private string _reportName;

        public TestHTMLFile(UnitOfDataPersistenceWork repository, string name, ReportIndexEntity results)
        {
            _repo = repository;
            _reportName = name;

            if (results == null)
            {
                _id = new Guid("d1999649-36ad-4e33-b7c2-e2afbea9b5fa");
            }
            else
            {
                _id = results.Id;
                _sid = results.SimulationID == null ? null : results.SimulationID;
            }
        }

        public Guid ID { get => _id; set { } }
        public Guid? SimulationID { get => _sid; set { } }
        public string Results { get => $"<p>Hello, World!</p>"; set { } }

        public ReportType Type => ReportType.HTML;

        public string ReportTypeName => _reportName;

        public List<string> Errors => _blankErrorList;

        public bool IsComplete => true;

        public string Status => "Report finished running";

        public Task Run(string parameters) => throw new NotImplementedException();
    }

    public class TestBadReport : IReport
    {
        private List<string> _blankErrorList = new List<string>();
        private Guid _id = new Guid("d1999649-36ad-4e33-b7c2-e2afbea9b5fa");
        private UnitOfDataPersistenceWork _repo;
        private string _reportName;

        public TestBadReport(UnitOfDataPersistenceWork repository)
        {
            _repo = repository;
            _reportName = String.Empty;
        }

        public Guid ID { get => _id; set { } }
        public Guid? SimulationID { get => null; set { } }
        public string Results { get => $"<p>Hello, World!</p>"; set { } }

        public ReportType Type => ReportType.HTML;

        public string ReportTypeName => _reportName;

        public List<string> Errors => _blankErrorList;

        public bool IsComplete => true;

        public string Status => "Report finished running";

        public Task Run(string parameters) => throw new NotImplementedException();
    }
}
