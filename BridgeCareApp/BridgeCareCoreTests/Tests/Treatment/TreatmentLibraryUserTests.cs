using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public class TreatmentLibraryUserTests
    {
        [Fact]
        public async Task GetUsersOfLibrary_RequesterIsAdmin_Gets()
        {
            var user1 = UserDtos.Admin();
            var user2 = UserDtos.Dbe();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
            var treatmentRepo = TreatmentRepositoryMocks.New();
            var treatmentLibraryId = Guid.NewGuid();
            var ownerDto = new LibraryUserDTO
            {
                UserId = user2.Id,
                AccessLevel = LibraryAccessLevel.Owner,
            };
            var userDtos = new List<LibraryUserDTO> { ownerDto };
            treatmentRepo.Setup(br => br.GetLibraryUsers(treatmentLibraryId)).Returns(userDtos);
            var treatmentLibraryDto = new TreatmentLibraryDTO { Id = treatmentLibraryId };
            var treatmentLibraryDtos = new List<TreatmentLibraryDTO> { treatmentLibraryDto };
            treatmentRepo.SetupGetLibraryAccess(treatmentLibraryId, user1.Id, LibraryAccessLevel.Read);
            var controller = TreatmentControllerSetup.CreateAdminController(unitOfWork);
            unitOfWork.SetupTreatmentRepo(treatmentRepo);

            var result = await controller.GetTreatmentLibraryUsers(treatmentLibraryId);

            ActionResultAssertions.OkObject(result);
            var value = (result as OkObjectResult).Value;
            Assert.NotEqual(userDtos, value);
        }
    }
}
