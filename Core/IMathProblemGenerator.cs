using System;

namespace BBMath.Core
{
    /// <summary>
    /// 数学题目信息
    /// </summary>
    public class MathProblem
    {
        /// <summary>
        /// 题目类型
        /// </summary>
        public ExamType Type { get; set; }
        
        /// <summary>
        /// 第一个操作数
        /// </summary>
        public int Operand1 { get; set; }
        
        /// <summary>
        /// 第二个操作数
        /// </summary>
        public int Operand2 { get; set; }
        
        /// <summary>
        /// 正确答案（对于除法是商）
        /// </summary>
        public int CorrectAnswer { get; set; }
        
        /// <summary>
        /// 余数（仅适用于有余数除法）
        /// </summary>
        public int Remainder { get; set; }
        
        /// <summary>
        /// 算式字符串表示
        /// </summary>
        public string EquationString { get; set; }
        
        /// <summary>
        /// 整数位数限制
        /// </summary>
        public int IntegerBits { get; set; }
        
        /// <summary>
        /// 小数位数限制
        /// </summary>
        public int DecimalBits { get; set; }
        
        /// <summary>
        /// 是否允许负数结果
        /// </summary>
        public bool AllowNegativeResult { get; set; }
        
        /// <summary>
        /// 验证用户答案是否正确
        /// </summary>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="userRemainder">用户余数（仅适用于有余数除法）</param>
        /// <returns>验证结果</returns>
        public bool ValidateAnswer(int userAnswer, int userRemainder = 0)
        {
            if (Type == ExamType.DivisionWithRemainder)
            {
                return userAnswer == CorrectAnswer && userRemainder == Remainder;
            }
            else
            {
                return userAnswer == CorrectAnswer;
            }
        }
        
        /// <summary>
        /// 获取完整的题目描述
        /// </summary>
        public override string ToString()
        {
            return EquationString;
        }
    }
    
    /// <summary>
    /// 数学题目生成器接口
    /// </summary>
    public interface IMathProblemGenerator
    {
        /// <summary>
        /// 生成数学题目
        /// </summary>
        /// <param name="examType">题目类型</param>
        /// <param name="intBits">整数位数</param>
        /// <param name="decBits">小数位数</param>
        /// <param name="allowNegativeResult">是否允许负数结果</param>
        /// <returns>生成的题目信息</returns>
        MathProblem GenerateProblem(ExamType examType, int intBits = 0, int decBits = 0, bool allowNegativeResult = false);
        
        /// <summary>
        /// 生成随机题目（从当前配置的题目类型中随机选择）
        /// </summary>
        /// <param name="intBits">整数位数</param>
        /// <param name="decBits">小数位数</param>
        /// <param name="allowNegativeResult">是否允许负数结果</param>
        /// <returns>生成的题目信息</returns>
        MathProblem GenerateRandomProblem(int intBits = 0, int decBits = 0, bool allowNegativeResult = false);
    }
}