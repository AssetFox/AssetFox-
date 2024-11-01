using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public sealed class CalculateEvaluateCompiler
    {
        public static IEnumerable<string> NumberConstantNames => CalculateEvaluateCompilerVisitor.NumberConstants.Keys;

        public static IEnumerable<NumberFunctionDescription> NumberFunctionDescriptions => CalculateEvaluateCompilerVisitor.NumberFunctionDescriptions;

        public Dictionary<string, CalculateEvaluateParameterType> ParameterTypes { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Calculator GetCalculator(string expression) => (Calculator)Compile(expression, () => new Calculator());

        public Evaluator GetEvaluator(string expression) => (Evaluator)Compile(expression, () => new Evaluator());

        private static readonly IEqualityComparer<IEnumerable<(int, string)>> CacheComparer = SequenceEqualityComparer.Create(ValueTupleEqualityComparer.Create<int, string>(comparer2: StringComparer.OrdinalIgnoreCase));

        private readonly ConcurrentDictionary<(int, string)[], WeakReference<FinalActor<object>>> Cache = new(CacheComparer);

        private CalculateEvaluateDelegateWrapper<T> Compile<T>(string expression, Func<CalculateEvaluateDelegateWrapper<T>> createDelegateWrapper)
        {
            var input = new AntlrInputStream(expression);
            var lexer = new CalculateEvaluateBailingLexer(input);

            var tokenList = lexer.GetAllTokens();

            var cacheKey = tokenList
                .Where(token => token.Channel != Lexer.Hidden)
                .Select(token => (token.Type, token.Text))
                .ToArray();

            if (Cache.TryGetValue(cacheKey, out var weakReference))
            {
                if (weakReference.TryGetTarget(out var cachedWrapper))
                {
                    return (CalculateEvaluateDelegateWrapper<T>)cachedWrapper.Value;
                }
            }

            var tokenSource = new ListTokenSource(tokenList);
            var tokens = new CommonTokenStream(tokenSource);

            var parser = new CalculateEvaluateBailingParser(tokens);
            parser.Interpreter.PredictionMode = PredictionMode.SLL; // Fast mode. Not as robust as default, but almost always works.

            IParseTree tree;
            try
            {
                tree = parser.root();
            }
            catch (ParseCanceledException)
            {
                tokens.Reset();
                parser.Reset();
                parser.Interpreter.PredictionMode = PredictionMode.LL; // Default, robust mode. Fails only when successful parse is impossible (for ANTLR, anyway).

                try
                {
                    tree = parser.root();
                }
                catch (ParseCanceledException parseCanceled)
                {
                    string message = null;
                    if (parseCanceled.InnerException is RecognitionException recognition)
                    {
                        var (text, line, column) = (
                            recognition.OffendingToken.Text,
                            recognition.OffendingToken.Line,
                            recognition.OffendingToken.Column);

                        message = $"Offending token \"{text}\" at line {line}, column {column}.";
                    }

                    throw new CalculateEvaluateParsingException(message, parseCanceled);
                }
            }

            var visitor = new CalculateEvaluateCompilerVisitor(ParameterTypes);
            var lambdaExpression = (Expression<CalculateEvaluateDelegate<T>>)visitor.Visit(tree);
            var lambda = lambdaExpression.Compile();

            var wrapper = createDelegateWrapper();
            wrapper.Delegate = lambda;
            wrapper.ReferencedParameters = visitor.ReferencedParameters;

            var cacheValue = ((object)wrapper).WithFinalAction(actor => Cache.TryRemove(cacheKey, out _)).GetWeakReference();
            _ = Cache.TryAdd(cacheKey, cacheValue);

            return wrapper;
        }
    }
}
