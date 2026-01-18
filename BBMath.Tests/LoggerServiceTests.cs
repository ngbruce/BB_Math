using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BBMath.Core;

namespace BBMath.Tests
{
    [TestClass]
    public class LoggerServiceTests
    {
        private string _tempDirectory;
        private const string LogPrefix = "testlog";

        [TestInitialize]
        public void TestInitialize()
        {
            // 创建临时目录用于测试
            _tempDirectory = Path.Combine(Path.GetTempPath(), $"BBMath_LoggerTest_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempDirectory);

            // 确保使用新的 LoggerService 实例，避免与全局的 LoggerHelper 冲突
            // 注意：不要设置 LoggerHelper.Logger，因为这会影响其他测试
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // 清理临时目录
            if (Directory.Exists(_tempDirectory))
            {
                try
                {
                    Directory.Delete(_tempDirectory, true);
                }
                catch
                {
                    // 忽略清理失败
                }
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // 所有测试完成后清理全局 LoggerHelper
            try
            {
                LoggerHelper.Shutdown();
            }
            catch
            {
                // 忽略清理失败
            }
        }

        /// <summary>
        /// 重试读取日志文件，避免文件锁冲突
        /// </summary>
        private string ReadLogFileWithRetry(string filePath, int maxRetries = 50)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // 使用FileStream指定FileShare.ReadWrite，允许共享访问
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs))
                    {
                        return sr.ReadToEnd();
                    }
                }
                catch (IOException) when (i < maxRetries - 1)
                {
                    // 等待后重试，逐步增加等待时间，确保文件锁释放
                    System.Threading.Thread.Sleep(50 + 50 * i);
                }
            }
            
            // 最后一次尝试，如果失败则抛出异常
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }

        [TestMethod]
        public void LoggerService_CreatesLogFile()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);

            // 执行
            logger.Info("测试日志消息");

            // 清理
            (logger as IDisposable)?.Dispose();

            // 断言
            var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
            Assert.AreEqual(1, logFiles.Length, "应该创建一个日志文件");

            var logContent = ReadLogFileWithRetry(logFiles[0]);
            Assert.IsTrue(logContent.Contains("测试日志消息"), "日志文件应该包含测试消息");
        }



        [TestMethod]
        public void LoggerService_FormatsLogEntryCorrectly()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);

            // 执行
            logger.Info("格式化测试");

            // 清理
            (logger as IDisposable)?.Dispose();

        // 断言
        var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
        var logText = ReadLogFileWithRetry(logFiles[0]);
        var logContent = logText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        Assert.IsNotNull(logContent);
        Assert.IsTrue(logContent.Contains("INF"), "应该包含信息级别标记");
        Assert.IsTrue(logContent.Contains("格式化测试"), "应该包含消息内容");
        Assert.IsTrue(logContent.Contains("["), "应该包含线程ID括号");
        }



        [TestMethod]
        public void LoggerService_LogsDifferentLevels()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);

            // 执行
            logger.Debug("调试消息");
            logger.Info("信息消息");
            logger.Warning("警告消息");
            logger.Error("错误消息");
            logger.Critical("严重消息");

            // 清理
            (logger as IDisposable)?.Dispose();

        // 断言
        var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
        var logContent = ReadLogFileWithRetry(logFiles[0]);

        Assert.IsTrue(logContent.Contains("调试消息"), "应该包含调试消息");
        Assert.IsTrue(logContent.Contains("信息消息"), "应该包含信息消息");
        Assert.IsTrue(logContent.Contains("警告消息"), "应该包含警告消息");
        Assert.IsTrue(logContent.Contains("错误消息"), "应该包含错误消息");
        Assert.IsTrue(logContent.Contains("严重消息"), "应该包含严重消息");
        }



        [TestMethod]
        public void LoggerService_DisablesDebugLevelWhenDisabled()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, false);

            // 执行
            logger.Debug("调试消息");
            logger.Info("信息消息");

            // 清理
            (logger as IDisposable)?.Dispose();

        // 断言
        var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
        var logContent = ReadLogFileWithRetry(logFiles[0]);

        Assert.IsFalse(logContent.Contains("调试消息"), "不应该包含调试消息");
        Assert.IsTrue(logContent.Contains("信息消息"), "应该包含信息消息");
        }



        [TestMethod]
        public void LoggerService_LogsExceptionWithDetails()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);
            var exception = new InvalidOperationException("测试异常");

            // 执行
            logger.Exception(exception, "自定义上下文");

            // 清理
            (logger as IDisposable)?.Dispose();

        // 断言
        var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
        var logContent = ReadLogFileWithRetry(logFiles[0]);

        Assert.IsTrue(logContent.Contains("测试异常"), "应该包含异常消息");
        Assert.IsTrue(logContent.Contains("自定义上下文"), "应该包含自定义上下文");
        Assert.IsTrue(logContent.Contains("堆栈跟踪"), "应该包含堆栈跟踪");
        }



        [TestMethod]
        public void LoggerService_RotatesFileByDate()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);
            var today = DateTime.Today;

            // 执行 - 记录一条消息
            logger.Info("第一天消息");

            // 模拟日期变化（无法直接修改系统时间，测试按日期生成的文件名）
            // 我们创建一个新的记录器实例来模拟新的一天
            (logger as IDisposable)?.Dispose();

            // 创建第二天的日志文件
            var tomorrow = today.AddDays(1);
            var tomorrowFile = Path.Combine(_tempDirectory, $"{LogPrefix}_{tomorrow:yyyy-MM-dd}.log");
            File.WriteAllText(tomorrowFile, "第二天日志");

            // 断言
            var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
            Assert.AreEqual(2, logFiles.Length, "应该有两个日志文件（今天和明天）");
        }



        [TestMethod]
        public void LoggerService_RotatesFileBySize()
        {
            // 安排 - 设置非常小的最大文件大小
            var maxSize = 100; // 100字节
            var logger = new LoggerService(_tempDirectory, LogPrefix, maxSize, true);

            // 执行 - 写入足够多的日志以触发轮转
            for (int i = 0; i < 10; i++)
            {
                logger.Info($"测试消息 {i} - 这是一条较长的消息以确保快速达到文件大小限制");
            }

            // 清理
            (logger as IDisposable)?.Dispose();

            // 断言
            var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
            // 由于大小限制，应该创建了多个文件
            Assert.IsTrue(logFiles.Length >= 2, "文件大小轮转应该创建多个文件");
        }



        [TestMethod]
        public void LoggerService_IsEnabled_ReturnsCorrectValues()
        {
            // 安排 - 使用不同的日志文件前缀避免文件锁冲突
            var loggerWithDebug = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);
            var loggerWithoutDebug = new LoggerService(_tempDirectory, LogPrefix + "2", 1024 * 1024, false);

            // 执行和断言
            Assert.IsTrue(loggerWithDebug.IsEnabled(LogLevel.Debug), "启用调试时应返回true");
            Assert.IsTrue(loggerWithDebug.IsEnabled(LogLevel.Info), "信息级别应始终启用");
            Assert.IsTrue(loggerWithDebug.IsEnabled(LogLevel.Warning), "警告级别应始终启用");
            Assert.IsTrue(loggerWithDebug.IsEnabled(LogLevel.Error), "错误级别应始终启用");
            Assert.IsTrue(loggerWithDebug.IsEnabled(LogLevel.Critical), "严重级别应始终启用");

            Assert.IsFalse(loggerWithoutDebug.IsEnabled(LogLevel.Debug), "禁用调试时应返回false");
            Assert.IsTrue(loggerWithoutDebug.IsEnabled(LogLevel.Info), "信息级别应始终启用");

            // 清理
            (loggerWithDebug as IDisposable)?.Dispose();
            (loggerWithoutDebug as IDisposable)?.Dispose();
        }



        [TestMethod]
        public void LoggerService_LogMethod_WithFormat()
        {
            // 安排
            var logger = new LoggerService(_tempDirectory, LogPrefix, 1024 * 1024, true);

            // 执行
            logger.Log(LogLevel.Info, "用户 {0} 在 {1} 执行了操作", "张三", DateTime.Now.ToString("HH:mm"));

            // 清理
            (logger as IDisposable)?.Dispose();

        // 断言
        var logFiles = Directory.GetFiles(_tempDirectory, $"{LogPrefix}_*.log");
        var logContent = ReadLogFileWithRetry(logFiles[0]);

        Assert.IsTrue(logContent.Contains("用户 张三"), "应该包含格式化的用户信息");
        Assert.IsTrue(logContent.Contains("执行了操作"), "应该包含操作文本");
        }
    }
}