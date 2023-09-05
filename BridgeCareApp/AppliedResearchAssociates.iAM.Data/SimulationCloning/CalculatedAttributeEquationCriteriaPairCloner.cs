using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CalculatedAttributeEquationCriteriaPairCloner
    {
        internal static CalculatedAttributeEquationCriteriaPairDTO Clone(CalculatedAttributeEquationCriteriaPairDTO calculatedAttributeEquationCriterionPair,Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(calculatedAttributeEquationCriterionPair.CriteriaLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(calculatedAttributeEquationCriterionPair.Equation, ownerId);
            var clone = new CalculatedAttributeEquationCriteriaPairDTO
            {
                Id = Guid.NewGuid(),
                CriteriaLibrary = cloneCriterionLibrary,
                Equation = cloneEquation,
                
            };
            return clone;
        }

        internal static List<CalculatedAttributeEquationCriteriaPairDTO> CloneList(IEnumerable<CalculatedAttributeEquationCriteriaPairDTO> calculatedAttributeEquationCriterionPairs, Guid ownerId)
        {
            var clone = new List<CalculatedAttributeEquationCriteriaPairDTO>();
            foreach (var calculatedAttributeEquationCriterionPair in calculatedAttributeEquationCriterionPairs)
            {
                var childClone = Clone(calculatedAttributeEquationCriterionPair, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
