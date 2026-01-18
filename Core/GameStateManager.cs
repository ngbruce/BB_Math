using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BBMath.Configuration;

namespace BBMath.Core
{
    /// <summary>
    /// 游戏状态管理器 - 重构后的bbMath类
    /// </summary>
    public static class GameStateManager
    {
        /// <summary>
        /// 存放题目对象 ExamObject 的列表
        /// </summary>
        public static List<ExamObject> lstExamObjects;
        
        private static readonly object _stateLock = new object();
        
        private static AppSettings _appSettings;
        
        /// <summary>
        /// 应用程序配置实例
        /// </summary>
        public static AppSettings Settings
        {
            get
            {
                if (_appSettings == null)
                {
                    lock (_stateLock)
                    {
                        if (_appSettings == null)
                        {
                            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniFileName);
                            var configService = new IniConfigurationService(configFilePath);
                            _appSettings = new AppSettings(configService);
                        }
                    }
                }
                return _appSettings;
            }
        }
        
        public static int currentTypeIndex = 0;
        
        /// <summary>
        /// 保存Log的文件名
        /// </summary>
        public static readonly string fileName = AppConstants.LogDataFileName; 
        
        /// <summary>
        /// 保存配置的文件名
        /// </summary>
        public static string iniFileName = AppConstants.ConfigFileName; 
        
        /// <summary>
        /// 标记是否已完成
        /// </summary>
        public static bool finished = false;
        
        /// <summary>
        /// 习题总数
        /// </summary>
        public static int examTtl = AppConstants.DefaultExamTotal;
        
        /// <summary>
        /// 保存初始习题总数
        /// </summary>
        public static int examTtlRec = 0;
        
        /// <summary>
        /// 做错加题数量设置
        /// </summary>
        public static int punishment = AppConstants.DefaultPunishmentAddQuestions;
        
        /// <summary>
        /// 总正确数量统计
        /// </summary>
        public static int correct = AppConstants.DefaultCorrectCount;
        
        /// <summary>
        /// 总错误数量统计
        /// </summary>
        public static int wrong = AppConstants.DefaultWrongCount;
        
        /// <summary>
        /// 是否允许辅助框
        /// </summary>
        public static bool helpBox = AppConstants.DefaultHelpBoxEnabled;
        
        /// <summary>
        /// 辅助框免费使用
        /// </summary>
        public static bool helpBoxFree = AppConstants.DefaultHelpBoxFree;
        
        /// <summary>
        /// 箱子内金币数
        /// </summary>
        public static int coinTtl = AppConstants.DefaultCoinTotal;
        
        /// <summary>
        /// 辅助框检查花费金币
        /// </summary>
        public static int costCoinCheck = AppConstants.DefaultCostCoinCheck;
        
        /// <summary>
        /// 辅助框给出结果花费金币
        /// </summary>
        public static int costCoinGive = AppConstants.DefaultCostCoinGive;
        
        /// <summary>
        /// 答对一题增加金币
        /// </summary>
        public static int awardCoin = AppConstants.DefaultAwardCoinPerCorrect;
        
        /// <summary>
        /// 超时加题数量
        /// </summary>
        public static int punishmentTimeOut = AppConstants.DefaultPunishmentTimeout;
        
        public static string PSW = AppConstants.DefaultPassword;
        
        /// <summary>
        /// 是否暂停状态
        /// </summary>
        public static bool flagPause = AppConstants.DefaultPaused;
        
        /// <summary>
        /// 可用暂停次数（运行时，练习过程中会减少）
        /// </summary>
        public static int allowPause = AppConstants.DefaultAllowPauseCount;

        /// <summary>
        /// 配置的可用暂停次数（固定值，从配置文件读取）
        /// </summary>
        public static int allowPauseConfig = AppConstants.DefaultAllowPauseCount;
        
        /// <summary>
        /// 错误框显示时间
        /// </summary>
        public static int errorMsgShowTime = AppConstants.DefaultErrorMessageShowTime;
        
        /// <summary>
        /// 暂停类型：0=限次数暂停，1=限时间暂停
        /// </summary>
        public static int pauseType = AppConstants.DefaultPauseType;

        /// <summary>
        /// 获取暂停类型的枚举值
        /// </summary>
        /// <returns>PauseType 枚举值</returns>
        public static PauseType GetPauseTypeEnum()
        {
            return (PauseType)pauseType;
        }

        /// <summary>
        /// 设置暂停类型（使用枚举）
        /// </summary>
        /// <param name="type">PauseType 枚举值</param>
        public static void SetPauseTypeEnum(PauseType type)
        {
            pauseType = (int)type;
        }
        
