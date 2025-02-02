﻿using System;
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
        /// Project source associated with the committed project
        /// </summary>
        public ProjectSourceDTO ProjectSource { get; set; }

        /// <summary>
        /// Project Id associated with the committed project
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// The number of years until any other treatment may be performed
        /// </summary>
        public int ShadowForAnyTreatment { get; set; }

        /// <summary>
        /// The number of years until the same treatment may be performed
        /// </summary>
        public int ShadowForSameTreatment { get; set; }

        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Verifies the LocationsKeys provided result in a valid location for the network type
        /// </summary>
        /// <param name="networkKeyAttribute"></param>
        /// <returns>
        /// An indication if the LocationKeys are valid
        /// </returns>
        public abstract bool VerifyLocation(string networkKeyAttribute);
        /// <summary>
        /// The Accept method is a key component of the Visitor pattern, enabling encapsulation of operations on objects without altering the objects themselves. 
        /// In this superclass (`BaseCommittedProjectDTO`), the Accept method provides a mechanism for external entities (visitors) to perform specific operations 
        /// on its instances and those of its subclasses. The actual operation executed is determined by the visitor's implementation.
        /// Refer to implementations of the <see cref="IBaseCommittedProjectDtoVisitor{THelper, TOutput}"/> interface for the specific operations that can be performed.
        /// </summary>
        /// <typeparam name="TOutput">Return type of the operation.</typeparam>
        /// <typeparam name="THelper">Auxiliary data or context for the operation.</typeparam>
        /// <param name="visitor">The visitor defining the set of operations.</param>
        /// <param name="helper">Contextual data required for the operation.</param>
        /// <returns>Result of the operation executed by the visitor.</returns>
        public abstract TOutput Accept<TOutput, THelper>(IBaseCommittedProjectDtoVisitor<THelper, TOutput> visitor, THelper helper);
    }
}
