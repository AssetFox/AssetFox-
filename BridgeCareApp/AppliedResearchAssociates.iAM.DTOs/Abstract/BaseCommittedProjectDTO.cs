using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    /// <summary>
    /// A base project DTO class that all projects derive from.
    /// </summary>
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
        /// Name of the treatment being applied as part of this committed project
        /// </summary>
        /// <remarks>
        /// This treatment does NOT need to exist in a library
        /// </remarks>
        public string Treatment { get; set; }

        /// <summary>
        /// FHWA Category for the treatment
        /// </summary>
        /// <remarks>
        /// This is a fixed list
        /// </remarks>
        public TreatmentCategory Category { get; set; }

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
        /// The number of years until any other treatment may be performed
        /// </summary>
        public int ShadowForAnyTreatment { get; set; }

        /// <summary>
        /// The number of years until the same treatment may be performed
        /// </summary>
        public int ShadowForSameTreatment { get; set; }

        /// <summary>
        /// Verifies the LocationsKeys provided result in a valid location for the network type
        /// </summary>
        /// <param name="networkKeyAttribute"></param>
        /// <returns>
        /// An indication if the LocationKeys are valid
        /// </returns>
        public abstract bool VerifyLocation(string networkKeyAttribute);

        public abstract TOutput Accept<TOutput, THelper>(IBaseCommittedProjectDtoVisitor<THelper, TOutput> visitor, THelper helper);

    }
}
