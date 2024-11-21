using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public void CheckUserSimulationCancelAnalysisAuthorization(Guid simulationId, string userName, bool checkSimulationAccess);

        public bool RequirePermittedCheck();

        public void CheckIfAdminOrOwner(Guid owner, Guid userId);
        public void CheckUserLibraryDeleteAuthorization(LibraryUserAccessModel libraryAccess, Guid userId);
        public void CheckUserLibraryRecreateAuthorization(LibraryUserAccessModel accessModel, Guid userId);
        bool CanModifyAccessLevels(LibraryUserAccessModel accessModel, Guid userId);
        void CheckAccessModifyValidity(List<LibraryUserDTO> currentUsers, List<LibraryUserDTO> proposedUsers, Guid userId);
        void CheckGetLibraryUsersValidity(LibraryUserAccessModel accessModel, Guid userId);
        void CheckUserLibraryModifyAuthorization(LibraryUserAccessModel accessModel, Guid userId);
    }
}
