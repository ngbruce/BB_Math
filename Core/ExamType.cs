using System;

namespace BBMath.Core
{
    /// <summary>
    /// 题目类型枚举
    /// </summary>
    public enum ExamType
    {
        /// <summary>
        /// 加法
        /// </summary>
        Addition = 0,
        
        /// <summary>
        /// 减法
        /// </summary>
        Subtraction = 1,
        
        /// <summary>
        /// 乘法
        /// </summary>
        Multiplication = 2,
        
        /// <summary>
        /// 除法
        /// </summary>
        Division = 3,
        
        /// <summary>
        /// 无余数除法
        /// </summary>
        DivisionNoRemainder = 4,
        
        /// <summary>
        /// 有余数除法
        /// </summary>
        DivisionWithRemainder = 5,
        
        /// <summary>
        /// 混合运算
        /// </summary>
        Mixed = 6
    }
}