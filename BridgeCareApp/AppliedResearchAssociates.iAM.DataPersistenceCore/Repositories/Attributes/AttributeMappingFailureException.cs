using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    [Serializable]
    internal class AttributeMappingFailureException : Exception
    {
        public AttributeMappingFailureException(string message) : base(message)
        {
        }
    }
}
