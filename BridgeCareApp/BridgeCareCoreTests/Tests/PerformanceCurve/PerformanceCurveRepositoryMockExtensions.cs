using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class PerformanceCurveRepositoryMockExtensions
    {
        public static void SetupGetPerformanceCurveLibrary(this Mock<IPerformanceCurveRepository> mock, PerformanceCurveLibraryDTO library)
        {
            mock.Setup(m => m.GetPerformanceCurveLibrary(library.Id)).Returns(library);
        }

        public static void SetupGetPerformanceCurvesForLibrary(this Mock<IPerformanceCurveRepository> mock, Guid libraryId, List<PerformanceCurveDTO> curves)
        {
            mock.Setup(m => m.GetPerformanceCurvesForLibrary(libraryId)).Returns(curves);
        }

        public static List<PerformanceCurveDTO> GetUpsertedOrDeletedPerformanceCurves(this Mock<IPerformanceCurveRepository> mock, Guid libraryId)
        {
            var invocations = mock.Invocations.Where(i => i.Method.Name == nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurves));
            var invocationsForLibraryId = invocations.Where(i => i.Arguments[1].ToString() == libraryId.ToString()).ToList();
            var theInvocation = invocationsForLibraryId.Single();
            return (List<PerformanceCurveDTO>)theInvocation.Arguments[0];
        }

        public static void SetupUpsertOrDeleteScenarioPerformanceCurvesThrows(
            this Mock<IPerformanceCurveRepository> mock,
            string exceptionMessage)
        {
            var exception = new Exception(exceptionMessage);
            mock.Setup(m => m.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(
                It.IsAny<List<PerformanceCurveDTO>>(),
                It.IsAny<Guid>()))
                .Throws(exception);
        }
    }
}
