using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CalculatedAttributes
{
    public class CalculatedAttributeRepositoryTests
    {
        [Fact]
        public void SuccessfullyPullsDataFromRepository()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void HandlesEmptyRepository()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsLibraryToRepository()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdatesRepositoryWithExistingLibrary()
        {
            // TODO:  Ensure that calculated attribute changes are being reflected
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsAttributeListToExistingLibrary()
        {
            // TODO:  Ensure existing attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdatesExistingCalculatedAttributeInLibrary()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void RemovesCaclulatedAttributesInLibrary()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpsertHandlesNoLibraryFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteCalculatedAttributeHandlesNoLibraryFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfullyDeletesLibrary()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteLibraryHandlesNoLibraryFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfulyGetScenarioAttributes()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void HandlesNoScenarioAttributes()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsNewScenarioAttributes()
        {
            // TODO:  Ensure existing attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateExistingScenarioAttributes()
        {
            // TODO:  Ensure existing, non-modified attributes are preserved
            throw new NotImplementedException();
        }

        [Fact]
        public void UpsertScenarioCalculatedAttributesHandlesNoScenarioFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfullyDeleteScenarioCalculatedAttribute()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteScenarioCalculatedAttributeHandlesAttributeNotFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void DeleteScenarioCalculatedAttributeHandlesScenarioNotFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void SuccessfullyClearsCalculatedAttributesFromScenario()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ClearHandlesScenarioNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
