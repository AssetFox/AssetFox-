using System.Text.RegularExpressions;
using Xunit;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class ScratchTests
    {
        [Fact]
        public void WhatHappensWhenParameterIsNotDefined()
        {
            var compiler = new CalculateEvaluateCompiler();
            var e = Assert.Throws<CalculateEvaluateCompilationException>(() => compiler.GetEvaluator("foobar=42"));
        }

        [Fact]
        public void WhatHappensWhenParameterIsParenthesized()
        {
            var compiler = new CalculateEvaluateCompiler();
            var e = Assert.Throws<CalculateEvaluateParsingException>(() => compiler.GetEvaluator("(foobar)=42"));
        }

        [Fact]
        public void QuestionFromMaxxLeach_2022_05_12()
        {
            const string input = @"((([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK1])>|50|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK1])>|50|)) AND ((([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CFLTJNT3])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CFLTJNT3])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CBRKSLB2])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CBRKSLB2])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CBRKSLB3])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CBRKSLB3])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNJNT3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNJNT3])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK2])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK2])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK3])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CLNGCRK2])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CLNGCRK2])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CLNGCRK3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CLNGCRK3])>|40|))";
            var compiler = new CalculateEvaluateCompiler();
            var parsingException = Assert.Throws<CalculateEvaluateParsingException>(() => compiler.GetEvaluator(input));

            const string parameterNamePattern = "[A-Za-z_][A-Za-z_0-9]*";
            var parenthesizedParameterPattern = new Regex($@"\((\[{parameterNamePattern}\]|{parameterNamePattern})\)");
            var fixedInput = parenthesizedParameterPattern.Replace(input, "$1");
            var compilationException = Assert.Throws<CalculateEvaluateCompilationException>(() => compiler.GetEvaluator(fixedInput));

            compiler.ParameterTypes["BUSIPLAN"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CBRKSLB2"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CBRKSLB3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CFLTJNT3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CLNGCRK2"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CLNGCRK3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CTRNCRK1"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CTRNCRK2"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CTRNCRK3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CTRNJNT3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["EXP_IND"] = CalculateEvaluateParameterType.Text;
            compiler.ParameterTypes["SURFACEID"] = CalculateEvaluateParameterType.Number;
            _ = compiler.GetEvaluator(fixedInput);
        }
    }
}
