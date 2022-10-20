﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repsitories;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public bool RequirePermittedCheck();

        public void ObsoleteCheckUserLibraryModifyAuthorization(Guid owner, Guid userId);
        public void CheckUserLibraryModifyAuthorization(LibraryAccessModel existingLibraryAccess, Guid userId);
        public void CheckUserLibraryDeleteAuthorization(LibraryAccessModel libraryAccess, Guid userId);
        public void CheckUserLibraryRecreateAuthorization(LibraryAccessModel accessModel, Guid userId);
        bool CanModifyAccessLevels(LibraryAccessModel accessModel, Guid userId);

        public void CheckIfLibraryExists(LibraryAccessModel accessModel);
    }
}
