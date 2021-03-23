using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Reporting
{
    public class ReportGeneratorTests
    {
        private Dictionary<string, Type> _testReportLibrary;
        private UnitOfDataPersistenceWork _testRepo;
        private DictionaryBasedReportGenerator _generator;

        public ReportGeneratorTests()
        {
            var mockedRepo = new Mock<UnitOfDataPersistenceWork>((new Mock<IConfiguration>()).Object, (new Mock<IAMContext>()).Object);
            _testRepo = mockedRepo.Object;

            _testReportLibrary = new Dictionary<string, Type>();
            _testReportLibrary.Add("Test Report File", typeof(TestReportFile));
            _testReportLibrary.Add("Test HTML File", typeof(TestHTMLFile));
            _testReportLibrary.Add("Bad Report", typeof(TestBadReport));

            _generator = new DictionaryBasedReportGenerator(_testRepo, _testReportLibrary);
        }

        [Fact]
        public async Task GeneratorCanGenerateReportInLibrary()
        {
            string goodReport = "Test Report File";

            // Act
            IReport report = await _generator.Generate(goodReport);

            // Assert
            Assert.Equal(ReportType.File, report.Type);
            Assert.Equal(goodReport, report.ReportTypeName);
            Assert.Equal(0, report.Errors.Count());
        }

        [Fact]
        public async Task GeneratorReturnsFailureReportWhenReportNotInLibrary()
        {
            string badReport = "Some missing report";

            // Act
            IReport report = await _generator.Generate(badReport);

            // Assert
            Assert.Equal(ReportType.HTML, report.Type);
            Assert.Equal("Failure Report", report.ReportTypeName);
            Assert.True(report.Errors.Count() > 0);
        }

        [Fact]
        public async Task GeneeratorReturnsFailureReportWhenReportIsMissingProperConstructor()
        {
            string badReport = "Bad Report";

            // Act
            IReport report = await _generator.Generate(badReport);

            // Assert
            Assert.Equal(ReportType.HTML, report.Type);
            Assert.Equal("Failure Report", report.ReportTypeName);
            Assert.True(report.Errors.Count() > 0);
        }
    }

    // Test Reports
    public class TestReportFile : IReport
    {
        private List<string> _blankErrorList = new List<string>();
        private Guid _id = new Guid("344b305f-ba15-4dfe-bb00-ac7b7de84d3c");
        private Guid _sid = new Guid("2319a829-8df7-4ad7-86a1-00dceb1fadaa");
        private UnitOfDataPersistenceWork _repo;
        private string _reportName;

        public TestReportFile(UnitOfDataPersistenceWork repository, string name)
        {
            _repo = repository;
            _reportName = name;
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
        private Guid _id = new Guid("d1999649-36ad-4e33-b7c2-e2afbea9b5fa");
        private UnitOfDataPersistenceWork _repo;
        private string _reportName;

        public TestHTMLFile(UnitOfDataPersistenceWork repository, string name)
        {
            _repo = repository;
            _reportName = name;
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
