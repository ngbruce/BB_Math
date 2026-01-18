using System;

namespace BBMath.Core
{
    /// <summary>
    /// 有余数除法题目生成策略
    /// </summary>
    public class DivisionWithRemainderStrategy : IProblemGenerationStrategy
    {
        private readonly Random _random;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="random">随机数生成器实例</param>
        public DivisionWithRemainderStrategy(Random random = null)
        {
            _random = random ?? new Random();
        }
        
        /// <summary>
        /// 生成有余数除法题目
        /// </summary>
        /// <param name="problem">题目信息对象</param>
        /// <param name="intBits">整数位数限制</param>
        /// <param name="decBits">小数位数限制（当前未使用）</param>
        /// <param name="allowNegativeResult">是否允许负数结果（对除法不适用）</param>
        public void GenerateProblem(MathProblem problem, int intBits = 0, int decBits = 0, bool allowNegativeResult = false)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            // 根据 intBits 生成不同位数的除法题目
            int max = (int)Math.Pow(10, intBits);
            int min = (int)Math.Pow(10, intBits - 1);

            // 对于个位数（intBits=1），min=1而不是10
            if (intBits == 1)
            {
                min = 2; // 除数至少为2
            }

            problem.Operand2 = _random.Next(min, max); // 除数
            problem.CorrectAnswer = _random.Next(min, max); // 商
            problem.Remainder = _random.Next(1, problem.Operand2); // 余数，小于除数
            problem.Operand1 = problem.Operand2 * problem.CorrectAnswer + problem.Remainder; // 被除数
            problem.EquationString = $"{problem.Operand1} ÷ {problem.Operand2}";
        }
    }
}