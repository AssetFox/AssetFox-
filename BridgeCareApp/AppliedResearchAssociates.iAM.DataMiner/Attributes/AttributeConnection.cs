using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public Attribute Attribute { get; }
        public string ConnectionString { get; }
        public string Command { get; }
        public abstract IEnumerable<IAttributeDatum> GetData<T>();

        public AttributeConnection(Attribute attribute, string connectionString, string command)
        {
            Attribute = attribute;
            ConnectionString = connectionString;
            Command = command;
        }
    }
}
