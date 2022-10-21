﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public bool RequirePermittedCheck();

        public void ObsoleteCheckUserLibraryModifyAuthorization(Guid owner, Guid userId);
        public void CheckUserLibraryModifyAuthorization(LibraryUserAccessModel existingLibraryAccess, Guid userId);
        public void CheckUserLibraryDeleteAuthorization(LibraryUserAccessModel libraryAccess, Guid userId);
        public void CheckUserLibraryRecreateAuthorization(LibraryUserAccessModel accessModel, Guid userId);
        bool CanModifyAccessLevels(LibraryUserAccessModel accessModel, Guid userId);
    }
}
