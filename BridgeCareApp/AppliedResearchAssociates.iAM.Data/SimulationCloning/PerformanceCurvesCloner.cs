using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class PerformanceCurvesCloner
    {
        internal static PerformanceCurveDTO Clone(PerformanceCurveDTO performanceCurve, Guid ownerId)
        {

            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(performanceCurve.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(performanceCurve.Equation, ownerId);
            var clone = new PerformanceCurveDTO            
            {
                Id = Guid.NewGuid(),
                IsModified = performanceCurve.IsModified,
                Name = performanceCurve.Name,
                Shift = performanceCurve.Shift,
                Attribute = performanceCurve.Attribute,
                LibraryId = performanceCurve.LibraryId,
                CriterionLibrary = cloneCriterionLibrary,
                Equation = cloneEquation,                
            };
            return clone;
        }
        internal static List<PerformanceCurveDTO> CloneList(IEnumerable<PerformanceCurveDTO> performanceCurves, Guid ownerId)
        {
            var clone = new List<PerformanceCurveDTO>();
            foreach (var performanceCurve in performanceCurves)
            {
                var childClone = Clone(performanceCurve, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
        internal static List<PerformanceCurveDTO> CloneListNullPropagating(IEnumerable<PerformanceCurveDTO> performanceCurves, Guid ownerId)
        {
           if (performanceCurves == null) { return null; }
           return CloneList(performanceCurves, ownerId);
        }
    }
}
