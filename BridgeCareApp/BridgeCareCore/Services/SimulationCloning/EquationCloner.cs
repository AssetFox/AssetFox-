﻿using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace BridgeCareCore.Services.SimulationCloning
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
