using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class EquationCloner
    {
        internal static EquationDTO Clone(EquationDTO equation, Guid ownerId)
        {
            var clone = new EquationDTO
            {
                Id = equation.Id == Guid.Empty ? Guid.Empty : Guid.NewGuid(),
                Expression = equation.Expression,
            };
            return clone;
        }
        internal static List<EquationDTO> CloneList(IEnumerable<EquationDTO> equations, Guid ownerId)
        {
            var clone = new List<EquationDTO>();
            foreach (var equation in equations)
            {
                var childClone = Clone(equation, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }

    }
}
