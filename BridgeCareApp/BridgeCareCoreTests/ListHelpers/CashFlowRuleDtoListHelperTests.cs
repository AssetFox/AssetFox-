﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace BridgeCareCoreTests.ListHelpers
{
    public class CashFlowRuleDtoListHelperTests
    {
        [Fact]
        public void AddModifiedToCashFlowRule_Does()
        {
            var curve = new CashFlowRuleDTO();
            var curves = new List<CashFlowRuleDTO> { curve };
            Assert.False(curve.IsModified);
            TestHelper.UnitOfWork.CashFlowRuleRepo.AddModifiedToScenarioCashFlowRule(curves, true);
            Assert.True(curve.IsModified);
        }


        [Fact]
        public void AddLibraryIdToCashFlowRule_Does()
        {
            var curve = new CashFlowRuleDTO();
            var curves = new List<CashFlowRuleDTO> { curve };
            var libraryId = Guid.NewGuid();

            TestHelper.UnitOfWork.CashFlowRuleRepo.AddLibraryIdToScenarioCashFlowRule(curves, libraryId);

            Assert.Equal(libraryId, curve.LibraryId);
        }

        [Fact]
        public void AddLibraryIdToCashFlowRule_LibraryIdIsNull_NoChange()
        {
            var curve = new CashFlowRuleDTO();
            var curves = new List<CashFlowRuleDTO> { curve };
            var libraryId = Guid.NewGuid();
            curve.LibraryId = libraryId;

            TestHelper.UnitOfWork.CashFlowRuleRepo.AddLibraryIdToScenarioCashFlowRule(curves, null);

            Assert.Equal(libraryId, curve.LibraryId);
        }
    }
}
