using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    [Serializable]
    public class InvalidAttributeException : Exception
    {
        public InvalidAttributeException(string message) : base(message)
        {
        }
    }
}
