using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public string ConnectionString { get; }
        public string Command { get; }
        public abstract IEnumerable<IAttributeDatum> GetData<T>();

        public AttributeConnection(string connectionString, string command)
        {
            ConnectionString = connectionString;
            Command = command;
        }
    }
}
