using System.Collections;

namespace AppliedResearchAssociates.iAM.Analysis.Testing.CharacterizationTesting;

public class EnumValues<T> : IEnumerable<object[]> where T : struct, Enum
{
    public IEnumerator<object[]> GetEnumerator() =>
        Enum.GetValues<T>()
        .Select(value => new object[] { value })
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
