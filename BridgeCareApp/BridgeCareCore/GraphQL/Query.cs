using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.GraphQL
{
    public class Query
    {
        public MessageInfo GetMessage =>
            new MessageInfo
            {
                Message = "This is a test",
                Type = "Information"
            };
    }

    #region Testing
    public class MessageInfo
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }
    #endregion
}
