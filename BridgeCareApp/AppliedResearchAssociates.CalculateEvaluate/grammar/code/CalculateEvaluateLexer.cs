//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from CalculateEvaluateLexer.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace AppliedResearchAssociates.CalculateEvaluate {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class CalculateEvaluateLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		WHITESPACE=1, AND=2, OR=3, LEFT_PAREN=4, RIGHT_PAREN=5, TIMES=6, DIVIDED_BY=7, 
		PLUS=8, MINUS=9, EQUAL=10, NOT_EQUAL=11, LESS_THAN=12, LESS_THAN_OR_EQUAL=13, 
		GREATER_THAN_OR_EQUAL=14, GREATER_THAN=15, COMMA=16, LEFT_BRACKET=17, 
		RIGHT_BRACKET=18, IDENTIFIER=19, NUMBER=20, LITERAL_OPENING_DELIMITER_1=21, 
		LITERAL_OPENING_DELIMITER_2=22, LITERAL_CLOSING_DELIMITER_1=23, LITERAL_CONTENT_1=24, 
		LITERAL_CLOSING_DELIMITER_2=25, LITERAL_CONTENT_2=26;
	public const int
		LITERAL_MODE_1=1, LITERAL_MODE_2=2;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "LITERAL_MODE_1", "LITERAL_MODE_2"
	};

	public static readonly string[] ruleNames = {
		"WHITESPACE", "AND", "OR", "LEFT_PAREN", "RIGHT_PAREN", "TIMES", "DIVIDED_BY", 
		"PLUS", "MINUS", "EQUAL", "NOT_EQUAL", "LESS_THAN", "LESS_THAN_OR_EQUAL", 
		"GREATER_THAN_OR_EQUAL", "GREATER_THAN", "COMMA", "LEFT_BRACKET", "RIGHT_BRACKET", 
		"IDENTIFIER", "NUMBER", "LETTER", "DIGIT", "MANTISSA_PART", "EXPONENT_PART", 
		"DECIMAL_PART", "NATURAL_NUMBER", "LITERAL_OPENING_DELIMITER_1", "LITERAL_OPENING_DELIMITER_2", 
		"LITERAL_DELIMITER_1", "LITERAL_DELIMITER_2", "LITERAL_CLOSING_DELIMITER_1", 
		"LITERAL_CONTENT_1", "LITERAL_CLOSING_DELIMITER_2", "LITERAL_CONTENT_2"
	};


	public CalculateEvaluateLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public CalculateEvaluateLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, null, null, "'('", "')'", "'*'", "'/'", "'+'", "'-'", "'='", 
		"'<>'", "'<'", "'<='", "'>='", "'>'", "','", "'['", "']'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "WHITESPACE", "AND", "OR", "LEFT_PAREN", "RIGHT_PAREN", "TIMES", 
		"DIVIDED_BY", "PLUS", "MINUS", "EQUAL", "NOT_EQUAL", "LESS_THAN", "LESS_THAN_OR_EQUAL", 
		"GREATER_THAN_OR_EQUAL", "GREATER_THAN", "COMMA", "LEFT_BRACKET", "RIGHT_BRACKET", 
		"IDENTIFIER", "NUMBER", "LITERAL_OPENING_DELIMITER_1", "LITERAL_OPENING_DELIMITER_2", 
		"LITERAL_CLOSING_DELIMITER_1", "LITERAL_CONTENT_1", "LITERAL_CLOSING_DELIMITER_2", 
		"LITERAL_CONTENT_2"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "CalculateEvaluateLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static CalculateEvaluateLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x1C', '\xBB', '\b', '\x1', '\b', '\x1', '\b', '\x1', 
		'\x4', '\x2', '\t', '\x2', '\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', 
		'\x4', '\x4', '\x5', '\t', '\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', 
		'\t', '\a', '\x4', '\b', '\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', 
		'\t', '\n', '\x4', '\v', '\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', 
		'\t', '\r', '\x4', '\xE', '\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', 
		'\x10', '\t', '\x10', '\x4', '\x11', '\t', '\x11', '\x4', '\x12', '\t', 
		'\x12', '\x4', '\x13', '\t', '\x13', '\x4', '\x14', '\t', '\x14', '\x4', 
		'\x15', '\t', '\x15', '\x4', '\x16', '\t', '\x16', '\x4', '\x17', '\t', 
		'\x17', '\x4', '\x18', '\t', '\x18', '\x4', '\x19', '\t', '\x19', '\x4', 
		'\x1A', '\t', '\x1A', '\x4', '\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', 
		'\x1C', '\x4', '\x1D', '\t', '\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', 
		'\x1F', '\t', '\x1F', '\x4', ' ', '\t', ' ', '\x4', '!', '\t', '!', '\x4', 
		'\"', '\t', '\"', '\x4', '#', '\t', '#', '\x3', '\x2', '\x6', '\x2', 'K', 
		'\n', '\x2', '\r', '\x2', '\xE', '\x2', 'L', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', '\b', '\x3', '\b', '\x3', 
		'\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', 
		'\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', '\x3', 
		'\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', 
		'\xF', '\x3', '\x10', '\x3', '\x10', '\x3', '\x11', '\x3', '\x11', '\x3', 
		'\x12', '\x3', '\x12', '\x3', '\x13', '\x3', '\x13', '\x3', '\x14', '\x3', 
		'\x14', '\x3', '\x14', '\a', '\x14', '|', '\n', '\x14', '\f', '\x14', 
		'\xE', '\x14', '\x7F', '\v', '\x14', '\x3', '\x15', '\x3', '\x15', '\x5', 
		'\x15', '\x83', '\n', '\x15', '\x3', '\x16', '\x3', '\x16', '\x3', '\x17', 
		'\x3', '\x17', '\x3', '\x18', '\x3', '\x18', '\x5', '\x18', '\x8B', '\n', 
		'\x18', '\x3', '\x18', '\x5', '\x18', '\x8E', '\n', '\x18', '\x3', '\x19', 
		'\x3', '\x19', '\x5', '\x19', '\x92', '\n', '\x19', '\x3', '\x19', '\x3', 
		'\x19', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1B', '\x6', 
		'\x1B', '\x9A', '\n', '\x1B', '\r', '\x1B', '\xE', '\x1B', '\x9B', '\x3', 
		'\x1C', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1D', '\x3', 
		'\x1D', '\x3', '\x1D', '\x3', '\x1D', '\x3', '\x1E', '\x3', '\x1E', '\x3', 
		'\x1F', '\x3', '\x1F', '\x3', ' ', '\x3', ' ', '\x3', ' ', '\x3', ' ', 
		'\x3', '!', '\x6', '!', '\xAF', '\n', '!', '\r', '!', '\xE', '!', '\xB0', 
		'\x3', '\"', '\x3', '\"', '\x3', '\"', '\x3', '\"', '\x3', '#', '\x6', 
		'#', '\xB8', '\n', '#', '\r', '#', '\xE', '#', '\xB9', '\x2', '\x2', '$', 
		'\x5', '\x3', '\a', '\x4', '\t', '\x5', '\v', '\x6', '\r', '\a', '\xF', 
		'\b', '\x11', '\t', '\x13', '\n', '\x15', '\v', '\x17', '\f', '\x19', 
		'\r', '\x1B', '\xE', '\x1D', '\xF', '\x1F', '\x10', '!', '\x11', '#', 
		'\x12', '%', '\x13', '\'', '\x14', ')', '\x15', '+', '\x16', '-', '\x2', 
		'/', '\x2', '\x31', '\x2', '\x33', '\x2', '\x35', '\x2', '\x37', '\x2', 
		'\x39', '\x17', ';', '\x18', '=', '\x2', '?', '\x2', '\x41', '\x19', '\x43', 
		'\x1A', '\x45', '\x1B', 'G', '\x1C', '\x5', '\x2', '\x3', '\x4', '\xE', 
		'\x5', '\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', '\x4', '\x2', '\x43', 
		'\x43', '\x63', '\x63', '\x4', '\x2', 'P', 'P', 'p', 'p', '\x4', '\x2', 
		'\x46', '\x46', '\x66', '\x66', '\x4', '\x2', 'Q', 'Q', 'q', 'q', '\x4', 
		'\x2', 'T', 'T', 't', 't', '\x5', '\x2', '\x43', '\\', '\x61', '\x61', 
		'\x63', '|', '\x3', '\x2', '\x32', ';', '\x4', '\x2', 'G', 'G', 'g', 'g', 
		'\x4', '\x2', '-', '-', '/', '/', '\x3', '\x2', '~', '~', '\x3', '\x2', 
		')', ')', '\x2', '\xBA', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\a', '\x3', '\x2', '\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\v', '\x3', '\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\xF', '\x3', '\x2', '\x2', '\x2', '\x2', '\x11', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x13', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x15', '\x3', '\x2', '\x2', '\x2', '\x2', '\x17', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x19', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1B', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x1F', '\x3', '\x2', '\x2', '\x2', '\x2', '!', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '#', '\x3', '\x2', '\x2', '\x2', '\x2', '%', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\'', '\x3', '\x2', '\x2', '\x2', '\x2', ')', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '+', '\x3', '\x2', '\x2', '\x2', '\x2', '\x39', '\x3', 
		'\x2', '\x2', '\x2', '\x2', ';', '\x3', '\x2', '\x2', '\x2', '\x3', '\x41', 
		'\x3', '\x2', '\x2', '\x2', '\x3', '\x43', '\x3', '\x2', '\x2', '\x2', 
		'\x4', '\x45', '\x3', '\x2', '\x2', '\x2', '\x4', 'G', '\x3', '\x2', '\x2', 
		'\x2', '\x5', 'J', '\x3', '\x2', '\x2', '\x2', '\a', 'P', '\x3', '\x2', 
		'\x2', '\x2', '\t', 'T', '\x3', '\x2', '\x2', '\x2', '\v', 'W', '\x3', 
		'\x2', '\x2', '\x2', '\r', 'Y', '\x3', '\x2', '\x2', '\x2', '\xF', '[', 
		'\x3', '\x2', '\x2', '\x2', '\x11', ']', '\x3', '\x2', '\x2', '\x2', '\x13', 
		'_', '\x3', '\x2', '\x2', '\x2', '\x15', '\x61', '\x3', '\x2', '\x2', 
		'\x2', '\x17', '\x63', '\x3', '\x2', '\x2', '\x2', '\x19', '\x65', '\x3', 
		'\x2', '\x2', '\x2', '\x1B', 'h', '\x3', '\x2', '\x2', '\x2', '\x1D', 
		'j', '\x3', '\x2', '\x2', '\x2', '\x1F', 'm', '\x3', '\x2', '\x2', '\x2', 
		'!', 'p', '\x3', '\x2', '\x2', '\x2', '#', 'r', '\x3', '\x2', '\x2', '\x2', 
		'%', 't', '\x3', '\x2', '\x2', '\x2', '\'', 'v', '\x3', '\x2', '\x2', 
		'\x2', ')', 'x', '\x3', '\x2', '\x2', '\x2', '+', '\x80', '\x3', '\x2', 
		'\x2', '\x2', '-', '\x84', '\x3', '\x2', '\x2', '\x2', '/', '\x86', '\x3', 
		'\x2', '\x2', '\x2', '\x31', '\x8D', '\x3', '\x2', '\x2', '\x2', '\x33', 
		'\x8F', '\x3', '\x2', '\x2', '\x2', '\x35', '\x95', '\x3', '\x2', '\x2', 
		'\x2', '\x37', '\x99', '\x3', '\x2', '\x2', '\x2', '\x39', '\x9D', '\x3', 
		'\x2', '\x2', '\x2', ';', '\xA1', '\x3', '\x2', '\x2', '\x2', '=', '\xA5', 
		'\x3', '\x2', '\x2', '\x2', '?', '\xA7', '\x3', '\x2', '\x2', '\x2', '\x41', 
		'\xA9', '\x3', '\x2', '\x2', '\x2', '\x43', '\xAE', '\x3', '\x2', '\x2', 
		'\x2', '\x45', '\xB2', '\x3', '\x2', '\x2', '\x2', 'G', '\xB7', '\x3', 
		'\x2', '\x2', '\x2', 'I', 'K', '\t', '\x2', '\x2', '\x2', 'J', 'I', '\x3', 
		'\x2', '\x2', '\x2', 'K', 'L', '\x3', '\x2', '\x2', '\x2', 'L', 'J', '\x3', 
		'\x2', '\x2', '\x2', 'L', 'M', '\x3', '\x2', '\x2', '\x2', 'M', 'N', '\x3', 
		'\x2', '\x2', '\x2', 'N', 'O', '\b', '\x2', '\x2', '\x2', 'O', '\x6', 
		'\x3', '\x2', '\x2', '\x2', 'P', 'Q', '\t', '\x3', '\x2', '\x2', 'Q', 
		'R', '\t', '\x4', '\x2', '\x2', 'R', 'S', '\t', '\x5', '\x2', '\x2', 'S', 
		'\b', '\x3', '\x2', '\x2', '\x2', 'T', 'U', '\t', '\x6', '\x2', '\x2', 
		'U', 'V', '\t', '\a', '\x2', '\x2', 'V', '\n', '\x3', '\x2', '\x2', '\x2', 
		'W', 'X', '\a', '*', '\x2', '\x2', 'X', '\f', '\x3', '\x2', '\x2', '\x2', 
		'Y', 'Z', '\a', '+', '\x2', '\x2', 'Z', '\xE', '\x3', '\x2', '\x2', '\x2', 
		'[', '\\', '\a', ',', '\x2', '\x2', '\\', '\x10', '\x3', '\x2', '\x2', 
		'\x2', ']', '^', '\a', '\x31', '\x2', '\x2', '^', '\x12', '\x3', '\x2', 
		'\x2', '\x2', '_', '`', '\a', '-', '\x2', '\x2', '`', '\x14', '\x3', '\x2', 
		'\x2', '\x2', '\x61', '\x62', '\a', '/', '\x2', '\x2', '\x62', '\x16', 
		'\x3', '\x2', '\x2', '\x2', '\x63', '\x64', '\a', '?', '\x2', '\x2', '\x64', 
		'\x18', '\x3', '\x2', '\x2', '\x2', '\x65', '\x66', '\a', '>', '\x2', 
		'\x2', '\x66', 'g', '\a', '@', '\x2', '\x2', 'g', '\x1A', '\x3', '\x2', 
		'\x2', '\x2', 'h', 'i', '\a', '>', '\x2', '\x2', 'i', '\x1C', '\x3', '\x2', 
		'\x2', '\x2', 'j', 'k', '\a', '>', '\x2', '\x2', 'k', 'l', '\a', '?', 
		'\x2', '\x2', 'l', '\x1E', '\x3', '\x2', '\x2', '\x2', 'm', 'n', '\a', 
		'@', '\x2', '\x2', 'n', 'o', '\a', '?', '\x2', '\x2', 'o', ' ', '\x3', 
		'\x2', '\x2', '\x2', 'p', 'q', '\a', '@', '\x2', '\x2', 'q', '\"', '\x3', 
		'\x2', '\x2', '\x2', 'r', 's', '\a', '.', '\x2', '\x2', 's', '$', '\x3', 
		'\x2', '\x2', '\x2', 't', 'u', '\a', ']', '\x2', '\x2', 'u', '&', '\x3', 
		'\x2', '\x2', '\x2', 'v', 'w', '\a', '_', '\x2', '\x2', 'w', '(', '\x3', 
		'\x2', '\x2', '\x2', 'x', '}', '\x5', '-', '\x16', '\x2', 'y', '|', '\x5', 
		'-', '\x16', '\x2', 'z', '|', '\x5', '/', '\x17', '\x2', '{', 'y', '\x3', 
		'\x2', '\x2', '\x2', '{', 'z', '\x3', '\x2', '\x2', '\x2', '|', '\x7F', 
		'\x3', '\x2', '\x2', '\x2', '}', '{', '\x3', '\x2', '\x2', '\x2', '}', 
		'~', '\x3', '\x2', '\x2', '\x2', '~', '*', '\x3', '\x2', '\x2', '\x2', 
		'\x7F', '}', '\x3', '\x2', '\x2', '\x2', '\x80', '\x82', '\x5', '\x31', 
		'\x18', '\x2', '\x81', '\x83', '\x5', '\x33', '\x19', '\x2', '\x82', '\x81', 
		'\x3', '\x2', '\x2', '\x2', '\x82', '\x83', '\x3', '\x2', '\x2', '\x2', 
		'\x83', ',', '\x3', '\x2', '\x2', '\x2', '\x84', '\x85', '\t', '\b', '\x2', 
		'\x2', '\x85', '.', '\x3', '\x2', '\x2', '\x2', '\x86', '\x87', '\t', 
		'\t', '\x2', '\x2', '\x87', '\x30', '\x3', '\x2', '\x2', '\x2', '\x88', 
		'\x8A', '\x5', '\x37', '\x1B', '\x2', '\x89', '\x8B', '\x5', '\x35', '\x1A', 
		'\x2', '\x8A', '\x89', '\x3', '\x2', '\x2', '\x2', '\x8A', '\x8B', '\x3', 
		'\x2', '\x2', '\x2', '\x8B', '\x8E', '\x3', '\x2', '\x2', '\x2', '\x8C', 
		'\x8E', '\x5', '\x35', '\x1A', '\x2', '\x8D', '\x88', '\x3', '\x2', '\x2', 
		'\x2', '\x8D', '\x8C', '\x3', '\x2', '\x2', '\x2', '\x8E', '\x32', '\x3', 
		'\x2', '\x2', '\x2', '\x8F', '\x91', '\t', '\n', '\x2', '\x2', '\x90', 
		'\x92', '\t', '\v', '\x2', '\x2', '\x91', '\x90', '\x3', '\x2', '\x2', 
		'\x2', '\x91', '\x92', '\x3', '\x2', '\x2', '\x2', '\x92', '\x93', '\x3', 
		'\x2', '\x2', '\x2', '\x93', '\x94', '\x5', '\x37', '\x1B', '\x2', '\x94', 
		'\x34', '\x3', '\x2', '\x2', '\x2', '\x95', '\x96', '\a', '\x30', '\x2', 
		'\x2', '\x96', '\x97', '\x5', '\x37', '\x1B', '\x2', '\x97', '\x36', '\x3', 
		'\x2', '\x2', '\x2', '\x98', '\x9A', '\x5', '/', '\x17', '\x2', '\x99', 
		'\x98', '\x3', '\x2', '\x2', '\x2', '\x9A', '\x9B', '\x3', '\x2', '\x2', 
		'\x2', '\x9B', '\x99', '\x3', '\x2', '\x2', '\x2', '\x9B', '\x9C', '\x3', 
		'\x2', '\x2', '\x2', '\x9C', '\x38', '\x3', '\x2', '\x2', '\x2', '\x9D', 
		'\x9E', '\x5', '=', '\x1E', '\x2', '\x9E', '\x9F', '\x3', '\x2', '\x2', 
		'\x2', '\x9F', '\xA0', '\b', '\x1C', '\x3', '\x2', '\xA0', ':', '\x3', 
		'\x2', '\x2', '\x2', '\xA1', '\xA2', '\x5', '?', '\x1F', '\x2', '\xA2', 
		'\xA3', '\x3', '\x2', '\x2', '\x2', '\xA3', '\xA4', '\b', '\x1D', '\x4', 
		'\x2', '\xA4', '<', '\x3', '\x2', '\x2', '\x2', '\xA5', '\xA6', '\a', 
		'~', '\x2', '\x2', '\xA6', '>', '\x3', '\x2', '\x2', '\x2', '\xA7', '\xA8', 
		'\a', ')', '\x2', '\x2', '\xA8', '@', '\x3', '\x2', '\x2', '\x2', '\xA9', 
		'\xAA', '\x5', '=', '\x1E', '\x2', '\xAA', '\xAB', '\x3', '\x2', '\x2', 
		'\x2', '\xAB', '\xAC', '\b', ' ', '\x5', '\x2', '\xAC', '\x42', '\x3', 
		'\x2', '\x2', '\x2', '\xAD', '\xAF', '\n', '\f', '\x2', '\x2', '\xAE', 
		'\xAD', '\x3', '\x2', '\x2', '\x2', '\xAF', '\xB0', '\x3', '\x2', '\x2', 
		'\x2', '\xB0', '\xAE', '\x3', '\x2', '\x2', '\x2', '\xB0', '\xB1', '\x3', 
		'\x2', '\x2', '\x2', '\xB1', '\x44', '\x3', '\x2', '\x2', '\x2', '\xB2', 
		'\xB3', '\x5', '?', '\x1F', '\x2', '\xB3', '\xB4', '\x3', '\x2', '\x2', 
		'\x2', '\xB4', '\xB5', '\b', '\"', '\x5', '\x2', '\xB5', '\x46', '\x3', 
		'\x2', '\x2', '\x2', '\xB6', '\xB8', '\n', '\r', '\x2', '\x2', '\xB7', 
		'\xB6', '\x3', '\x2', '\x2', '\x2', '\xB8', '\xB9', '\x3', '\x2', '\x2', 
		'\x2', '\xB9', '\xB7', '\x3', '\x2', '\x2', '\x2', '\xB9', '\xBA', '\x3', 
		'\x2', '\x2', '\x2', '\xBA', 'H', '\x3', '\x2', '\x2', '\x2', '\xF', '\x2', 
		'\x3', '\x4', 'L', '{', '}', '\x82', '\x8A', '\x8D', '\x91', '\x9B', '\xB0', 
		'\xB9', '\x6', '\x2', '\x3', '\x2', '\a', '\x3', '\x2', '\a', '\x4', '\x2', 
		'\x6', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace AppliedResearchAssociates.CalculateEvaluate