using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreateScenarioPerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId);

        void GetScenarioPerformanceCurves(Simulation simulation, Dictionary<Guid, string> attributeNameLookupDictionary);

        List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibraries();
        List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibrariesNoPerformanceCurves();

        void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto);

        void UpsertOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId);

        void DeletePerformanceCurveLibrary(Guid libraryId);

        List<PerformanceCurveDTO> GetScenarioPerformanceCurves(Guid simulationId);

        void UpsertOrDeleteScenarioPerformanceCurves(List<PerformanceCurveDTO> scenarioPerformanceCurves, Guid simulationId);

        public List<PerformanceCurveDTO> GetPerformanceCurvesForLibrary(Guid performanceCurveLibraryId);

        public PerformanceCurveLibraryDTO GetPerformanceCurveLibrary(Guid performanceCurveLibraryId);

        public List<PerformanceCurveDTO> GetScenarioPerformanceCurvesOrderedById(Guid simulationId);

        public List<PerformanceCurveDTO> GetPerformanceCurvesForLibraryOrderedById(Guid performanceCurveLibraryId);

        public List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibrariesNoChildrenAccessibleToUser(Guid guid);

        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid performanceCurveLibraryId, IList<LibraryUserDTO> libraryUsers);

        List<LibraryUserDTO> GetLibraryUsers(Guid performanceCurveLibraryId);

        List<LibraryUserDTO> GetAccessForUser(Guid performanceCurveLibraryId, Guid userId);
    }
}
