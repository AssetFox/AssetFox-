using System;
using System.Runtime.Serialization;

namespace AppliedResearchAssociates.iAM
{
    [Serializable]
    public class InvalidAttributeUpsertException : Exception
    {
        public InvalidAttributeUpsertException(string message) : base(message)
        {
        }
    }
}
