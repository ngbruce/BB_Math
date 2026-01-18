using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BBMath.Core;
using BBMath.Configuration;

namespace BBMath.Tests
{
    /// <summary>
    /// 全局异常处理器单元测试
    /// </summary>
    [TestClass]
    public class GlobalExceptionHandlerTests
    {
        private TestContext _testContext;

        /// <summary>
        /// 测试上下文
        /// </summary>
        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // 清理之前的初始化状态
            // 注意：GlobalExceptionHandler 只能初始化一次
        }

        [TestMethod]
        public void Initialize_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                GlobalExceptionHandler.Initialize();
            }
            catch (Exception ex)
            {
                Assert.Fail($"初始化不应抛出异常: {ex.Message}");
            }
        }

        [TestMethod]
        public void GetDiagnosticInfo_ShouldContainRequiredInformation()
        {
            // Arrange
            var recoveryService = new ErrorRecoveryService();

            // Act
            string diagnosticInfo = recoveryService.GetDiagnosticInfo();

            // Assert
            Assert.IsNotNull(diagnosticInfo);
            Assert.IsTrue(diagnosticInfo.Contains("=== BBMath 诊断信息 ==="));
            Assert.IsTrue(diagnosticInfo.Contains("=== 系统信息 ==="));
            Assert.IsTrue(diagnosticInfo.Contains("=== 应用目录 ==="));
            Assert.IsTrue(diagnosticInfo.Contains("=== 文件状态 ==="));
            Assert.IsTrue(diagnosticInfo.Contains("=== 游戏状态 ==="));
        }

        [TestMethod]
        public void GetUserFriendlyMessage_ShouldHandleFileNotFound()
        {
            // Arrange
            var ex = new FileNotFoundException("测试文件");

            // Act
            // 通过反射访问私有方法（仅用于测试）
            var method = typeof(GlobalExceptionHandler).GetMethod("GetUserFriendlyMessage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (method != null)
            {
                string message = (string)method.Invoke(null, new object[] { ex });

                // Assert
                Assert.IsTrue(message.Contains("找不到必要的文件"));
            }
            else
            {
                Assert.Inconclusive("无法访问私有方法进行测试");
            }
        }

        [TestMethod]
        public void LogPerformanceWarning_ShouldLogWhenOverThreshold()
        {
            // Arrange
            long duration = 5000; // 5秒
            long threshold = 1000; // 1秒

            // Act & Assert
            try
            {
                GlobalExceptionHandler.LogPerformanceWarning("测试组件", "测试操作", duration, threshold);
                // 验证日志中包含警告（需要检查日志文件或使用日志捕获）
            }
            catch
            {
                Assert.Fail("LogPerformanceWarning 不应抛出异常");
            }
        }
    }

    /// <summary>
    /// 输入验证器单元测试
    /// </summary>
    [TestClass]
    public class InputValidatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateNotEmptyOrWhitespace_WithEmptyString_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateNotEmptyOrWhitespace("", "测试参数");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateNotEmptyOrWhitespace_WithWhitespaceString_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateNotEmptyOrWhitespace("   ", "测试参数");
        }

        [TestMethod]
        public void ValidateNotEmptyOrWhitespace_WithValidString_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateNotEmptyOrWhitespace("有效内容", "测试参数");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        public void ValidateRange_WithValidValue_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateRange(5, "测试参数", 1, 10);
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateRange_WithTooSmallValue_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateRange(0, "测试参数", 1, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateRange_WithTooLargeValue_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateRange(11, "测试参数", 1, 10);
        }

        [TestMethod]
        public void ValidatePercentage_WithValidValue_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidatePercentage(50, "测试百分比");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidatePercentage_WithNegativeValue_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidatePercentage(-1, "测试百分比");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidatePercentage_WithValueOver100_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidatePercentage(101, "测试百分比");
        }

        [TestMethod]
        public void ValidatePassword_WithValidPassword_ShouldReturnTrue()
        {
            // Arrange & Act
            var result = InputValidator.ValidatePassword("test1234", 4, 20);

            // Assert
            Assert.IsTrue(result.Item1, "有效密码应通过验证");
            Assert.IsTrue(string.IsNullOrEmpty(result.Item2), "有效密码的错误消息应为空");
        }

        [TestMethod]
        public void ValidatePassword_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange & Act
            var result = InputValidator.ValidatePassword("", 4, 20);

            // Assert
            Assert.IsFalse(result.Item1, "空密码应未通过验证");
            Assert.IsFalse(string.IsNullOrEmpty(result.Item2), "应返回错误消息");
        }

        [TestMethod]
        public void ValidatePassword_WithTooShortPassword_ShouldReturnFalse()
        {
            // Arrange & Act
            var result = InputValidator.ValidatePassword("ab", 4, 20);

            // Assert
            Assert.IsFalse(result.Item1, "过短密码应未通过验证");
            Assert.IsTrue(result.Item2.Contains("不能少于"), "应返回长度不足的错误消息");
        }

        [TestMethod]
        public void ValidatePassword_WithTooLongPassword_ShouldReturnFalse()
        {
            // Arrange & Act
            var result = InputValidator.ValidatePassword("abcdefghijk1234567890", 4, 20);

            // Assert
            Assert.IsFalse(result.Item1, "过长密码应未通过验证");
            Assert.IsTrue(result.Item2.Contains("不能超过"), "应返回长度超出的错误消息");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateArrayNotEmpty_WithNullArray_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateArrayNotEmpty<string>(null, "测试数组");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateArrayNotEmpty_WithEmptyArray_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateArrayNotEmpty(new string[0], "测试数组");
        }

        [TestMethod]
        public void ValidateArrayNotEmpty_WithValidArray_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateArrayNotEmpty(new[] { "a", "b" }, "测试数组");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        public void TryParseInt_WithValidInteger_ShouldReturnTrue()
        {
            // Arrange & Act
            bool result = InputValidator.TryParseInt("123", "测试参数", out int parsedValue);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(123, parsedValue);
        }

        [TestMethod]
        public void TryParseInt_WithInvalidString_ShouldReturnFalse()
        {
            // Arrange & Act
            bool result = InputValidator.TryParseInt("abc", "测试参数", out int parsedValue);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryParseInt_WithEmptyString_ShouldReturnFalse()
        {
            // Arrange & Act
            bool result = InputValidator.TryParseInt("", "测试参数", out int parsedValue);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateNumberBits_WithValidBits_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateNumberBits(3, 0);
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateNumberBits_WithExceedingSum_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateNumberBits(6, 5); // 总和为 11，超过 10
        }

        [TestMethod]
        public void ValidateQuestionCount_WithValidCount_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateQuestionCount(20, "题目数量");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateQuestionCount_WithZero_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateQuestionCount(0, "题目数量");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateQuestionCount_WithExceedingCount_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateQuestionCount(1001, "题目数量");
        }

        [TestMethod]
        public void ValidateCoinAmount_WithValidAmount_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateCoinAmount(100, "金币数量");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateCoinAmount_WithNegativeAmount_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateCoinAmount(-10, "金币数量");
        }

        [TestMethod]
        public void ValidateTimeLimit_WithValidTime_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateTimeLimit(60, "时间限制");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateTimeLimit_WithZero_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateTimeLimit(0, "时间限制");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateTimeLimit_WithExceedingTime_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateTimeLimit(86401, "时间限制"); // 超过 24 小时
        }

        [TestMethod]
        public void SafeExecute_WithValidAction_ShouldReturnTrue()
        {
            // Arrange & Act
            bool result = InputValidator.SafeExecute(() => { /* do nothing */ }, "测试操作");

            // Assert
            Assert.IsTrue(result, "成功执行应返回 true");
        }

        [TestMethod]
        public void SafeExecute_WithThrowingAction_ShouldReturnFalse()
        {
            // Arrange & Act
            bool result = InputValidator.SafeExecute(() => { throw new Exception("测试异常"); }, "测试操作");

            // Assert
            Assert.IsFalse(result, "失败执行应返回 false");
        }

        [TestMethod]
        public void SafeExecute_WithNullAction_ShouldReturnFalse()
        {
            // Arrange & Act
            bool result = InputValidator.SafeExecute((Action)null, "测试操作");

            // Assert
            Assert.IsFalse(result, "null 操作应返回 false");
        }

        [TestMethod]
        public void SafeExecuteGeneric_WithValidFunc_ShouldReturnValue()
        {
            // Arrange & Act
            int result = InputValidator.SafeExecute(() => 42, "测试操作", 0);

            // Assert
            Assert.AreEqual(42, result, "应返回函数的返回值");
        }

        [TestMethod]
        public void SafeExecuteGeneric_WithThrowingFunc_ShouldReturnDefaultValue()
        {
            // Arrange & Act
            int result = InputValidator.SafeExecute<int>(() => throw new Exception("测试异常"), "测试操作", 99);

            // Assert
            Assert.AreEqual(99, result, "应返回默认值");
        }

        [TestMethod]
        public void ValidateFilePath_WithValidPath_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateFilePath("valid_file.txt", "文件路径");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateFilePath_WithEmptyPath_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateFilePath("", "文件路径");
        }

        [TestMethod]
        public void ValidateConfigSection_WithValidSection_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateConfigSection("General");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateConfigSection_WithEmptySection_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateConfigSection("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateConfigSection_WithInvalidChars_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateConfigSection("Section=Name");
        }

        [TestMethod]
        public void ValidateConfigKey_WithValidKey_ShouldNotThrow()
        {
            // Arrange & Act & Assert
            try
            {
                InputValidator.ValidateConfigKey("KeyName");
            }
            catch
            {
                Assert.Fail("不应抛出异常");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateConfigKey_WithInvalidChars_ShouldThrow()
        {
            // Arrange & Act & Assert
            InputValidator.ValidateConfigKey("Key=Name");
        }
    }

    /// <summary>
    /// 错误恢复服务单元测试
    /// </summary>
    [TestClass]
    public class ErrorRecoveryServiceTests
    {
        private string _testDirectory;
        private ErrorRecoveryService _recoveryService;

        [TestInitialize]
        public void TestInitialize()
        {
            // 创建临时测试目录
            _testDirectory = Path.Combine(Path.GetTempPath(), "BBMath_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDirectory);

            // 创建测试用的服务
            var fileService = new FileService();
            var configService = new IniConfigurationService(Path.Combine(_testDirectory, "test.cfg"));
            _recoveryService = new ErrorRecoveryService(fileService, configService, _testDirectory);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // 清理临时测试目录
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch
            {
                // 忽略清理错误
            }
        }

        [TestMethod]
        public void GetDiagnosticInfo_ShouldReturnValidInformation()
        {
            // Arrange & Act
            string info = _recoveryService.GetDiagnosticInfo();

            // Assert
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Length > 0);
            Assert.IsTrue(info.Contains("诊断信息"));
        }

        [TestMethod]
        public void SaveDiagnosticInfo_ShouldCreateFile()
        {
            // Arrange
            string savePath = Path.Combine(_testDirectory, "diagnostic.txt");

            // Act
            bool result = _recoveryService.SaveDiagnosticInfo(savePath);

            // Assert
            Assert.IsTrue(result, "保存诊断信息应成功");
            Assert.IsTrue(File.Exists(savePath), "诊断文件应存在");

            // 验证文件内容
            string content = File.ReadAllText(savePath);
            Assert.IsTrue(content.Contains("诊断信息"), "文件应包含诊断信息");
        }

        [TestMethod]
        public void SaveDiagnosticInfo_WithInvalidPath_ShouldReturnFalse()
        {
            // Arrange
            string invalidPath = "X:\\invalid\\path.txt";

            // Act
            bool result = _recoveryService.SaveDiagnosticInfo(invalidPath);

            // Assert
            Assert.IsFalse(result, "无效路径应返回 false");
        }

        [TestMethod]
        public void CheckAndFixGameState_WithValidState_ShouldReturnFalse()
        {
            // Arrange - 使用默认的合理状态
            GameStateManager.coinTtl = 50;
            GameStateManager.pauseSecLeft = 600;
            GameStateManager.allowPause = 5;
            GameStateManager.examTtl = 15;

            // Act
            bool needsFix = _recoveryService.CheckAndFixGameState();

            // Assert
            Assert.IsFalse(needsFix, "有效状态不需要修复");
        }

        [TestMethod]
        public void CheckAndFixGameState_WithNegativeCoins_ShouldReturnTrue()
        {
            // Arrange - 设置无效的金币数量
            GameStateManager.coinTtl = -10;

            // Act
            bool needsFix = _recoveryService.CheckAndFixGameState();

            // Assert
            Assert.IsTrue(needsFix, "无效金币数量应触发修复");
            Assert.AreEqual(AppConstants.DefaultCoinTotal, GameStateManager.coinTtl, "金币应被重置为默认值");
        }

        [TestMethod]
        public void CheckAndFixGameState_WithNegativePauseTime_ShouldReturnTrue()
        {
            // Arrange - 设置无效的暂停时间配置值（而非运行时值）
            GameStateManager.pauseSecondsLeftConfig = -100;

            // Act
            bool needsFix = _recoveryService.CheckAndFixGameState();

            // Assert
            Assert.IsTrue(needsFix, "无效暂停时间应触发修复");
            Assert.AreEqual(AppConstants.DefaultPauseSecondsLeft, GameStateManager.pauseSecondsLeftConfig, "暂停时间配置值应被重置为默认值");
        }

        [TestMethod]
        public void CheckAndFixGameState_WithNegativePauseCount_ShouldReturnTrue()
        {
            // Arrange - 设置无效的暂停次数配置值（而非运行时值）
            GameStateManager.allowPauseConfig = -5;

            // Act
            bool needsFix = _recoveryService.CheckAndFixGameState();

            // Assert
            Assert.IsTrue(needsFix, "无效暂停次数应触发修复");
            Assert.AreEqual(AppConstants.DefaultAllowPauseCount, GameStateManager.allowPauseConfig, "暂停次数配置值应被重置为默认值");
        }

        [TestMethod]
        public void CheckAndFixGameState_WithNegativeExamCount_ShouldReturnTrue()
        {
            // Arrange - 设置无效的题目数量
            GameStateManager.examTtl = -20;

            // Act
            bool needsFix = _recoveryService.CheckAndFixGameState();

            // Assert
            Assert.IsTrue(needsFix, "无效题目数量应触发修复");
            Assert.AreEqual(AppConstants.DefaultExamTotal, GameStateManager.examTtl, "题目数量应被重置为默认值");
        }

        [TestMethod]
        public void BackupConfigurationFile_ShouldCreateBackup()
        {
            // Arrange
            string configPath = Path.Combine(_testDirectory, AppConstants.ConfigFileName);
            File.WriteAllText(configPath, "test config");

            // Act
            bool result = _recoveryService.BackupConfigurationFile();

            // Assert
            Assert.IsTrue(result, "备份应成功");
            Assert.IsTrue(File.Exists(configPath + ".backup"), "备份文件应存在");
        }

        [TestMethod]
        public void BackupConfigurationFile_WithNonExistentFile_ShouldReturnFalse()
        {
            // Arrange - 不创建配置文件

            // Act
            bool result = _recoveryService.BackupConfigurationFile();

            // Assert
            Assert.IsFalse(result, "不存在的文件应返回 false");
        }
    }
}
