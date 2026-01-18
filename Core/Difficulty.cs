using System;

namespace BBMath.Core
{
    /// <summary>
    /// 题目难度级别枚举
    /// </summary>
    public enum Difficulty
    {
        /// <summary>
        /// 级别1：个位数加减法
        /// </summary>
        LV1 = 0,

        /// <summary>
        /// 级别2：100以内加减法
        /// </summary>
        LV2 = 1,

        /// <summary>
        /// 级别3：个位数乘除法
        /// </summary>
        LV3 = 2,

        /// <summary>
        /// 级别4：100以内加减乘除带余数
        /// </summary>
        LV4 = 3,

        /// <summary>
        /// 级别5：100以上加减乘除带余数
        /// </summary>
        LV5 = 4
    }
}
