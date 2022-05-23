using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    [Serializable]
    public class InvalidAttributeUpsertException : Exception
    {
        public InvalidAttributeUpsertException(string message) : base(message)
        {
        }
    }
}