        /// <summary>
        /// 剩余暂停时间（运行时，练习过程中会减少）
        /// </summary>
        public static int pauseSecLeft = AppConstants.DefaultPauseSecondsLeft;

        /// <summary>
        /// 配置的暂停时间（固定值，从配置文件读取）
        /// </summary>
        public static int pauseSecondsLeftConfig = AppConstants.DefaultPauseSecondsLeft;

        /// <summary>
        /// 当前题目难度
        /// </summary>
        public static Difficulty currentDifficulty = Difficulty.LV1;

        /// <summary>
        /// 属性：返回金币数
        /// </summary>
        public static int TotalCoin { get { return coinTtl; } }
        
        /// <summary>
        /// 增加金币
        /// </summary>
        public static void AwardCoins(int amount)
        {
            if (amount < 0) throw new ArgumentException("金额不能为负数", nameof(amount));
            lock (_stateLock)
            {
                coinTtl += amount;
            }
        }
        
        /// <summary>
        /// 消费金币
        /// </summary>
        public static bool ConsumeCoins(int amount)
        {
            if (amount < 0) throw new ArgumentException("金额不能为负数", nameof(amount));
            lock (_stateLock)
            {
                if (coinTtl >= amount)
                {
                    coinTtl -= amount;
                    return true;
                }
                return false;
            }
        }
        
        /// <summary>
        /// 检查是否足够金币
        /// </summary>
        public static bool HasEnoughCoins(int amount)
        {
            if (amount < 0) throw new ArgumentException("金额不能为负数", nameof(amount));
            lock (_stateLock)
            {
                return coinTtl >= amount;
            }
        }
        
        /// <summary>
        /// 使用一次暂停（限次数模式）
        /// </summary>
        public static bool UsePause()
        {
            lock (_stateLock)
            {
                if ((PauseType)pauseType == PauseType.ByCount && allowPause > 0)
                {
                    allowPause--;
                    flagPause = true;
                    return true;
                }
                else if ((PauseType)pauseType == PauseType.ByTime && pauseSecLeft > 0)
                {
                    // 限时模式，暂停状态由计时器管理
                    flagPause = true;
                    return true;
                }
                return false;
            }
        }
        
        /// <summary>
        /// 恢复暂停
        /// </summary>
        public static void ResumePause()
        {
            lock (_stateLock)
            {
                flagPause = false;
            }
        }
        
        /// <summary>
        /// 更新剩余暂停时间（限时模式）
        /// </summary>
        public static void UpdatePauseTime(int secondsElapsed)
        {
            lock (_stateLock)
            {
                if ((PauseType)pauseType == PauseType.ByTime)
                {
                    pauseSecLeft = Math.Max(0, pauseSecLeft - secondsElapsed);
                }
            }
        }
        
        /// <summary>
        /// 获取剩余暂停次数或时间
        /// </summary>
        public static int GetRemainingPause()
        {
            lock (_stateLock)
            {
                return (PauseType)pauseType == PauseType.ByCount ? allowPause : pauseSecLeft;
            }
        }
        
        /// <summary>
        /// 黑名单数组 Process.ProcessName
        /// </summary>
        public static string[] strProcessBlackList = AppConstants.ProcessBlackList;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        static GameStateManager()
        {
            // 初始化代码
        }
        
        /// <summary>
        /// 计算每种类题目数量，有必要时更新总数
        /// </summary>
        public static void UpdateExamQty()
        {
            int ttlOfSeetting = 0;
            int ttlExceptLast = 0;
            
            for (int i = 0; i < ExamObject.TotalTypeQty; i++)
            {
                ttlOfSeetting += lstExamObjects[i].Percent;
            }
            
            for (int i = 0; i < (ExamObject.TotalTypeQty - 1); i++)
            {
                lstExamObjects[i].TotalQty = examTtl * lstExamObjects[i].Percent / ttlOfSeetting;
                ttlExceptLast += lstExamObjects[i].TotalQty;
                LoggerHelper.Print("examType[" + i.ToString() + "] =" + lstExamObjects[i].TotalQty.ToString());
            }
            
            lstExamObjects[ExamObject.TotalTypeQty - 1].TotalQty = examTtl - ttlExceptLast;
            LoggerHelper.Print("examType[" + (ExamObject.TotalTypeQty - 1).ToString() + "] =" + lstExamObjects[ExamObject.TotalTypeQty - 1].TotalQty.ToString());
        }
        
