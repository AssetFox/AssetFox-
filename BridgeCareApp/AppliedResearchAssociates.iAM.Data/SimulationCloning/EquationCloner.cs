using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class EquationCloner
    {
        internal static EquationDTO Clone(EquationDTO equation)
        {
             var clone = new EquationDTO
             {
                 Expression = equation.Expression,
             };
            return clone;
        }

    }
}
