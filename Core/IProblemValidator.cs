using System;

namespace BBMath.Core
{
    /// <summary>
    /// 题目验证器接口
    /// </summary>
    public interface IProblemValidator
    {
        /// <summary>
        /// 验证用户答案是否正确
        /// </summary>
        /// <param name="problem">题目信息</param>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="userRemainder">用户余数（仅适用于有余数除法）</param>
        /// <returns>验证结果</returns>
        bool ValidateAnswer(MathProblem problem, int userAnswer, int userRemainder = 0);
        
        /// <summary>
        /// 验证用户答案是否正确，返回详细的验证信息
        /// </summary>
        /// <param name="problem">题目信息</param>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="userRemainder">用户余数（仅适用于有余数除法）</param>
        /// <returns>验证结果和消息</returns>
        ValidationResult ValidateWithDetails(MathProblem problem, int userAnswer, int userRemainder = 0);
    }
    
    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// 验证消息
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// 正确答案
        /// </summary>
        public int CorrectAnswer { get; set; }
        
        /// <summary>
        /// 正确余数（仅适用于有余数除法）
        /// </summary>
        public int CorrectRemainder { get; set; }
        
        /// <summary>
        /// 创建成功的验证结果
        /// </summary>
        public static ValidationResult Success(MathProblem problem)
        {
            return new ValidationResult
            {
                IsValid = true,
                Message = "答案正确！",
                CorrectAnswer = problem.CorrectAnswer,
                CorrectRemainder = problem.Remainder
            };
        }
        
        /// <summary>
        /// 创建失败的验证结果
        /// </summary>
        public static ValidationResult Failure(MathProblem problem, string reason)
        {
            return new ValidationResult
            {
                IsValid = false,
                Message = reason,
                CorrectAnswer = problem.CorrectAnswer,
                CorrectRemainder = problem.Remainder
            };
        }
    }
}