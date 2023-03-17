using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using HotChocolate;
using HotChocolate.Data;

namespace BridgeCareCore.GraphQL
{
    public class Query
    {
        //public MessageInfo GetMessage =>
        //    new MessageInfo
        //    {
        //        Message = "This is a test",
        //        Type = "Information"
        //    };

        [UseFiltering]
        [UseSorting]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork) =>
            _unitOfWork.SimulationRepo.GetAllScenario();
    }

    #region Testing
    //public class MessageInfo
    //{
    //    public string Message { get; set; }
    //    public string Type { get; set; }
    //}
    #endregion
}
