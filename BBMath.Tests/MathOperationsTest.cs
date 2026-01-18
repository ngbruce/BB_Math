using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BBMath.Core;

namespace BBMath.Tests
{
    [TestClass]
    public class MathOperationsTest
    {
        private MathProblemGenerator _generator;
        private Random _testRandom;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _testRandom = new Random(12345); // 固定种子确保测试可重复
            _generator = new MathProblemGenerator(_testRandom);
        }
        
        [TestMethod]
        public void TestAdditionOperation()
        {
            // 基础测试示例，验证测试框架正常工作
            int result = 1 + 1;
            Assert.AreEqual(2, result, "加法运算应该正确");
        }

        [TestMethod]
        public void TestExamTypeEnum()
        {
            // 验证枚举定义
            Assert.IsTrue(Enum.IsDefined(typeof(ExamType), ExamType.Addition), "加法题型应该存在");
            Assert.IsTrue(Enum.IsDefined(typeof(ExamType), ExamType.Subtraction), "减法题型应该存在");
            Assert.IsTrue(Enum.IsDefined(typeof(ExamType), ExamType.Multiplication), "乘法题型应该存在");
            Assert.IsTrue(Enum.IsDefined(typeof(ExamType), ExamType.DivisionNoRemainder), "无余数除法题型应该存在");
            Assert.IsTrue(Enum.IsDefined(typeof(ExamType), ExamType.DivisionWithRemainder), "有余数除法题型应该存在");
        }

        [TestMethod]
        public void TestExamObjectCreation()
        {
            // 测试ExamObject创建
            var examObj = new ExamObject(25, 300, false, "测试题型", ExamType.Addition, 3, 0, false);
            Assert.IsNotNull(examObj, "ExamObject应该成功创建");
            Assert.AreEqual(ExamType.Addition, examObj.Examtype, "题型应该正确设置");
        }
        
        [TestMethod]
        public void TestAdditionStrategy_GeneratesCorrectProblem()
        {
            // 测试加法策略
            var strategy = new AdditionStrategy(_testRandom);
            var problem = new MathProblem
            {
                Type = ExamType.Addition,
                IntegerBits = 2
            };
            
            strategy.GenerateProblem(problem, 2);
            
            Assert.IsTrue(problem.Operand1 >= 10 && problem.Operand1 <= 99, "加数1应在10-99范围内");
            Assert.IsTrue(problem.Operand2 >= 10 && problem.Operand2 <= 99, "加数2应在10-99范围内");
            Assert.AreEqual(problem.Operand1 + problem.Operand2, problem.CorrectAnswer, "正确答案应为两数之和");
            Assert.AreEqual($"{problem.Operand1} + {problem.Operand2}", problem.EquationString, "算式字符串应正确格式化");
        }
        
        [TestMethod]
        public void TestSubtractionStrategy_GeneratesNonNegativeResultWhenNotAllowed()
        {
            // 测试减法策略（不允许负数结果）
            var strategy = new SubtractionStrategy(_testRandom);
            var problem = new MathProblem
            {
                Type = ExamType.Subtraction,
                IntegerBits = 2,
                AllowNegativeResult = false
            };
            
            strategy.GenerateProblem(problem, 2, 0, false);
            
            Assert.IsTrue(problem.Operand1 >= problem.Operand2, "当不允许负数结果时，被减数应大于等于减数");
            Assert.AreEqual(problem.Operand1 - problem.Operand2, problem.CorrectAnswer, "正确答案应为两数之差");
            Assert.IsTrue(problem.CorrectAnswer >= 0, "结果应为非负数");
        }
        
        [TestMethod]
        public void TestSubtractionStrategy_AllowsNegativeResultWhenAllowed()
        {
            // 测试减法策略（允许负数结果）
            var strategy = new SubtractionStrategy(_testRandom);
            var problem = new MathProblem
            {
                Type = ExamType.Subtraction,
                IntegerBits = 2,
                AllowNegativeResult = true
            };
            
            strategy.GenerateProblem(problem, 2, 0, true);
            
            // 当允许负数结果时，不验证被减数>=减数
            Assert.AreEqual(problem.Operand1 - problem.Operand2, problem.CorrectAnswer, "正确答案应为两数之差");
        }
        
        [TestMethod]
        public void TestMultiplicationStrategy_GeneratesCorrectProblem()
        {
            // 测试乘法策略
            var strategy = new MultiplicationStrategy(_testRandom);
            var problem = new MathProblem
            {
                Type = ExamType.Multiplication,
                IntegerBits = 2
            };
            
            strategy.GenerateProblem(problem, 2);
            
            Assert.IsTrue(problem.Operand1 >= 10 && problem.Operand1 <= 99, "乘数1应在10-99范围内");
            Assert.IsTrue(problem.Operand2 >= 10 && problem.Operand2 <= 99, "乘数2应在10-99范围内");
            Assert.AreEqual(problem.Operand1 * problem.Operand2, problem.CorrectAnswer, "正确答案应为两数之积");
            Assert.AreEqual($"{problem.Operand1} × {problem.Operand2}", problem.EquationString, "算式字符串应正确格式化");
        }
        
