using System;

namespace BBMath.Core
{
    /// <summary>
    /// 减法题目生成策略
    /// </summary>
    public class SubtractionStrategy : IProblemGenerationStrategy
    {
        private readonly Random _random;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="random">随机数生成器实例</param>
        public SubtractionStrategy(Random random = null)
        {
            _random = random ?? new Random();
        }
        
        /// <summary>
        /// 生成减法题目
        /// </summary>
        /// <param name="problem">题目信息对象</param>
        /// <param name="intBits">整数位数限制</param>
        /// <param name="decBits">小数位数限制（当前未使用）</param>
        /// <param name="allowNegativeResult">是否允许负数结果</param>
        public void GenerateProblem(MathProblem problem, int intBits = 0, int decBits = 0, bool allowNegativeResult = false)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));
                
            int min = (int)Math.Pow(10, intBits - 1);
            int max = (int)Math.Pow(10, intBits);
            
            do
            {
                problem.Operand1 = _random.Next(min, max);
                problem.Operand2 = _random.Next(min, max);
                problem.CorrectAnswer = problem.Operand1 - problem.Operand2;
            } while (!allowNegativeResult && problem.CorrectAnswer < 0);
            
            problem.EquationString = $"{problem.Operand1} - {problem.Operand2}";
        }
    }
}