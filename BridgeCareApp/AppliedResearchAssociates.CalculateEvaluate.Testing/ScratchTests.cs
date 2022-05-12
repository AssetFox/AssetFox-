using NUnit.Framework;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class ScratchTests
    {
        [Test]
        public void WhatHappensWhenParameterIsNotDefined()
        {
            var compiler = new CalculateEvaluateCompiler();
            var e = Assert.Throws<CalculateEvaluateCompilationException>(() => compiler.GetEvaluator("foobar=42"));
        }

        [Test]
        public void WhatHappensWhenParameterIsParenthesized()
        {
            var compiler = new CalculateEvaluateCompiler();
            var e = Assert.Throws<CalculateEvaluateParsingException>(() => compiler.GetEvaluator("(foobar)=42"));
        }

        [Test]
        public void QuestionFromMaxxLeach_2022_05_12()
        {
            const string input = @"((([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK1])>|50|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK1])>|50|)) AND ((([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CFLTJNT3])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CFLTJNT3])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CBRKSLB2])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CBRKSLB2])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CBRKSLB3])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CBRKSLB3])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNJNT3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNJNT3])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK2])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK2])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK3])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CLNGCRK2])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CLNGCRK2])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CLNGCRK3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CLNGCRK3])>|40|))";
            var compiler = new CalculateEvaluateCompiler();
            var e = Assert.Throws<CalculateEvaluateParsingException>(() => compiler.GetEvaluator(input));
        }
    }
}