        /// <summary>
        /// 从配置文件读取，初始化各项设置
        /// </summary>
        public static void InitSettings()
        {
            // 确保 AppSettings 使用正确的配置文件
            lock (_stateLock)
            {
                if (_appSettings == null)
                {
                    string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniFileName);
                    var configService = new IniConfigurationService(configFilePath);
                    _appSettings = new AppSettings(configService);
                }
            }
            
            // 检查配置文件是否存在，如果不存在则创建并写入默认值
            string configFilePathToCheck = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniFileName);
            if (!File.Exists(configFilePathToCheck))
            {
                LoggerHelper.Print($"配置文件不存在，创建并写入默认值: {configFilePathToCheck}\r\n");
                
                // 先确保所有静态字段有正确的默认值
                EnsureStaticFieldsHaveDefaultValues();
                
                // 然后保存到配置文件
                SaveProgSettings();
                
                // 写入后直接返回，因为默认值已设置，无需再读取
                LoggerHelper.Print("默认配置已保存\r\n");

                // 初始化默认题目设置
                lstExamObjects = CreateDefaultExamObjects();
                return;
            }
            else
            {
                // 配置文件存在，先加载配置到静态字段
                try
                {
                    // 从配置加载值到静态字段
                    lock (_stateLock)
                    {
                        // 密码
                        PSW = Settings.Password;

                        // 金币总数
                        coinTtl = Settings.CoinTotal;

                        // 辅助框设置
                        helpBox = Settings.HelpBox;
                        helpBoxFree = Settings.HelpBoxFree;

                        // 暂停设置
                        pauseType = Settings.PauseType;
                        pauseSecondsLeftConfig = Settings.PauseSecondsLeft;
                        pauseSecLeft = pauseSecondsLeftConfig; // 初始时剩余时间等于配置时间
                        allowPauseConfig = Settings.AllowPauseCount;
                        allowPause = allowPauseConfig; // 初始时可用次数等于配置次数

                        // 错误消息显示时间
                        errorMsgShowTime = Settings.ErrorMessageShowTime;

                        // 奖励和消费设置
                        awardCoin = Settings.AwardCoinPerCorrect;
                        costCoinCheck = Settings.CostCoinCheck;
                        costCoinGive = Settings.CostCoinGive;

                        // 惩罚设置
                        punishment = Settings.PunishmentAddQuestions;
                        punishmentTimeOut = Settings.PunishmentTimeout;
                    }

                    LoggerHelper.Print("配置加载成功\r\n");

                    // 验证配置值范围，超出范围时使用默认值
                    ValidateConfigurationRanges();

                    // 加载成功后，检查并确保所有必需的配置项都有正确的默认值
                    EnsureDefaultConfigurationValues();

                    // 同步 pauseSecondsLeftConfig 和 pauseSecLeft
                    pauseSecondsLeftConfig = Settings.PauseSecondsLeft;
                    pauseSecLeft = pauseSecondsLeftConfig;

                    // 同步 allowPauseConfig 和 allowPause
                    allowPauseConfig = Settings.AllowPauseCount;
                    allowPause = allowPauseConfig;
                }
                catch (Exception ex)
                {
                    LoggerHelper.Print($"配置加载失败: {ex.Message}\r\n");
                    // 使用默认值，并保存默认配置
                    SaveProgSettings();
                }

                // 初始化默认题目设置
                lstExamObjects = CreateDefaultExamObjects();
            }
        }

        /// <summary>
        /// 创建默认的题目对象列表
        /// </summary>
        private static List<ExamObject> CreateDefaultExamObjects()
        {
            // 创建5个ExamObject，对应所有5种题型
            ExamObject objAddition = new ExamObject(AppConstants.DefaultQuestionPercent, AppConstants.DefaultQuestionTotalQty, false, "整数加法", ExamType.Addition, AppConstants.DefaultQuestionIntBits, AppConstants.DefaultQuestionDecBits, false);
            objAddition.Percent = AppConstants.DefaultQuestionPercent;
            objAddition.Description = objAddition.Name;
            objAddition.IntBits = AppConstants.DefaultQuestionIntBits;
            objAddition.DecBits = AppConstants.DefaultQuestionDecBits;
            objAddition.AllowNegativeResult = false;
            objAddition.TimeLimit = AppConstants.DefaultQuestionTimeLimit;

            ExamObject objSubtraction = new ExamObject(AppConstants.DefaultQuestionPercent, AppConstants.DefaultQuestionTotalQty, false, "整数减法", ExamType.Subtraction, AppConstants.DefaultQuestionIntBits, AppConstants.DefaultQuestionDecBits, false);
            objSubtraction.Percent = AppConstants.DefaultQuestionPercent;
            objSubtraction.Description = objSubtraction.Name;
            objSubtraction.IntBits = AppConstants.DefaultQuestionIntBits;
            objSubtraction.DecBits = AppConstants.DefaultQuestionDecBits;
            objSubtraction.AllowNegativeResult = false;
            objSubtraction.TimeLimit = AppConstants.DefaultQuestionTimeLimit;

            ExamObject objMultiplication = new ExamObject(AppConstants.DefaultQuestionPercent, AppConstants.DefaultQuestionTotalQty, false, "整数乘法", ExamType.Multiplication, AppConstants.DefaultQuestionIntBits, AppConstants.DefaultQuestionDecBits, false);
            objMultiplication.Percent = AppConstants.DefaultQuestionPercent;
            objMultiplication.Description = objMultiplication.Name;
            objMultiplication.IntBits = AppConstants.DefaultQuestionIntBits;
            objMultiplication.DecBits = AppConstants.DefaultQuestionDecBits;
            objMultiplication.AllowNegativeResult = false;
            objMultiplication.TimeLimit = AppConstants.DefaultQuestionTimeLimit;

            ExamObject objDivisionNoRemainder = new ExamObject(AppConstants.DefaultQuestionPercent, AppConstants.DefaultQuestionTotalQty, false, "整数无余数除法", ExamType.DivisionNoRemainder, AppConstants.DefaultQuestionIntBits, AppConstants.DefaultQuestionDecBits, false);
            objDivisionNoRemainder.Percent = AppConstants.DefaultQuestionPercent;
            objDivisionNoRemainder.Description = objDivisionNoRemainder.Name;
            objDivisionNoRemainder.IntBits = AppConstants.DefaultQuestionIntBits;
            objDivisionNoRemainder.DecBits = AppConstants.DefaultQuestionDecBits;
            objDivisionNoRemainder.AllowNegativeResult = false;
            objDivisionNoRemainder.TimeLimit = AppConstants.DefaultQuestionTimeLimit;

            ExamObject objDivisionWithRemainder = new ExamObject(AppConstants.DefaultQuestionPercent, AppConstants.DefaultQuestionTotalQty, false, "整数有余数除法", ExamType.DivisionWithRemainder, AppConstants.DefaultQuestionIntBits, AppConstants.DefaultQuestionDecBits, false);
            objDivisionWithRemainder.Percent = AppConstants.DefaultQuestionPercent;
            objDivisionWithRemainder.Description = objDivisionWithRemainder.Name;
            objDivisionWithRemainder.IntBits = AppConstants.DefaultQuestionIntBits;
            objDivisionWithRemainder.DecBits = AppConstants.DefaultQuestionDecBits;
            objDivisionWithRemainder.AllowNegativeResult = false;
            objDivisionWithRemainder.TimeLimit = AppConstants.DefaultQuestionTimeLimit;

            // 设置题型总数，使用常量而非立即数
            ExamObject.TotalTypeQty = AppConstants.TotalQuestionTypes;

            return new List<ExamObject>(AppConstants.TotalQuestionTypes)
            {
                objAddition,
                objSubtraction,
                objMultiplication,
                objDivisionNoRemainder,
                objDivisionWithRemainder
            };
        }

        /// <summary>
        /// 验证配置值范围，超出范围时使用默认值
        /// </summary>
        private static void ValidateConfigurationRanges()
        {
            bool hasInvalidValue = false;

            // 验证布尔类型配置项（helpBox 和 helpBoxFree）
            // 由于布尔配置使用 ReadBool 方法，如果配置文件中的值不是有效的布尔值（如 "3"、"4"），
            // ReadBool 会返回默认值 false。我们需要检测这种情况并记录警告。
            // 注意：ReadBool 方法会尝试解析 "1"/"0"/"true"/"false"，只有这些是有效的布尔表示
            bool invalidHelpBox = false;
            bool invalidHelpBoxFree = false;

            // 重新读取配置文件中的原始值来验证
            try
            {
                var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniFileName);
                var tempConfigService = new IniConfigurationService(configFilePath);
                string rawHelpBox = tempConfigService.ReadValue("APP", "helpBox");
                string rawHelpBoxFree = tempConfigService.ReadValue("APP", "helpBoxFree");

                // 检查 helpBox 是否包含无效值
                if (string.IsNullOrEmpty(rawHelpBox))
                {
                    // 配置项不存在，检查当前值是否为默认值
                    if (helpBox != AppConstants.DefaultHelpBoxEnabled)
                    {
                        helpBox = AppConstants.DefaultHelpBoxEnabled;
                        LoggerHelper.Print($"配置项 helpBox 不存在，使用默认值: {AppConstants.DefaultHelpBoxEnabled}\r\n");
                        invalidHelpBox = true;
                    }
                }
                else if (!IsValidBoolValue(rawHelpBox))
                {
                    // 配置值不是有效的布尔表示
                    LoggerHelper.Print($"警告: helpBox 值 '{rawHelpBox}' 不是有效的布尔值，应为 true/false/1/0，已自动修正为 {AppConstants.DefaultHelpBoxEnabled}\r\n");
                    helpBox = AppConstants.DefaultHelpBoxEnabled;
                    invalidHelpBox = true;
                }

                // 检查 helpBoxFree 是否包含无效值
                if (string.IsNullOrEmpty(rawHelpBoxFree))
                {
                    // 配置项不存在，检查当前值是否为默认值
                    if (helpBoxFree != AppConstants.DefaultHelpBoxFree)
                    {
                        helpBoxFree = AppConstants.DefaultHelpBoxFree;
                        LoggerHelper.Print($"配置项 helpBoxFree 不存在，使用默认值: {AppConstants.DefaultHelpBoxFree}\r\n");
                        invalidHelpBoxFree = true;
                    }
                }
                else if (!IsValidBoolValue(rawHelpBoxFree))
                {
                    // 配置值不是有效的布尔表示
                    LoggerHelper.Print($"警告: helpBoxFree 值 '{rawHelpBoxFree}' 不是有效的布尔值，应为 true/false/1/0，已自动修正为 {AppConstants.DefaultHelpBoxFree}\r\n");
                    helpBoxFree = AppConstants.DefaultHelpBoxFree;
                    invalidHelpBoxFree = true;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Print($"验证布尔配置项时出错: {ex.Message}\r\n");
            }

            // 如果布尔配置项无效，标记需要保存
            if (invalidHelpBox || invalidHelpBoxFree)
            {
                hasInvalidValue = true;
            }

            // 验证暂停类型
            if (pauseType < AppConstants.MinPauseType || pauseType > AppConstants.MaxPauseType)
            {
                LoggerHelper.Print($"警告: pauseType 值 {pauseType} 超出范围 [{AppConstants.MinPauseType}, {AppConstants.MaxPauseType}]，使用默认值 {AppConstants.DefaultPauseType}\r\n");
                pauseType = AppConstants.DefaultPauseType;
                hasInvalidValue = true;
            }

            // 验证配置的暂停时间（pauseSecondsLeftConfig，而不是运行时值 pauseSecLeft）
            if (pauseSecondsLeftConfig < AppConstants.MinPauseSecondsLeft || pauseSecondsLeftConfig > AppConstants.MaxPauseSecondsLeft)
            {
                LoggerHelper.Print($"警告: pauseSecondsLeftConfig 值 {pauseSecondsLeftConfig} 超出范围 [{AppConstants.MinPauseSecondsLeft}, {AppConstants.MaxPauseSecondsLeft}]，使用默认值 {AppConstants.DefaultPauseSecondsLeft}\r\n");
                pauseSecondsLeftConfig = AppConstants.DefaultPauseSecondsLeft;
                pauseSecLeft = pauseSecondsLeftConfig; // 同步运行时值
                hasInvalidValue = true;
            }

            // 验证配置的可用暂停次数（allowPauseConfig，而不是运行时值 allowPause）
            if (allowPauseConfig < AppConstants.MinAllowPauseCount || allowPauseConfig > AppConstants.MaxAllowPauseCount)
            {
                LoggerHelper.Print($"警告: allowPauseConfig 值 {allowPauseConfig} 超出范围 [{AppConstants.MinAllowPauseCount}, {AppConstants.MaxAllowPauseCount}]，使用默认值 {AppConstants.DefaultAllowPauseCount}\r\n");
                allowPauseConfig = AppConstants.DefaultAllowPauseCount;
                allowPause = allowPauseConfig; // 同步运行时值
                hasInvalidValue = true;
            }

            // 验证错误消息显示时间
            if (errorMsgShowTime < AppConstants.MinErrorMessageShowTime || errorMsgShowTime > AppConstants.MaxErrorMessageShowTime)
            {
                LoggerHelper.Print($"警告: errorMsgShowTime 值 {errorMsgShowTime} 超出范围 [{AppConstants.MinErrorMessageShowTime}, {AppConstants.MaxErrorMessageShowTime}]，使用默认值 {AppConstants.DefaultErrorMessageShowTime}\r\n");
                errorMsgShowTime = AppConstants.DefaultErrorMessageShowTime;
                hasInvalidValue = true;
            }

            // 验证奖励金币
            if (awardCoin < AppConstants.MinAwardCoin || awardCoin > AppConstants.MaxAwardCoin)
            {
                LoggerHelper.Print($"警告: awardCoin 值 {awardCoin} 超出范围 [{AppConstants.MinAwardCoin}, {AppConstants.MaxAwardCoin}]，使用默认值 {AppConstants.DefaultAwardCoinPerCorrect}\r\n");
                awardCoin = AppConstants.DefaultAwardCoinPerCorrect;
                hasInvalidValue = true;
            }

            // 验证检查花费金币
            if (costCoinCheck < AppConstants.MinCostCoinCheck || costCoinCheck > AppConstants.MaxCostCoinCheck)
            {
                LoggerHelper.Print($"警告: costCoinCheck 值 {costCoinCheck} 超出范围 [{AppConstants.MinCostCoinCheck}, {AppConstants.MaxCostCoinCheck}]，使用默认值 {AppConstants.DefaultCostCoinCheck}\r\n");
                costCoinCheck = AppConstants.DefaultCostCoinCheck;
                hasInvalidValue = true;
            }

            // 验证给出答案花费金币
            if (costCoinGive < AppConstants.MinCostCoinGive || costCoinGive > AppConstants.MaxCostCoinGive)
            {
                LoggerHelper.Print($"警告: costCoinGive 值 {costCoinGive} 超出范围 [{AppConstants.MinCostCoinGive}, {AppConstants.MaxCostCoinGive}]，使用默认值 {AppConstants.DefaultCostCoinGive}\r\n");
                costCoinGive = AppConstants.DefaultCostCoinGive;
                hasInvalidValue = true;
            }

            // 验证惩罚加题数量
            if (punishment < AppConstants.MinPunishment || punishment > AppConstants.MaxPunishment)
            {
                LoggerHelper.Print($"警告: punishment 值 {punishment} 超出范围 [{AppConstants.MinPunishment}, {AppConstants.MaxPunishment}]，使用默认值 {AppConstants.DefaultPunishmentAddQuestions}\r\n");
                punishment = AppConstants.DefaultPunishmentAddQuestions;
                hasInvalidValue = true;
            }

            // 验证超时惩罚加题数量
            if (punishmentTimeOut < AppConstants.MinPunishmentTimeout || punishmentTimeOut > AppConstants.MaxPunishmentTimeout)
            {
                LoggerHelper.Print($"警告: punishmentTimeOut 值 {punishmentTimeOut} 超出范围 [{AppConstants.MinPunishmentTimeout}, {AppConstants.MaxPunishmentTimeout}]，使用默认值 {AppConstants.DefaultPunishmentTimeout}\r\n");
                punishmentTimeOut = AppConstants.DefaultPunishmentTimeout;
                hasInvalidValue = true;
            }

            // 如果有配置值超出范围，保存修正后的值
            if (hasInvalidValue)
            {
                LoggerHelper.Print("检测到配置值超出范围，已修正并保存\r\n");
                SaveProgSettings();
            }
        }

        /// <summary>
        /// 检查字符串是否是有效的布尔值表示（true/false/1/0）
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>是否是有效的布尔值</returns>
        private static bool IsValidBoolValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // 有效的布尔值表示
            string[] validValues = { "true", "false", "1", "0", "True", "False", "TRUE", "FALSE" };
            return Array.Exists(validValues, v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 确保所有静态字段有正确的默认值（用于首次创建配置文件时）
        /// </summary>
        private static void EnsureStaticFieldsHaveDefaultValues()
        {
            // 确保密码有默认值
            if (string.IsNullOrEmpty(PSW))
            {
                PSW = AppConstants.DefaultPassword;
                LoggerHelper.Print($"设置密码默认值: {AppConstants.DefaultPassword}\r\n");
            }

            // 确保金币有默认值
            if (coinTtl < AppConstants.MinCoinTotal)
            {
                coinTtl = AppConstants.DefaultCoinTotal;
                LoggerHelper.Print($"设置金币默认值: {AppConstants.DefaultCoinTotal}\r\n");
            }

            // 确保惩罚值有正确的默认值
            if (punishment < AppConstants.MinPunishment || punishment > AppConstants.MaxPunishment)
            {
                punishment = AppConstants.DefaultPunishmentAddQuestions;
                LoggerHelper.Print($"设置惩罚默认值: {AppConstants.DefaultPunishmentAddQuestions}\r\n");
            }

            if (punishmentTimeOut < AppConstants.MinPunishmentTimeout || punishmentTimeOut > AppConstants.MaxPunishmentTimeout)
            {
                punishmentTimeOut = AppConstants.DefaultPunishmentTimeout;
                LoggerHelper.Print($"设置超时惩罚默认值: {AppConstants.DefaultPunishmentTimeout}\r\n");
            }

            // 确保奖励和消费有默认值
            if (awardCoin < AppConstants.MinAwardCoin || awardCoin > AppConstants.MaxAwardCoin)
            {
                awardCoin = AppConstants.DefaultAwardCoinPerCorrect;
                LoggerHelper.Print($"设置奖励金币默认值: {AppConstants.DefaultAwardCoinPerCorrect}\r\n");
            }

            if (costCoinCheck < AppConstants.MinCostCoinCheck || costCoinCheck > AppConstants.MaxCostCoinCheck)
            {
                costCoinCheck = AppConstants.DefaultCostCoinCheck;
                LoggerHelper.Print($"设置检查花费默认值: {AppConstants.DefaultCostCoinCheck}\r\n");
            }

            if (costCoinGive < AppConstants.MinCostCoinGive || costCoinGive > AppConstants.MaxCostCoinGive)
            {
                costCoinGive = AppConstants.DefaultCostCoinGive;
                LoggerHelper.Print($"设置给出答案花费默认值: {AppConstants.DefaultCostCoinGive}\r\n");
            }

            // 确保其他配置有默认值
            if (errorMsgShowTime < AppConstants.MinErrorMessageShowTime || errorMsgShowTime > AppConstants.MaxErrorMessageShowTime)
            {
                errorMsgShowTime = AppConstants.DefaultErrorMessageShowTime;
                LoggerHelper.Print($"设置错误消息显示时间默认值: {AppConstants.DefaultErrorMessageShowTime}\r\n");
            }

            LoggerHelper.Print("静态字段默认值设置完成\r\n");
        }

        /// <summary>
        /// 确保配置文件中的所有必需配置项都有正确的默认值
        /// </summary>
        private static void EnsureDefaultConfigurationValues()
        {
            try
            {
                // 创建临时配置服务用于检查配置项是否存在
                var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniFileName);
                var tempConfigService = new IniConfigurationService(configFilePath);

                // 检查并设置惩罚相关配置的默认值（包括范围检查）
                if (!tempConfigService.Exists("APP", "punishment") ||
                    Settings.PunishmentAddQuestions < AppConstants.MinPunishment ||
                    Settings.PunishmentAddQuestions > AppConstants.MaxPunishment)
                {
                    Settings.PunishmentAddQuestions = AppConstants.DefaultPunishmentAddQuestions;
                    LoggerHelper.Print($"设置惩罚配置默认值: {AppConstants.DefaultPunishmentAddQuestions}\r\n");
                }

                if (!tempConfigService.Exists("APP", "punishmentTimeOut") ||
                    Settings.PunishmentTimeout < AppConstants.MinPunishmentTimeout ||
                    Settings.PunishmentTimeout > AppConstants.MaxPunishmentTimeout)
                {
                    Settings.PunishmentTimeout = AppConstants.DefaultPunishmentTimeout;
                    LoggerHelper.Print($"设置超时惩罚配置默认值: {AppConstants.DefaultPunishmentTimeout}\r\n");
                }

                // 检查并设置金币相关配置的默认值（包括范围检查）
                if (!tempConfigService.Exists("APP", "awardCoin") ||
                    Settings.AwardCoinPerCorrect < AppConstants.MinAwardCoin ||
                    Settings.AwardCoinPerCorrect > AppConstants.MaxAwardCoin)
                {
                    Settings.AwardCoinPerCorrect = AppConstants.DefaultAwardCoinPerCorrect;
                    LoggerHelper.Print($"设置奖励金币配置默认值: {AppConstants.DefaultAwardCoinPerCorrect}\r\n");
                }

                if (!tempConfigService.Exists("APP", "costCoinCheck") ||
                    Settings.CostCoinCheck < AppConstants.MinCostCoinCheck ||
                    Settings.CostCoinCheck > AppConstants.MaxCostCoinCheck)
                {
                    Settings.CostCoinCheck = AppConstants.DefaultCostCoinCheck;
                    LoggerHelper.Print($"设置检查花费金币配置默认值: {AppConstants.DefaultCostCoinCheck}\r\n");
                }

                if (!tempConfigService.Exists("APP", "costCoinGive") ||
                    Settings.CostCoinGive < AppConstants.MinCostCoinGive ||
                    Settings.CostCoinGive > AppConstants.MaxCostCoinGive)
                {
                    Settings.CostCoinGive = AppConstants.DefaultCostCoinGive;
                    LoggerHelper.Print($"设置给出答案花费金币配置默认值: {AppConstants.DefaultCostCoinGive}\r\n");
                }

                // 检查并设置其他重要配置的默认值（包括范围检查）
                if (!tempConfigService.Exists("APP", "errorMsgShowTime") ||
                    Settings.ErrorMessageShowTime < AppConstants.MinErrorMessageShowTime ||
                    Settings.ErrorMessageShowTime > AppConstants.MaxErrorMessageShowTime)
                {
                    Settings.ErrorMessageShowTime = AppConstants.DefaultErrorMessageShowTime;
                    LoggerHelper.Print($"设置错误消息显示时间默认值: {AppConstants.DefaultErrorMessageShowTime}\r\n");
                }

                if (!tempConfigService.Exists("APP", "pauseType") ||
                    Settings.PauseType < AppConstants.MinPauseType ||
                    Settings.PauseType > AppConstants.MaxPauseType)
                {
                    Settings.PauseType = AppConstants.DefaultPauseType;
                    LoggerHelper.Print($"设置暂停类型默认值: {AppConstants.DefaultPauseType}\r\n");
                }

                if (!tempConfigService.Exists("APP", "pauseSecLeft") ||
                    Settings.PauseSecondsLeft < AppConstants.MinPauseSecondsLeft ||
                    Settings.PauseSecondsLeft > AppConstants.MaxPauseSecondsLeft)
                {
                    Settings.PauseSecondsLeft = AppConstants.DefaultPauseSecondsLeft;
                    LoggerHelper.Print($"设置暂停时间默认值: {AppConstants.DefaultPauseSecondsLeft}\r\n");
                }

                if (!tempConfigService.Exists("APP", "allowPause") ||
                    Settings.AllowPauseCount < AppConstants.MinAllowPauseCount ||
                    Settings.AllowPauseCount > AppConstants.MaxAllowPauseCount)
                {
                    Settings.AllowPauseCount = AppConstants.DefaultAllowPauseCount;
                    LoggerHelper.Print($"设置可用暂停次数默认值: {AppConstants.DefaultAllowPauseCount}\r\n");
                }

                if (!tempConfigService.Exists("APP", "coinTtl") || Settings.CoinTotal < AppConstants.MinCoinTotal)
                {
                    Settings.CoinTotal = AppConstants.DefaultCoinTotal;
                    LoggerHelper.Print($"设置初始金币默认值: {AppConstants.DefaultCoinTotal}\r\n");
                }

                LoggerHelper.Print("配置默认值已检查并设置完成\r\n");
            }
            catch (Exception ex)
            {
                LoggerHelper.Print($"检查配置默认值时出错: {ex.Message}\r\n");
            }
        }
        
        /// <summary>
        /// 保存到配置文件
        /// </summary>
        public static void saveProgSettings()
        {
            SaveProgSettings();
        }
        
        /// <summary>
        /// 保存到配置文件（PascalCase版本）
        /// </summary>
        public static void SaveProgSettings()
        {
            try
            {
                // 将静态字段的值保存到 AppSettings
                lock (_stateLock)
                {
                    Settings.Password = PSW;
                    Settings.CoinTotal = coinTtl;
                    Settings.HelpBox = helpBox;
                    Settings.HelpBoxFree = helpBoxFree;
                    Settings.PauseType = pauseType;
                    Settings.PauseSecondsLeft = pauseSecondsLeftConfig;
                    Settings.AllowPauseCount = allowPauseConfig;
                    Settings.ErrorMessageShowTime = errorMsgShowTime;
                    Settings.AwardCoinPerCorrect = awardCoin;
                    Settings.CostCoinCheck = costCoinCheck;
                    Settings.CostCoinGive = costCoinGive;
                    Settings.PunishmentAddQuestions = punishment;
                    Settings.PunishmentTimeout = punishmentTimeOut;
                }
                
                LoggerHelper.Print("配置保存成功\r\n");
            }
            catch (Exception ex)
            {
                LoggerHelper.Print($"配置保存失败: {ex.Message}\r\n");
                LoggerHelper.Exception(ex);
                throw;
            }
        }
    }
}