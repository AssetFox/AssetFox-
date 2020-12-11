using System;

namespace AppliedResearchAssociates.iAM.Domains
{
    public class WeakEntity
    {
        /// <summary>
        ///     Kind of like a DDD entity, but the <see cref="Id"/> property here is NOT used to
        ///     determine runtime object identity. This <see cref="Id"/> property is basically like an
        ///     extra "name". If we used "normal" entity semantics, we'd have to refactor either (a)
        ///     tons of identity/hashcode-dependent implementation details or (b) tons of creational
        ///     logic, due to the way domain object creation is commonly mediated by "adder" methods and
        ///     "owner object" initialization.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
