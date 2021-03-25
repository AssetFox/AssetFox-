using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class PennDOTAssetDataRepositoryTests
    {

        [Fact]
        public void GeneratesKeyPropertiesDictionaryWithNumericKey()
        {

        }

        [Fact]
        public void GeneratesKeyPropertiesDictionaryWithStringKey()
        {

        }

        [Fact]
        public void HandlesMissingKeyInAttributeRepository()
        {

        }

        [Fact]
        public void HandlesNoDataForExistingAttribute()
        {

        }

        [Fact]
        public void ReturnsSegmeentData()
        {

        }

        [Fact]
        public void HandlesUnmatchedKey()
        {

        }

        [Fact]
        public void HandlesNoSegmentFound()
        {
            // Should the system also remove the asset from KeyProperties if not found?  I think so.
        }

        [Fact]
        public void HandlesOnlyTextAttributes()
        {

        }

        [Fact]
        public void HandlesOnlyNumericAttributes()
        {

        }
    }
}
