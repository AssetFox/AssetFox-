using Xunit;

namespace AppliedResearchAssociates.CalculateEvaluate.Testing
{
    public class ScratchTests
    {
        [Fact]
        public void ParensInsideLiteral()
        {
            const string input = "[MATERIAL_TYPE]<>'2-Concrete (cast in place)'";
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["MATERIAL_TYPE"] = CalculateEvaluateParameterType.Text;
            _ = compiler.GetEvaluator(input);
        }

        [Fact]
        public void QuestionFromJeffDavis_2024_01_15()
        {
            const string input1 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]>='28500' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.75' AND [DECK_SEEDED]<'5.75' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input2 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND ([BUS_PLAN_NETWORK]='1' OR [BUS_PLAN_NETWORK]='T') AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.75' AND [DECK_SEEDED]<'5.75' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input3 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND ([BUS_PLAN_NETWORK]='2' OR [BUS_PLAN_NETWORK]='H') AND [RISK_SCORE]>='15000' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.25' AND [DECK_SEEDED]<'5.5' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input4 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND ([BUS_PLAN_NETWORK]='2' OR [BUS_PLAN_NETWORK]='H') AND [RISK_SCORE]<'15000' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.25' AND [DECK_SEEDED]<'5' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input5 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND [BUS_PLAN_NETWORK]='3' AND [RISK_SCORE]>='7000' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.25' AND [DECK_SEEDED]<'5.5' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input6 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND [BUS_PLAN_NETWORK]='3' AND [RISK_SCORE]<'7000' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.25' AND [DECK_SEEDED]<'5' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input7 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND ([BUS_PLAN_NETWORK]='4' OR [BUS_PLAN_NETWORK]='L' OR [BUS_PLAN_NETWORK]='D' OR [BUS_PLAN_NETWORK]='N') AND [RISK_SCORE]>='2000' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.25' AND [DECK_SEEDED]<'5.5' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";
            const string input8 = "[BRIDGE_TYPE]='B' AND [DECK_AREA]<'28500' AND [LENGTH]>='30' AND ([BUS_PLAN_NETWORK]='4' OR [BUS_PLAN_NETWORK]='L' OR [BUS_PLAN_NETWORK]='D' OR [BUS_PLAN_NETWORK]='N') AND [RISK_SCORE]<'2000' AND [SUB_SEEDED]>='4.75' AND [SUP_SEEDED]>='5.25' AND [DECK_SEEDED]<'5' AND [P3]='0' AND ([MATERIAL_TYPE]<>'2-Concrete (cast in place)' OR [STRUCT_CONFIG_6A29]<>'03')";

            var compiler = new CalculateEvaluateCompiler();

            compiler.ParameterTypes["BRIDGE_TYPE"] = CalculateEvaluateParameterType.Text;
            compiler.ParameterTypes["BUS_PLAN_NETWORK"] = CalculateEvaluateParameterType.Text;
            compiler.ParameterTypes["DECK_AREA"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["DECK_SEEDED"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["LENGTH"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["MATERIAL_TYPE"] = CalculateEvaluateParameterType.Text;
            compiler.ParameterTypes["P3"] = CalculateEvaluateParameterType.Text;
            compiler.ParameterTypes["RISK_SCORE"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["STRUCT_CONFIG_6A29"] = CalculateEvaluateParameterType.Text;
            compiler.ParameterTypes["SUB_SEEDED"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["SUP_SEEDED"] = CalculateEvaluateParameterType.Number;

            _ = compiler.GetEvaluator(input1);
            _ = compiler.GetEvaluator(input2);
            _ = compiler.GetEvaluator(input3);
            _ = compiler.GetEvaluator(input4);
            _ = compiler.GetEvaluator(input5);
            _ = compiler.GetEvaluator(input6);
            _ = compiler.GetEvaluator(input7);
            _ = compiler.GetEvaluator(input8);
        }

        [Fact]
        public void QuestionFromLaxAndTyler_2023_10_11_Part1()
        {
            const string input = "[CTRNJNT3]*100/[CJOINTCT]>'50'";
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["CTRNJNT3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["CJOINTCT"] = CalculateEvaluateParameterType.Number;
            _ = compiler.GetEvaluator(input);
        }

        [Fact]
        public void QuestionFromLaxAndTyler_2023_10_11_Part2()
        {
            const string input = "[BFATICR2]+[BFATICR3] <= [SEGMENT_LENGTH] *.3";
            var compiler = new CalculateEvaluateCompiler();
            compiler.ParameterTypes["BFATICR2"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["BFATICR3"] = CalculateEvaluateParameterType.Number;
            compiler.ParameterTypes["SEGMENT_LENGTH"] = CalculateEvaluateParameterType.Number;
            _ = compiler.GetEvaluator(input);
        }

        [Fact]
        public void QuestionFromMaxxLeach_2022_05_12()
        {
            const string input = @"((([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK1])>|50|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK1])>|50|)) AND ((([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CFLTJNT3])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CFLTJNT3])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CBRKSLB2])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CBRKSLB2])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CBRKSLB3])>|30|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CBRKSLB3])>|30|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNJNT3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNJNT3])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK2])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK2])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CTRNCRK3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CTRNCRK3])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CLNGCRK2])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CLNGCRK2])>|40|) OR (([SURFACEID]>|62|) AND (([BUSIPLAN]=|1|) OR ([BUSIPLAN]=|2| AND [EXP_IND]=|Y|))) AND (([CLNGCRK3])>|40|) OR (([SURFACEID]>|62|) AND ([BUSIPLAN]=|2| AND [EXP_IND]<>|Y|)) AND (([CLNGCRK3])>|40|))";
            var compiler = new CalculateEvaluateCompiler();
            var compilationException = Assert.Throws<CalculateEvaluateCompilationException>(() => compiler.GetEvaluator(input));

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
            _ = compiler.GetEvaluator(input);
        }

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
            compiler.ParameterTypes["foobar"] = CalculateEvaluateParameterType.Number;
            _ = compiler.GetEvaluator("(foobar)=42");
        }
    }
}
