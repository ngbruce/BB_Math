using System;
using System.Collections.Generic;

namespace BBMath.Core
{
    /// <summary>
    /// 暂停类型枚举
    /// </summary>
    public enum PauseType
    {
        /// <summary>
        /// 限次数暂停
        /// </summary>
        ByCount = 0,

        /// <summary>
        /// 限时间暂停
        /// </summary>
        ByTime = 1
    }

    /// <summary>
    /// 应用程序常量类，集中管理所有硬编码的常量值
    /// </summary>
    public static class AppConstants
    {
        #region 文件名常量

        /// <summary>
        /// 配置文件名
        /// </summary>
        public const string ConfigFileName = "bbmath.cfg";

        /// <summary>
        /// 日志数据文件名
        /// </summary>
        public const string LogDataFileName = "bbmath.dat";

        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public const string LogFilePrefix = "bbmath";

        /// <summary>
        /// 日志目录名称
        /// </summary>
        public const string LogDirectoryName = "log";

        #endregion

        #region 密码常量

        /// <summary>
        /// 默认管理员密码
        /// </summary>
        public const string DefaultPassword = "qiwei";

        #endregion

        #region 默认游戏状态值

        /// <summary>
        /// 默认习题总数
        /// </summary>
        public const int DefaultExamTotal = 15;

        /// <summary>
        /// 习题总数最小值（不允许负数）
        /// </summary>
        public const int MinExamTotal = 0;

        /// <summary>
        /// 默认初始金币数
        /// </summary>
        public const int DefaultCoinTotal = 0;

        /// <summary>
        /// 金币总数最小值（不允许负数）
        /// </summary>
        public const int MinCoinTotal = 0;

        /// <summary>
        /// 默认正确数量
        /// </summary>
        public const int DefaultCorrectCount = 0;

        /// <summary>
        /// 默认错误数量
        /// </summary>
        public const int DefaultWrongCount = 0;

        #endregion

        #region 暂停机制常量

        /// <summary>
        /// 默认暂停类型：1=限时间暂停，对应 PauseType.ByTime
        /// </summary>
        public const int DefaultPauseType = 1;

        /// <summary>
        /// 默认可用暂停次数（限次数模式）
        /// </summary>
        public const int DefaultAllowPauseCount = 5;

        /// <summary>
        /// 默认初始剩余暂停时间（秒，限时模式）
        /// </summary>
        public const int DefaultPauseSecondsLeft = 600;

        /// <summary>
        /// 暂停类型：限次数暂停，对应 PauseType.ByCount
        /// 注意：建议使用枚举类型 PauseType.ByCount 而非此常量
        /// </summary>
        [Obsolete("请使用枚举类型 PauseType.ByCount 代替此常量")]
        public const int PauseTypeByCount = 0;

        /// <summary>
        /// 暂停类型：限时间暂停，对应 PauseType.ByTime
        /// 注意：建议使用枚举类型 PauseType.ByTime 而非此常量
        /// </summary>
        [Obsolete("请使用枚举类型 PauseType.ByTime 代替此常量")]
        public const int PauseTypeByTime = 1;

        #endregion

        #region 金币奖励与消费常量

        /// <summary>
        /// 答对一题默认奖励金币数
        /// </summary>
        public const int DefaultAwardCoinPerCorrect = 3;

        /// <summary>
        /// 全对通关额外奖励系数（额外金币 = 初始题数 × 此系数）
        /// </summary>
        public const double FullAnswerBonusCoefficient = 0.5;

        /// <summary>
        /// 默认检查辅助框花费金币
        /// </summary>
        public const int DefaultCostCoinCheck = 1;

        /// <summary>
        /// 默认给出答案花费金币
        /// </summary>
        public const int DefaultCostCoinGive = 3;

        #endregion

        #region 惩罚系统常量

        /// <summary>
        /// 默认做错惩罚加题数量
        /// </summary>
        public const int DefaultPunishmentAddQuestions = 2;

        /// <summary>
        /// 默认超时惩罚加题数量
        /// </summary>
        public const int DefaultPunishmentTimeout = 1;

        #endregion

        #region 辅助框常量

        /// <summary>
        /// 默认是否启用辅助框
        /// </summary>
        public const bool DefaultHelpBoxEnabled = true;

        /// <summary>
        /// 默认辅助框是否免费使用
        /// </summary>
        public const bool DefaultHelpBoxFree = false;

        /// <summary>
        /// 默认错误消息显示时间（秒）
        /// </summary>
        public const int DefaultErrorMessageShowTime = 5;

        #endregion

        #region 题目配置常量

        /// <summary>
        /// 题目默认百分比
        /// </summary>
        public const int DefaultQuestionPercent = 25;

        /// <summary>
        /// 题目默认总数量
        /// </summary>
        public const int DefaultQuestionTotalQty = 300;

        /// <summary>
        /// 题目默认整数位数
        /// </summary>
        public const int DefaultQuestionIntBits = 3;

