using System;
using System.Collections.Generic;

namespace BBMath.Core
{
    /// <summary>
    /// 数学题目生成器实现 - 采用策略模式实现可扩展的题目生成
    /// </summary>
    public class MathProblemGenerator : IMathProblemGenerator
    {
        private readonly Random _random;
        private readonly Dictionary<ExamType, IProblemGenerationStrategy> _strategies;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="random">随机数生成器实例</param>
        public MathProblemGenerator(Random random = null)
        {
            _random = random ?? new Random();
            _strategies = new Dictionary<ExamType, IProblemGenerationStrategy>();
            
            // 注册默认策略
            RegisterDefaultStrategies();
        }
        
        /// <summary>
        /// 注册题目生成策略
        /// </summary>
        /// <param name="examType">题目类型</param>
        /// <param name="strategy">生成策略实例</param>
        public void RegisterStrategy(ExamType examType, IProblemGenerationStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
                
            _strategies[examType] = strategy;
        }
        
        /// <summary>
        /// 生成数学题目
        /// </summary>
        /// <param name="examType">题目类型</param>
        /// <param name="intBits">整数位数</param>
        /// <param name="decBits">小数位数</param>
        /// <param name="allowNegativeResult">是否允许负数结果</param>
        /// <returns>生成的题目信息</returns>
        public MathProblem GenerateProblem(ExamType examType, int intBits = 0, int decBits = 0, bool allowNegativeResult = false)
        {
            if (!_strategies.TryGetValue(examType, out var strategy))
            {
                throw new ArgumentException($"不支持的题目类型: {examType}", nameof(examType));
            }
            
            var problem = new MathProblem
            {
                Type = examType,
                IntegerBits = intBits,
                DecimalBits = decBits,
                AllowNegativeResult = allowNegativeResult
            };
            
            strategy.GenerateProblem(problem, intBits, decBits, allowNegativeResult);
            
            return problem;
        }
        
        /// <summary>
        /// 生成随机题目（从当前配置的题目类型中随机选择）
        /// </summary>
        /// <param name="intBits">整数位数</param>
        /// <param name="decBits">小数位数</param>
        /// <param name="allowNegativeResult">是否允许负数结果</param>
        /// <returns>生成的题目信息</returns>
        public MathProblem GenerateRandomProblem(int intBits = 0, int decBits = 0, bool allowNegativeResult = false)
        {
            // 从已注册的策略中随机选择
            var availableTypes = new List<ExamType>(_strategies.Keys);
            if (availableTypes.Count == 0)
            {
                throw new InvalidOperationException("没有注册任何题目生成策略");
            }
            
            ExamType randomType = availableTypes[_random.Next(availableTypes.Count)];
            return GenerateProblem(randomType, intBits, decBits, allowNegativeResult);
        }
        
        /// <summary>
        /// 注册默认策略（加法、减法、乘法、无余数除法、有余数除法）
        /// </summary>
        private void RegisterDefaultStrategies()
        {
            RegisterStrategy(ExamType.Addition, new AdditionStrategy(_random));
            RegisterStrategy(ExamType.Subtraction, new SubtractionStrategy(_random));
            RegisterStrategy(ExamType.Multiplication, new MultiplicationStrategy(_random));
            RegisterStrategy(ExamType.DivisionNoRemainder, new DivisionNoRemainderStrategy(_random));
            RegisterStrategy(ExamType.DivisionWithRemainder, new DivisionWithRemainderStrategy(_random));
        }
    }
}