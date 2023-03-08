using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes
{
    public static class CalculatedAttributeTestSetup
    {
        private static object DefaultCalculatedAttributeSetupLock = new object();
        private static bool DefaultCalculatedAttributeLibraryHasBeenCreated = false;

        public static void CreateCalculatedAttributeLibrary(UnitOfDataPersistenceWork unitOfWork)
        {
            if (!DefaultCalculatedAttributeLibraryHasBeenCreated)
            {
                lock (DefaultCalculatedAttributeSetupLock)
                {
                    if (!DefaultCalculatedAttributeLibraryHasBeenCreated)
                    {
                        var dto = new CalculatedAttributeLibraryDTO
                        {
                            IsDefault = true,
                            Id = Guid.NewGuid(),
                            Name = "Default Test Calculated Attribute Library",
                            CalculatedAttributes = { },
                        };
                        unitOfWork.CalculatedAttributeRepo.UpsertCalculatedAttributeLibraryAtomically(dto);
                        DefaultCalculatedAttributeLibraryHasBeenCreated = true;
                    }
                }
            }
        }
    }
}
