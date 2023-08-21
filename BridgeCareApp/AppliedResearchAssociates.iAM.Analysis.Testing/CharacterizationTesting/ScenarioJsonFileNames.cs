using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Testing.CharacterizationTesting;

public class ScenarioJsonFileNames : IEnumerable<object[]>
{
    public static string FolderPath { get; } = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "CharacterizationTesting",
        "Inputs");

    public IEnumerator<object[]> GetEnumerator() =>
        Directory.EnumerateFiles(FolderPath, "*.json")
        .Select(path => new object[] { Path.GetFileName(path) })
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
