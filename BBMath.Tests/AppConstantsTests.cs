using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BBMath.Core;

namespace BBMath.Tests
{
    /// <summary>
    /// AppConstants 类的单元测试
    /// </summary>
    [TestClass]
    public class AppConstantsTests
    {
        #region 文件名常量测试

        [TestMethod]
        public void ConfigFileName_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string configFileName = AppConstants.ConfigFileName;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(configFileName), "配置文件名不应为空");
            Assert.AreEqual("bbmath.cfg", configFileName, "配置文件名应为 bbmath.cfg");
        }

        [TestMethod]
        public void LogDataFileName_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string logDataFileName = AppConstants.LogDataFileName;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(logDataFileName), "日志数据文件名不应为空");
            Assert.AreEqual("bbmath.dat", logDataFileName, "日志数据文件名应为 bbmath.dat");
        }

        [TestMethod]
        public void LogFilePrefix_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string logFilePrefix = AppConstants.LogFilePrefix;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(logFilePrefix), "日志文件前缀不应为空");
            Assert.AreEqual("bbmath", logFilePrefix, "日志文件前缀应为 bbmath");
        }

        #endregion

        #region 密码常量测试

        [TestMethod]
        public void DefaultPassword_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string defaultPassword = AppConstants.DefaultPassword;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(defaultPassword), "默认密码不应为空");
            Assert.AreEqual("qiwei", defaultPassword, "默认密码应为 qiwei");
        }

        #endregion

        #region 默认游戏状态值测试

        [TestMethod]
        public void DefaultExamTotal_ShouldBePositive()
        {
            // Arrange & Act
            int defaultExamTotal = AppConstants.DefaultExamTotal;

            // Assert
            Assert.IsTrue(defaultExamTotal > 0, "默认习题总数应大于0");
            Assert.AreEqual(15, defaultExamTotal, "默认习题总数应为 15");
        }

        [TestMethod]
        public void DefaultCoinTotal_ShouldBePositive()
        {
            // Arrange & Act
            int defaultCoinTotal = AppConstants.DefaultCoinTotal;

            // Assert
            Assert.IsTrue(defaultCoinTotal >= 0, "默认金币数应大于等于0");
            Assert.AreEqual(0, defaultCoinTotal, "默认金币数应为 0");
        }

        [TestMethod]
        public void DefaultCorrectCount_ShouldBeZero()
        {
            // Arrange & Act
            int defaultCorrectCount = AppConstants.DefaultCorrectCount;

            // Assert
            Assert.AreEqual(0, defaultCorrectCount, "默认正确数量应为 0");
        }

        [TestMethod]
        public void DefaultWrongCount_ShouldBeZero()
        {
            // Arrange & Act
            int defaultWrongCount = AppConstants.DefaultWrongCount;

            // Assert
            Assert.AreEqual(0, defaultWrongCount, "默认错误数量应为 0");
        }

        #endregion

        #region 暂停机制常量测试

        [TestMethod]
        public void DefaultPauseType_ShouldBeValid()
        {
            // Arrange & Act
            int defaultPauseType = AppConstants.DefaultPauseType;

            // Assert
            Assert.IsTrue(defaultPauseType == (int)PauseType.ByCount || defaultPauseType == (int)PauseType.ByTime,
                "默认暂停类型应为 0 或 1");
            Assert.AreEqual(1, defaultPauseType, "默认暂停类型应为 1（限时模式）");
        }

        [TestMethod]
        public void DefaultAllowPauseCount_ShouldBePositive()
        {
            // Arrange & Act
            int defaultAllowPauseCount = AppConstants.DefaultAllowPauseCount;

            // Assert
            Assert.IsTrue(defaultAllowPauseCount >= 0, "默认允许暂停次数应大于等于0");
            Assert.AreEqual(5, defaultAllowPauseCount, "默认允许暂停次数应为 5");
        }

        [TestMethod]
        public void DefaultPauseSecondsLeft_ShouldBePositive()
        {
            // Arrange & Act
            int defaultPauseSecondsLeft = AppConstants.DefaultPauseSecondsLeft;

            // Assert
            Assert.IsTrue(defaultPauseSecondsLeft >= 0, "默认剩余暂停时间应大于等于0");
            Assert.AreEqual(600, defaultPauseSecondsLeft, "默认剩余暂停时间应为 600 秒");
        }

        [TestMethod]
        public void PauseTypeConstants_ShouldBeValid()
        {
            // Arrange & Act
            int pauseTypeByCount = (int)PauseType.ByCount;
            int pauseTypeByTime = (int)PauseType.ByTime;

            // Assert
            Assert.AreEqual(0, pauseTypeByCount, "限次数暂停类型应为 0");
            Assert.AreEqual(1, pauseTypeByTime, "限时间暂停类型应为 1");
            Assert.AreNotEqual(pauseTypeByCount, pauseTypeByTime, "两种暂停类型应不同");
        }

        [TestMethod]
        public void PauseTypeEnum_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            PauseType byCount = PauseType.ByCount;
            PauseType byTime = PauseType.ByTime;

            // Assert
            Assert.AreEqual(0, (int)byCount, "PauseType.ByCount 应等于 0");
            Assert.AreEqual(1, (int)byTime, "PauseType.ByTime 应等于 1");
        }

        [TestMethod]
        public void PauseTypeEnum_ShouldBeValidEnumType()
        {
            // Arrange & Act
            Type pauseType = typeof(PauseType);

            // Assert
            Assert.IsTrue(pauseType.IsEnum, "PauseType 应为枚举类型");
            Assert.AreEqual(typeof(int), System.Enum.GetUnderlyingType(pauseType), "PauseType 的基础类型应为 int");
        }

        [TestMethod]
        public void PauseTypeEnum_ShouldContainExpectedMembers()
        {
            // Arrange & Act
            var enumValues = System.Enum.GetValues(typeof(PauseType));

            // Assert
            Assert.AreEqual(2, enumValues.Length, "PauseType 应有 2 个成员");
            Assert.IsTrue(System.Enum.IsDefined(typeof(PauseType), PauseType.ByCount), "应包含 ByCount");
            Assert.IsTrue(System.Enum.IsDefined(typeof(PauseType), PauseType.ByTime), "应包含 ByTime");
        }

        #endregion

        #region 金币奖励与消费常量测试

        [TestMethod]
        public void DefaultAwardCoinPerCorrect_ShouldBePositive()
        {
            // Arrange & Act
            int defaultAwardCoin = AppConstants.DefaultAwardCoinPerCorrect;

            // Assert
            Assert.IsTrue(defaultAwardCoin > 0, "默认答对奖励金币应大于0");
            Assert.AreEqual(3, defaultAwardCoin, "默认答对奖励金币应为 3");
        }

        [TestMethod]
        public void DefaultCostCoinCheck_ShouldBePositive()
        {
            // Arrange & Act
            int defaultCostCoinCheck = AppConstants.DefaultCostCoinCheck;

            // Assert
            Assert.IsTrue(defaultCostCoinCheck >= 0, "默认检查花费金币应大于等于0");
            Assert.AreEqual(1, defaultCostCoinCheck, "默认检查花费金币应为 1");
        }

        [TestMethod]
        public void DefaultCostCoinGive_ShouldBePositive()
        {
            // Arrange & Act
            int defaultCostCoinGive = AppConstants.DefaultCostCoinGive;

            // Assert
            Assert.IsTrue(defaultCostCoinGive > 0, "默认给出答案花费金币应大于0");
            Assert.AreEqual(3, defaultCostCoinGive, "默认给出答案花费金币应为 3");
        }

        #endregion

        #region 惩罚系统常量测试

        [TestMethod]
        public void DefaultPunishmentAddQuestions_ShouldBePositive()
        {
            // Arrange & Act
            int defaultPunishment = AppConstants.DefaultPunishmentAddQuestions;

            // Assert
            Assert.IsTrue(defaultPunishment >= 0, "默认做错加题数量应大于等于0");
            Assert.AreEqual(2, defaultPunishment, "默认做错加题数量应为 2");
        }

        [TestMethod]
        public void DefaultPunishmentTimeout_ShouldBePositive()
        {
            // Arrange & Act
            int defaultPunishmentTimeout = AppConstants.DefaultPunishmentTimeout;

            // Assert
            Assert.IsTrue(defaultPunishmentTimeout >= 0, "默认超时加题数量应大于等于0");
            Assert.AreEqual(1, defaultPunishmentTimeout, "默认超时加题数量应为 1");
        }

        #endregion

        #region 辅助框常量测试

        [TestMethod]
        public void DefaultHelpBoxEnabled_ShouldBeValid()
        {
            // Arrange & Act
            bool defaultHelpBoxEnabled = AppConstants.DefaultHelpBoxEnabled;

            // Assert
            Assert.IsTrue(defaultHelpBoxEnabled, "默认应启用辅助框");
        }

        [TestMethod]
        public void DefaultHelpBoxFree_ShouldBeValid()
        {
            // Arrange & Act
            bool defaultHelpBoxFree = AppConstants.DefaultHelpBoxFree;

            // Assert
            Assert.IsFalse(defaultHelpBoxFree, "默认辅助框不应免费");
        }

        [TestMethod]
        public void DefaultErrorMessageShowTime_ShouldBePositive()
        {
            // Arrange & Act
            int defaultErrorMessageShowTime = AppConstants.DefaultErrorMessageShowTime;

            // Assert
            Assert.IsTrue(defaultErrorMessageShowTime > 0, "默认错误消息显示时间应大于0");
            Assert.AreEqual(5, defaultErrorMessageShowTime, "默认错误消息显示时间应为 5 秒");
        }

        #endregion

        #region 题目配置常量测试

        [TestMethod]
        public void DefaultQuestionPercent_ShouldBeValid()
        {
            // Arrange & Act
            int defaultQuestionPercent = AppConstants.DefaultQuestionPercent;

            // Assert
            Assert.IsTrue(defaultQuestionPercent > 0 && defaultQuestionPercent <= 100, "题目默认百分比应在 1-100 之间");
            Assert.AreEqual(25, defaultQuestionPercent, "题目默认百分比应为 25");
        }

        [TestMethod]
        public void DefaultQuestionTotalQty_ShouldBePositive()
        {
            // Arrange & Act
            int defaultQuestionTotalQty = AppConstants.DefaultQuestionTotalQty;

            // Assert
            Assert.IsTrue(defaultQuestionTotalQty > 0, "题目默认总数量应大于0");
            Assert.AreEqual(300, defaultQuestionTotalQty, "题目默认总数量应为 300");
        }

        [TestMethod]
        public void DefaultQuestionIntBits_ShouldBeValid()
        {
            // Arrange & Act
            int defaultQuestionIntBits = AppConstants.DefaultQuestionIntBits;

            // Assert
            Assert.IsTrue(defaultQuestionIntBits >= 0, "题目默认整数位数应大于等于0");
            Assert.AreEqual(3, defaultQuestionIntBits, "题目默认整数位数应为 3");
        }

        [TestMethod]
        public void DefaultQuestionDecBits_ShouldBeValid()
        {
            // Arrange & Act
            int defaultQuestionDecBits = AppConstants.DefaultQuestionDecBits;

            // Assert
            Assert.IsTrue(defaultQuestionDecBits >= 0, "题目默认小数位数应大于等于0");
            Assert.AreEqual(0, defaultQuestionDecBits, "题目默认小数位数应为 0");
        }

        [TestMethod]
        public void DefaultQuestionTimeLimit_ShouldBePositive()
        {
            // Arrange & Act
            int defaultQuestionTimeLimit = AppConstants.DefaultQuestionTimeLimit;

            // Assert
            Assert.IsTrue(defaultQuestionTimeLimit > 0, "题目默认时间限制应大于0");
            Assert.AreEqual(120, defaultQuestionTimeLimit, "题目默认时间限制应为 120 秒");
        }

        [TestMethod]
        public void TotalQuestionTypes_ShouldBePositive()
        {
            // Arrange & Act
            int totalQuestionTypes = AppConstants.TotalQuestionTypes;

            // Assert
            Assert.IsTrue(totalQuestionTypes > 0, "题目类型总数应大于0");
            Assert.AreEqual(5, totalQuestionTypes, "题目类型总数应为 5（加法、减法、乘法、无余数除法、有余数除法）");
        }

        #endregion

        #region 配置服务常量测试

        [TestMethod]
        public void DefaultIniStringBufferSize_ShouldBePositive()
        {
            // Arrange & Act
            int defaultIniStringBufferSize = AppConstants.DefaultIniStringBufferSize;

            // Assert
            Assert.IsTrue(defaultIniStringBufferSize > 0, "默认 INI 字符串缓冲区大小应大于0");
            Assert.AreEqual(500, defaultIniStringBufferSize, "默认 INI 字符串缓冲区大小应为 500");
        }

        [TestMethod]
        public void DefaultIniIntValue_ShouldBeValid()
        {
            // Arrange & Act
            int defaultIniIntValue = AppConstants.DefaultIniIntValue;

            // Assert
            Assert.IsTrue(defaultIniIntValue >= 0, "默认 INI 整型值应大于等于0");
            Assert.AreEqual(50, defaultIniIntValue, "默认 INI 整型值应为 50");
        }

        #endregion

        #region 日志系统常量测试

        [TestMethod]
        public void DefaultLogMaxFileSize_ShouldBePositive()
        {
            // Arrange & Act
            long defaultLogMaxFileSize = AppConstants.DefaultLogMaxFileSize;

            // Assert
            Assert.IsTrue(defaultLogMaxFileSize > 0, "默认日志文件最大大小应大于0");
            Assert.AreEqual(10 * 1024 * 1024, defaultLogMaxFileSize, "默认日志文件最大大小应为 10MB");
        }

        [TestMethod]
        public void LogFileMaxRetries_ShouldBePositive()
        {
            // Arrange & Act
            int logFileMaxRetries = AppConstants.LogFileMaxRetries;

            // Assert
            Assert.IsTrue(logFileMaxRetries > 0, "日志文件操作最大重试次数应大于0");
            Assert.AreEqual(10, logFileMaxRetries, "日志文件操作最大重试次数应为 10");
        }

        [TestMethod]
        public void LogFileRetryWaitMs_ShouldBePositive()
        {
            // Arrange & Act
            int logFileRetryWaitMs = AppConstants.LogFileRetryWaitMs;

            // Assert
            Assert.IsTrue(logFileRetryWaitMs > 0, "日志文件操作重试等待时间应大于0");
            Assert.AreEqual(100, logFileRetryWaitMs, "日志文件操作重试等待时间应为 100 毫秒");
        }

        [TestMethod]
        public void DefaultEnableDebugLogging_ShouldBeValid()
        {
            // Arrange & Act
            bool defaultEnableDebugLogging = AppConstants.DefaultEnableDebugLogging;

            // Assert
            Assert.IsTrue(defaultEnableDebugLogging, "默认应启用调试级别日志");
        }

        #endregion

        #region 进程监控常量测试

        [TestMethod]
        public void ProcessBlackList_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string[] processBlackList = AppConstants.ProcessBlackList;

            // Assert
            Assert.IsNotNull(processBlackList, "进程黑名单不应为 null");
            Assert.IsTrue(processBlackList.Length > 0, "进程黑名单不应为空");
        }

        [TestMethod]
        public void ProcessBlackList_ShouldContainExpectedProcesses()
        {
            // Arrange & Act
            string[] processBlackList = AppConstants.ProcessBlackList;

            // Assert
            CollectionAssert.Contains(processBlackList, "calc", "黑名单应包含 calc");
            CollectionAssert.Contains(processBlackList, "Calculator", "黑名单应包含 Calculator");
            CollectionAssert.Contains(processBlackList, "msedge", "黑名单应包含 msedge");
        }

        #endregion

        #region UI 文本常量测试

        [TestMethod]
        public void PauseUnitCount_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string pauseUnitCount = AppConstants.PauseUnitCount;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(pauseUnitCount), "暂停次数单位文本不应为空");
            Assert.AreEqual("次", pauseUnitCount, "暂停次数单位文本应为 '次'");
        }

        [TestMethod]
        public void PauseUnitTime_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string pauseUnitTime = AppConstants.PauseUnitTime;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(pauseUnitTime), "暂停时间单位文本不应为空");
            Assert.AreEqual("秒", pauseUnitTime, "暂停时间单位文本应为 '秒'");
        }

        [TestMethod]
        public void RemainderLabel_ShouldNotBeEmpty()
        {
            // Arrange & Act
            string remainderLabel = AppConstants.RemainderLabel;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(remainderLabel), "余数标签文本不应为空");
            Assert.AreEqual("余数：", remainderLabel, "余数标签文本应为 '余数：'");
        }

        #endregion

        #region 游戏状态常量测试

        [TestMethod]
        public void DefaultFinished_ShouldBeFalse()
        {
            // Arrange & Act
            bool defaultFinished = AppConstants.DefaultFinished;

            // Assert
            Assert.IsFalse(defaultFinished, "默认是否完成应为 false");
        }

        [TestMethod]
        public void DefaultPaused_ShouldBeFalse()
        {
            // Arrange & Act
            bool defaultPaused = AppConstants.DefaultPaused;

            // Assert
            Assert.IsFalse(defaultPaused, "默认是否暂停应为 false");
        }

        [TestMethod]
        public void DefaultCurrentTypeIndex_ShouldBeZero()
        {
            // Arrange & Act
            int defaultCurrentTypeIndex = AppConstants.DefaultCurrentTypeIndex;

            // Assert
            Assert.AreEqual(0, defaultCurrentTypeIndex, "默认当前题型索引应为 0");
        }

        #endregion
    }
}
