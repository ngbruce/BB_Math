using System;

namespace BBMath.Core
{
    /// <summary>
    /// 题目生成策略接口
    /// 定义不同运算类型的题目生成逻辑
    /// </summary>
    public interface IProblemGenerationStrategy
    {
        /// <summary>
        /// 生成题目
        /// </summary>
        /// <param name="problem">题目信息对象</param>
        /// <param name="intBits">整数位数限制</param>
        /// <param name="decBits">小数位数限制</param>
        /// <param name="allowNegativeResult">是否允许负数结果</param>
        void GenerateProblem(MathProblem problem, int intBits = 0, int decBits = 0, bool allowNegativeResult = false);
    }
}