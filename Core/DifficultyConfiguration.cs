using System;
using System.Collections.Generic;

namespace BBMath.Core
{
    /// <summary>
    /// 难度配置信息
    /// </summary>
    public class DifficultyConfig
    {
        /// <summary>
        /// 难度级别
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// 显示文本
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 最小操作数
        /// </summary>
        public int MinOperand { get; set; }

        /// <summary>
        /// 最大操作数
        /// </summary>
        public int MaxOperand { get; set; }

        /// <summary>
        /// 整数位数
        /// </summary>
        public int IntegerBits { get; set; }

        /// <summary>
        /// 支持的运算类型列表
        /// </summary>
        public List<ExamType> SupportedOperations { get; set; }

        /// <summary>
        /// 是否允许负数结果
        /// </summary>
        public bool AllowNegativeResult { get; set; }

        /// <summary>
        /// 是否要求至少有一个操作数或结果大于等于100（仅用于Hard难度）
        /// </summary>
        public bool RequireAbove100 { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DifficultyConfig()
        {
            SupportedOperations = new List<ExamType>();
        }
    }

    /// <summary>
    /// 难度配置管理器
    /// </summary>
    public static class DifficultyConfigurationManager
    {
        /// <summary>
        /// 获取指定难度的配置
        /// </summary>
        /// <param name="difficulty">难度级别</param>
        /// <returns>难度配置信息</returns>
        public static DifficultyConfig GetConfig(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.LV1:
                    // 个位数加减法：操作数1-9，加减法各50%，减法结果>=0
                    return new DifficultyConfig
                    {
                        Difficulty = Difficulty.LV1,
                        DisplayText = "(1)个位数加减法",
                        MinOperand = 1,
                        MaxOperand = 9,
                        IntegerBits = 1,
                        AllowNegativeResult = false,
                        RequireAbove100 = false,
                        SupportedOperations = new List<ExamType>
                        {
                            ExamType.Addition,
                            ExamType.Subtraction
                        }
                    };

                case Difficulty.LV2:
                    // 100以内加减法：操作数1-99，加减法各50%，加法结果<100，减法结果>=0
                    return new DifficultyConfig
                    {
                        Difficulty = Difficulty.LV2,
                        DisplayText = "(2)100以内加减法",
                        MinOperand = 1,
                        MaxOperand = 99,
                        IntegerBits = 2,
                        AllowNegativeResult = false,
                        RequireAbove100 = false,
                        SupportedOperations = new List<ExamType>
                        {
                            ExamType.Addition,
                            ExamType.Subtraction
                        }
                    };

                case Difficulty.LV3:
                    // 个位数乘除法：操作数1-9，乘除法各50%（除法无余数）
                    return new DifficultyConfig
                    {
                        Difficulty = Difficulty.LV3,
                        DisplayText = "(3)个位数乘除法",
                        MinOperand = 1,
                        MaxOperand = 9,
                        IntegerBits = 1,
                        AllowNegativeResult = false,
                        RequireAbove100 = false,
                        SupportedOperations = new List<ExamType>
                        {
                            ExamType.Multiplication,
                            ExamType.DivisionNoRemainder
                        }
                    };

                case Difficulty.LV4:
                    // 100以内加减乘除带余数：操作数1-99，五类运算各20%
                    return new DifficultyConfig
                    {
                        Difficulty = Difficulty.LV4,
                        DisplayText = "(4)100以内加减乘除带余数",
                        MinOperand = 1,
                        MaxOperand = 99,
                        IntegerBits = 2,
                        AllowNegativeResult = false,
                        RequireAbove100 = false,
                        SupportedOperations = new List<ExamType>
                        {
                            ExamType.Addition,
                            ExamType.Subtraction,
                            ExamType.Multiplication,
                            ExamType.DivisionNoRemainder,
                            ExamType.DivisionWithRemainder
                        }
                    };

                case Difficulty.LV5:
                    // 100以上加减乘除带余数：操作数1-999，五类运算各20%
                    // 注意："100以上"指操作数范围扩展到三位数（1-999），而非必须>=100
                    // 除法操作数为两位数（10-99），符合文档定义
                    return new DifficultyConfig
                    {
                        Difficulty = Difficulty.LV5,
                        DisplayText = "(5)100以上加减乘除带余数",
                        MinOperand = 1,
                        MaxOperand = 999,
                        IntegerBits = 3,
                        AllowNegativeResult = false,
                        RequireAbove100 = false, // 移除此约束，与文档定义保持一致
                        SupportedOperations = new List<ExamType>
                        {
                            ExamType.Addition,
                            ExamType.Subtraction,
                            ExamType.Multiplication,
                            ExamType.DivisionNoRemainder,
                            ExamType.DivisionWithRemainder
                        }
                    };

                default:
                    throw new ArgumentException($"不支持的难度级别: {difficulty}");
            }
        }

        /// <summary>
        /// 获取所有难度配置列表
        /// </summary>
        /// <returns>所有难度配置</returns>
        public static List<DifficultyConfig> GetAllConfigs()
        {
            return new List<DifficultyConfig>
            {
                GetConfig(Difficulty.LV1),
                GetConfig(Difficulty.LV2),
                GetConfig(Difficulty.LV3),
                GetConfig(Difficulty.LV4),
                GetConfig(Difficulty.LV5)
            };
        }

        /// <summary>
        /// 根据显示文本获取难度
        /// </summary>
        /// <param name="displayText">显示文本</param>
        /// <returns>难度级别</returns>
        public static Difficulty GetDifficultyByText(string displayText)
        {
            var configs = GetAllConfigs();
            foreach (var config in configs)
            {
                if (config.DisplayText == displayText)
                {
                    return config.Difficulty;
                }
            }
            return Difficulty.LV1; // 默认返回 LV1
        }
    }
}
