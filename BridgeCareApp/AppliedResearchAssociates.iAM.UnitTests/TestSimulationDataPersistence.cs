using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAccess;
using NUnit.Framework;
using Simulation;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestFixture]
    public class TestSimulationDataPersistence
    {
        // using local copy of DbBackup
        private static string ConnectionString =
            "data source=RMD-PPATORN2-LT\\SQLSERVER2014;initial catalog=DbBackup;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework";
        // there is just the one network with id 13
        private static int NetworkId = 13;
        // JML Run District 8
        private static int SimulationId = 1171;

        [Test]
        public void CreateDataAccessorStandAloneSimulation()
        {
            // create and open a sql server connection
            var connection = new SqlConnection(ConnectionString);
            connection.Open();

            // create an instance of the DataAccessor
            var accessor = new DataAccessor(connection, null);

            // get a stand alone simulation
            var simulation = accessor.GetStandAloneSimulation(NetworkId, SimulationId);

            // close the sql server connection
            connection.Close();

            // assert simulation is not null
            Assert.IsNotNull(simulation);

            // assert that simulation is of type Simulation
            Assert.IsInstanceOf<Simulation>(simulation);
        }
    }
}