        [TestMethod]
        public void TestDivisionNoRemainderStrategy_GeneratesCorrectProblem()
        {
            // 测试无余数除法策略
            var strategy = new DivisionNoRemainderStrategy(_testRandom);
            var problem = new MathProblem
            {
                Type = ExamType.DivisionNoRemainder,
                IntegerBits = 3
            };
            
            strategy.GenerateProblem(problem, 3);
            
            Assert.IsTrue(problem.Operand2 >= 1, "除数应大于等于1");
            Assert.IsTrue(problem.CorrectAnswer >= 1, "商应大于等于1");
            Assert.AreEqual(problem.Operand2 * problem.CorrectAnswer, problem.Operand1, "被除数应为除数乘以商");
            Assert.AreEqual(0, problem.Remainder, "无余数除法的余数应为0");
            Assert.AreEqual($"{problem.Operand1} ÷ {problem.Operand2}", problem.EquationString, "算式字符串应正确格式化");
        }
        
        [TestMethod]
        public void TestDivisionWithRemainderStrategy_GeneratesCorrectProblem()
        {
            // 测试有余数除法策略
            var strategy = new DivisionWithRemainderStrategy(_testRandom);
            var problem = new MathProblem
            {
                Type = ExamType.DivisionWithRemainder,
                IntegerBits = 3
            };
            
            strategy.GenerateProblem(problem, 3);
            
            Assert.IsTrue(problem.Operand2 >= 2, "除数应至少为2");
            Assert.IsTrue(problem.CorrectAnswer >= 1, "商应大于等于1");
            Assert.IsTrue(problem.Remainder >= 1 && problem.Remainder < problem.Operand2, "余数应大于等于1且小于除数");
            Assert.AreEqual(problem.Operand2 * problem.CorrectAnswer + problem.Remainder, problem.Operand1, "被除数应为除数乘以商加余数");
            Assert.AreEqual($"{problem.Operand1} ÷ {problem.Operand2}", problem.EquationString, "算式字符串应正确格式化");
        }
        
        [TestMethod]
        public void TestMathProblemGenerator_GeneratesAllProblemTypes()
        {
            // 测试生成器能生成所有类型的题目
            var examTypes = new[]
            {
                ExamType.Addition,
                ExamType.Subtraction,
                ExamType.Multiplication,
                ExamType.DivisionNoRemainder,
                ExamType.DivisionWithRemainder
            };
            
            foreach (var examType in examTypes)
            {
                var problem = _generator.GenerateProblem(examType, 2);
                Assert.IsNotNull(problem, $"应能生成{examType}类型的题目");
                Assert.AreEqual(examType, problem.Type, "题目类型应匹配");
                Assert.IsNotNull(problem.EquationString, "算式字符串不应为空");
            }
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMathProblemGenerator_ThrowsForUnsupportedType()
        {
            // 测试生成器对不支持的类型抛出异常
            _generator.GenerateProblem(ExamType.Mixed, 2);
        }
        
        [TestMethod]
        public void TestMathProblemGenerator_RegisterStrategy_AllowsExtension()
        {
            // 测试注册新策略的扩展机制
            var customGenerator = new MathProblemGenerator(_testRandom);
            
            // 创建一个简单的自定义策略
            var customStrategy = new TestCustomStrategy(_testRandom);
            
            // 注册自定义策略
            customGenerator.RegisterStrategy(ExamType.Mixed, customStrategy);
            
            // 验证自定义策略可用
            var problem = customGenerator.GenerateProblem(ExamType.Mixed, 2);
            Assert.AreEqual(ExamType.Mixed, problem.Type);
            Assert.AreEqual("Custom: 5", problem.EquationString);
        }
        
        [TestMethod]
        public void TestMathProblemGenerator_GenerateRandomProblem_ReturnsValidProblem()
        {
            // 测试随机题目生成
            for (int i = 0; i < 10; i++)
            {
                var problem = _generator.GenerateRandomProblem(2);
                Assert.IsNotNull(problem);
                Assert.IsTrue(Enum.IsDefined(typeof(ExamType), problem.Type));
                Assert.IsNotNull(problem.EquationString);
            }
        }
        
        [TestMethod]
        public void TestMathProblem_ValidateAnswer_Addition()
        {
            // 测试题目答案验证（加法）
            var problem = new MathProblem
            {
                Type = ExamType.Addition,
                Operand1 = 5,
                Operand2 = 3,
                CorrectAnswer = 8
            };
            
            Assert.IsTrue(problem.ValidateAnswer(8), "正确答案应通过验证");
            Assert.IsFalse(problem.ValidateAnswer(7), "错误答案应不通过验证");
        }
        
        [TestMethod]
        public void TestMathProblem_ValidateAnswer_DivisionWithRemainder()
        {
            // 测试题目答案验证（有余数除法）
            var problem = new MathProblem
            {
                Type = ExamType.DivisionWithRemainder,
                Operand1 = 7,
                Operand2 = 3,
                CorrectAnswer = 2,
                Remainder = 1
            };
            
            Assert.IsTrue(problem.ValidateAnswer(2, 1), "正确答案和余数应通过验证");
            Assert.IsFalse(problem.ValidateAnswer(2, 2), "正确商但错误余数应不通过验证");
            Assert.IsFalse(problem.ValidateAnswer(3, 1), "错误商但正确余数应不通过验证");
        }
        
        // 自定义策略类用于测试扩展机制
        private class TestCustomStrategy : IProblemGenerationStrategy
        {
            private readonly Random _random;
            
            public TestCustomStrategy(Random random)
            {
                _random = random;
            }
            
            public void GenerateProblem(MathProblem problem, int intBits = 0, int decBits = 0, bool allowNegativeResult = false)
            {
                problem.Operand1 = 2;
                problem.Operand2 = 3;
                problem.CorrectAnswer = 5;
                problem.EquationString = "Custom: 5";
            }
        }
    }
}