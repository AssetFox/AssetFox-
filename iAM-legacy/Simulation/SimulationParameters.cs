namespace Simulation
{
    public class SimulationParameters
    {
        public SimulationParameters(
            string simulationName,
            string networkName,
            int simulationId,
            int networkId,
            string connectionString,
            bool isApiCall,
            string sqlConnection = "")
        {
            SimulationName = simulationName;
            NetworkName = networkName;
            SimulationId = simulationId;
            NetworkId = networkId;
            ConnectionString = connectionString;
            IsApiCall = isApiCall;
            SQLConnection = sqlConnection;
        }

        public string ConnectionString { get; }

        public bool IsApiCall { get; }

        public int NetworkId { get; }

        public string NetworkName { get; }

        public int SimulationId { get; }

        public string SimulationName { get; }

        public string SQLConnection { get; }
    }
}