        /// <summary>
        /// 题目默认小数位数
        /// </summary>
        public const int DefaultQuestionDecBits = 0;

        /// <summary>
        /// 题目默认时间限制（秒）
        /// </summary>
        public const int DefaultQuestionTimeLimit = 120;

        /// <summary>
        /// 题目类型总数
        /// </summary>
        public const int TotalQuestionTypes = 5;

        #endregion

        #region 配置服务常量

        /// <summary>
        /// INI 文件读取默认字符串缓冲区大小
        /// </summary>
        public const int DefaultIniStringBufferSize = 500;

        /// <summary>
        /// INI 文件读取整型默认值
        /// </summary>
        public const int DefaultIniIntValue = 50;

        #endregion

        #region 日志系统常量

        /// <summary>
        /// 默认日志文件最大大小（字节）
        /// </summary>
        public const long DefaultLogMaxFileSize = 10 * 1024 * 1024;

        /// <summary>
        /// 日志文件操作最大重试次数
        /// </summary>
        public const int LogFileMaxRetries = 10;

        /// <summary>
        /// 日志文件操作重试等待时间基数（毫秒）
        /// </summary>
        public const int LogFileRetryWaitMs = 100;

        /// <summary>
        /// 是否默认启用调试级别日志
        /// </summary>
        public const bool DefaultEnableDebugLogging = true;

        #endregion

        #region 进程监控常量

        /// <summary>
        /// 进程黑名单（不允许运行的进程名称）
        /// </summary>
        public static readonly string[] ProcessBlackList = new string[]
        {
            "Calculator",
            "calc",
            "ApplicationFrameHost",
            "SogouExplorer",
            "msedge",
            "360se"
        };

        #endregion

        #region UI 文本常量

        /// <summary>
        /// 暂停次数单位文本
        /// </summary>
        public const string PauseUnitCount = "次";

        /// <summary>
        /// 暂停时间单位文本
        /// </summary>
        public const string PauseUnitTime = "秒";

        /// <summary>
        /// 余数标签文本
        /// </summary>
        public const string RemainderLabel = "余数：";

        #endregion

        #region 游戏状态常量

        /// <summary>
        /// 默认是否已完成
        /// </summary>
        public const bool DefaultFinished = false;

        /// <summary>
        /// 默认是否暂停
        /// </summary>
        public const bool DefaultPaused = false;

        /// <summary>
        /// 默认当前题型索引
        /// </summary>
        public const int DefaultCurrentTypeIndex = 0;

        #endregion

        #region 配置项范围验证常量

        /// <summary>
        /// 暂停类型最小值
        /// </summary>
        public const int MinPauseType = 0;

        /// <summary>
        /// 暂停类型最大值
        /// </summary>
        public const int MaxPauseType = 1;

        /// <summary>
        /// 初始剩余暂停时间最小值（秒）
        /// </summary>
        public const int MinPauseSecondsLeft = 60;

        /// <summary>
        ///  初始剩余暂停时间最大值（秒）
        /// </summary>
        public const int MaxPauseSecondsLeft = 1800;

        /// <summary>
        /// 可用暂停次数最小值
        /// </summary>
        public const int MinAllowPauseCount = 0;

        /// <summary>
        /// 可用暂停次数最大值
        /// </summary>
        public const int MaxAllowPauseCount = 20;

        /// <summary>
        /// 错误消息显示时间最小值（秒）
        /// </summary>
        public const int MinErrorMessageShowTime = 1;

        /// <summary>
        /// 错误消息显示时间最大值（秒）
        /// </summary>
        public const int MaxErrorMessageShowTime = 30;

        /// <summary>
        /// 答对奖励金币最小值
        /// </summary>
        public const int MinAwardCoin = 1;

        /// <summary>
        /// 答对奖励金币最大值
        /// </summary>
        public const int MaxAwardCoin = 10;

        /// <summary>
        /// 检查辅助框花费金币最小值
        /// </summary>
        public const int MinCostCoinCheck = 0;

        /// <summary>
        /// 检查辅助框花费金币最大值
        /// </summary>
        public const int MaxCostCoinCheck = 5;

        /// <summary>
        /// 给出答案花费金币最小值
        /// </summary>
        public const int MinCostCoinGive = 0;

        /// <summary>
        /// 给出答案花费金币最大值
        /// </summary>
        public const int MaxCostCoinGive = 10;

        /// <summary>
        /// 做错惩罚加题数量最小值
        /// </summary>
        public const int MinPunishment = 1;

        /// <summary>
        /// 做错惩罚加题数量最大值
        /// </summary>
        public const int MaxPunishment = 5;

        /// <summary>
        /// 超时惩罚加题数量最小值
        /// </summary>
        public const int MinPunishmentTimeout = 1;

        /// <summary>
        /// 超时惩罚加题数量最大值
        /// </summary>
        public const int MaxPunishmentTimeout = 5;

        #endregion
    }
}
