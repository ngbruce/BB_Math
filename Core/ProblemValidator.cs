using System;

namespace BBMath.Core
{
    /// <summary>
    /// 题目验证器实现
    /// </summary>
    public class ProblemValidator : IProblemValidator
    {
        /// <summary>
        /// 验证用户答案是否正确
        /// </summary>
        /// <param name="problem">题目信息</param>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="userRemainder">用户余数（仅适用于有余数除法）</param>
        /// <returns>验证结果</returns>
        public bool ValidateAnswer(MathProblem problem, int userAnswer, int userRemainder = 0)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));
                
            return problem.ValidateAnswer(userAnswer, userRemainder);
        }
        
        /// <summary>
        /// 验证用户答案是否正确，返回详细的验证信息
        /// </summary>
        /// <param name="problem">题目信息</param>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="userRemainder">用户余数（仅适用于有余数除法）</param>
        /// <returns>验证结果和消息</returns>
        public ValidationResult ValidateWithDetails(MathProblem problem, int userAnswer, int userRemainder = 0)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));
                
            bool isValid = problem.ValidateAnswer(userAnswer, userRemainder);
            
            if (isValid)
            {
                return ValidationResult.Success(problem);
            }
            else
            {
                string reason = "答案错误";
                
                if (problem.Type == ExamType.DivisionWithRemainder)
                {
                    reason += $"。正确答案：{problem.CorrectAnswer}，余数：{problem.Remainder}";
                }
                else
                {
                    reason += $"。正确答案：{problem.CorrectAnswer}";
                }
                
                return ValidationResult.Failure(problem, reason);
            }
        }
    }
}