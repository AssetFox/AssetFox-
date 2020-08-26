﻿using System.Text.RegularExpressions;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute
    {
        public Attribute(string name)
        {
            if (name == null || !NamePattern.IsMatch(name))
            {
                throw new MalformedInputException($"Invalid name. The valid pattern is \"{PatternStrings.Identifier}\".");
            }

            Name = name;
        }

        public static Regex NamePattern { get; } = new Regex($@"(?>\A{PatternStrings.Identifier}\z)");

        public string Name { get; }
    }

    public class Attribute<T> : Attribute
    {
        public Attribute(string name) : base(name)
        {
        }

        public T DefaultValue { get; set; }
    }
}
