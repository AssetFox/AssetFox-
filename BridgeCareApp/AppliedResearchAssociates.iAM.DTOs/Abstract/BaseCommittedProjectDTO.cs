using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public abstract class BaseCommittedProjectDTO : BaseDTO
    {
        /// <summary>
        /// The simulation that the committed project will be applied to
        /// </summary>
        public Guid SimulationId { get; set; }

        /// <summary>
        /// The budget in the simulation that this committed project uses to fund its work
        /// </summary>
        /// <remarks>
        /// May be null when the project is not using funds that have been made available to
        /// the simulation.  For example, when a capacity adding project replaces an asset.
        /// </remarks>
        public Guid? ScenarioBudgetId { get; set; }

        /// <summary>
        /// Stores specific location keys (i.e., asset identifiers, routes, etc.)
        /// </summary>
        public Dictionary<string, string> LocationKeys { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The cost to apply the project
        /// </summary>
        /// <remarks>
        /// May be 0 when costs are not using the funding available under the investment
        /// parameters of the scenario
        /// </remarks>
        public double Cost { get; set; }

        /// <summary>
        /// The year in which the committed project will be applied (YYYY)
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Verifies the LocationsKeys provided result in a valid location for the network type
        /// </summary>
        /// <returns>
        /// An indication if the LocationKeys are valid
        /// </returns>
        public abstract bool VerifyLocation();
    }
}
