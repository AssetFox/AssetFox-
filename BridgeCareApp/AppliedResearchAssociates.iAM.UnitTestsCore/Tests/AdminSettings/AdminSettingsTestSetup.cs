using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AdminSettingsTestSetup
    {
        public static void SetupBamsAdminSettings(IUnitOfWork unitOfWork, string networkName, string keyFields, string rawKeyFields)
        {
            unitOfWork.AdminSettingsRepo.SetPrimaryNetwork(networkName);
            unitOfWork.AdminSettingsRepo.SetRawDataNetwork(networkName);
            unitOfWork.AdminSettingsRepo.SetInventoryReports("BAMSInventoryLookup(P)");
            unitOfWork.AdminSettingsRepo.SetKeyFields(keyFields);
            unitOfWork.AdminSettingsRepo.SetRawDataKeyFields(rawKeyFields);
        }
    }
}
