using System;
using System.IO;
using System.Text;
using System.Threading;

namespace BBMath.Core
{
    /// <summary>
    /// 日志服务实现
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private static readonly object _lock = new object();
        private readonly string _logDirectory;
        private readonly string _logFilePrefix;
        private readonly long _maxFileSize;
        private readonly bool _enableDebug;
        private readonly IFileService _fileService;
        
        private string _currentLogFile;
        private StreamWriter _currentWriter;
        private long _currentFileSize;
        private DateTime _currentFileDate;

        /// <summary>
        /// 默认构造函数，使用应用程序根目录下的 log 子目录
        /// </summary>
        public LoggerService() : this(new FileService(), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppConstants.LogDirectoryName), AppConstants.LogFilePrefix, AppConstants.DefaultLogMaxFileSize, AppConstants.DefaultEnableDebugLogging)
        {
        }

        /// <summary>
        /// 自定义构造函数
        /// </summary>
        /// <param name="logDirectory">日志目录</param>
        /// <param name="logFilePrefix">日志文件前缀</param>
        /// <param name="maxFileSize">最大文件大小（字节）</param>
        /// <param name="enableDebug">是否启用调试级别日志</param>
        public LoggerService(string logDirectory, string logFilePrefix, long maxFileSize, bool enableDebug)
            : this(new FileService(), logDirectory, logFilePrefix, maxFileSize, enableDebug)
        {
        }

        /// <summary>
        /// 带依赖注入的构造函数
        /// </summary>
        /// <param name="fileService">文件服务</param>
        /// <param name="logDirectory">日志目录</param>
        /// <param name="logFilePrefix">日志文件前缀</param>
        /// <param name="maxFileSize">最大文件大小（字节）</param>
        /// <param name="enableDebug">是否启用调试级别日志</param>
        public LoggerService(IFileService fileService, string logDirectory, string logFilePrefix, long maxFileSize, bool enableDebug)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _logDirectory = logDirectory ?? throw new ArgumentNullException(nameof(logDirectory));
            _logFilePrefix = logFilePrefix ?? throw new ArgumentNullException(nameof(logFilePrefix));
            _maxFileSize = maxFileSize > 0 ? maxFileSize : AppConstants.DefaultLogMaxFileSize;
            _enableDebug = enableDebug;

            EnsureDirectoryExists();
            InitializeLogFile();
        }

        /// <summary>
        /// 记录调试级别日志
        /// </summary>
        public void Debug(string message)
        {
            if (!IsEnabled(LogLevel.Debug))
                return;

            LogInternal(LogLevel.Debug, message);
        }

        /// <summary>
        /// 记录信息级别日志
        /// </summary>
        public void Info(string message)
        {
            LogInternal(LogLevel.Info, message);
        }

        /// <summary>
        /// 记录警告级别日志
        /// </summary>
        public void Warning(string message)
        {
            LogInternal(LogLevel.Warning, message);
        }

        /// <summary>
        /// 记录错误级别日志
        /// </summary>
        public void Error(string message)
        {
            LogInternal(LogLevel.Error, message);
        }

        /// <summary>
        /// 记录严重级别日志
        /// </summary>
        public void Critical(string message)
        {
            LogInternal(LogLevel.Critical, message);
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        public void Exception(Exception ex, string message = null)
        {
            if (ex == null)
                return;

            var fullMessage = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
                fullMessage.Append(message).Append(": ");
            
            fullMessage.Append(ex.Message).Append(Environment.NewLine);
            fullMessage.Append("堆栈跟踪:").Append(Environment.NewLine);
            fullMessage.Append(ex.StackTrace);

            if (ex.InnerException != null)
            {
                fullMessage.Append(Environment.NewLine).Append("内部异常: ");
                fullMessage.Append(ex.InnerException.Message);
            }

            LogInternal(LogLevel.Error, fullMessage.ToString());
        }

        /// <summary>
        /// 记录带有格式字符串的日志
        /// </summary>
        public void Log(LogLevel level, string format, params object[] args)
        {
            if (!IsEnabled(level))
                return;

            string message;
            try
            {
                message = string.Format(format, args);
            }
            catch (FormatException)
            {
                message = format;
            }

            LogInternal(level, message);
        }

        /// <summary>
        /// 检查指定日志级别是否启用
        /// </summary>
        public bool IsEnabled(LogLevel level)
        {
            // 如果不启用调试级别，则过滤掉Debug日志
            if (level == LogLevel.Debug && !_enableDebug)
                return false;

            return true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    if (_currentWriter != null)
                    {
                        try
                        {
                            _currentWriter.Flush();
                            _currentWriter.Close();
                        }
                        finally
                        {
                            _currentWriter.Dispose();
                            _currentWriter = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~LoggerService()
        {
            Dispose(false);
        }

        #region 私有方法

        private void EnsureDirectoryExists()
        {
            _fileService.EnsureDirectoryExists(_logDirectory);
        }

        private void InitializeLogFile()
        {
            lock (_lock)
            {
                var today = DateTime.Today;
                _currentFileDate = today;
                _currentLogFile = GetLogFilePath(today);
                
                // 检查文件是否存在并获取当前大小
                _currentFileSize = 0;
                if (_fileService.FileExists(_currentLogFile))
                {
                    _currentFileSize = _fileService.GetFileSize(_currentLogFile);
                }

                // 如果文件大小超过限制，则进行轮转
                if (_currentFileSize >= _maxFileSize)
                {
                    RotateLogFile();
                }

                // 确保写入器已创建
                EnsureWriterCreated();
            }
        }

        private string GetLogFilePath(DateTime date)
        {
            return _fileService.CombinePaths(_logDirectory, $"{_logFilePrefix}_{date:yyyy-MM-dd}.log");
        }

        private void EnsureWriterCreated()
        {
            if (_currentWriter == null)
            {
                // 添加重试机制，处理文件被占用的情况
                int retryCount = 0;
                int maxRetries = AppConstants.LogFileMaxRetries;
                IOException lastException = null;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        // 使用 FileStream 允许文件共享
                        var fileStream = new FileStream(_currentLogFile,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.ReadWrite);
                        _currentWriter = new StreamWriter(fileStream, Encoding.UTF8);
                        _currentWriter.AutoFlush = true;
                        return;
                    }
                    catch (IOException ex)
                    {
                        lastException = ex;
                        retryCount++;
                        if (retryCount < maxRetries)
                        {
                            System.Threading.Thread.Sleep(AppConstants.LogFileRetryWaitMs * retryCount); // 逐步增加等待时间
                        }
                    }
                }

                throw new IOException($"无法打开日志文件 '{_currentLogFile}'（尝试了 {maxRetries} 次）", lastException);
            }
        }

        private void RotateLogFile()
        {
            if (_currentWriter != null)
            {
                _currentWriter.Flush();
                _currentWriter.Close();
                _currentWriter.Dispose();
                _currentWriter = null;
            }

            // 为当前日期生成带时间戳的新文件名，确保唯一性
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var baseFileName = _fileService.CombinePaths(_logDirectory, $"{_logFilePrefix}_{_currentFileDate:yyyy-MM-dd}_{timestamp}");
            var newFileName = baseFileName + ".log";
            
            // 如果目标文件已存在，添加递增后缀
            int counter = 1;
            while (_fileService.FileExists(newFileName))
            {
                newFileName = $"{baseFileName}_{counter++}.log";
            }
            
            if (_fileService.FileExists(_currentLogFile))
            {
                _fileService.MoveFile(_currentLogFile, newFileName);
            }

            _currentLogFile = GetLogFilePath(_currentFileDate);
            _currentFileSize = 0;
        }

        private void CheckAndRotateIfNeeded()
        {
            var today = DateTime.Today;
            
            // 如果日期变化，则切换到新的日志文件
            if (today != _currentFileDate)
            {
                lock (_lock)
                {
                    if (today != _currentFileDate)
                    {
                        if (_currentWriter != null)
                        {
                            _currentWriter.Flush();
                            _currentWriter.Close();
                            _currentWriter.Dispose();
                            _currentWriter = null;
                        }

                        _currentFileDate = today;
                        _currentLogFile = GetLogFilePath(today);
                        _currentFileSize = 0;
                    }
                }
            }

            // 如果文件大小超过限制，则进行轮转
            if (_currentFileSize >= _maxFileSize)
            {
                lock (_lock)
                {
                    if (_currentFileSize >= _maxFileSize)
                    {
                        RotateLogFile();
                    }
                }
            }
        }

        private void LogInternal(LogLevel level, string message)
        {
            CheckAndRotateIfNeeded();

            var logEntry = FormatLogEntry(level, message);
            var bytes = Encoding.UTF8.GetBytes(logEntry + Environment.NewLine);

            lock (_lock)
            {
                EnsureWriterCreated();
                _currentWriter.WriteLine(logEntry);
                _currentFileSize += bytes.Length;
            }
        }

        private string FormatLogEntry(LogLevel level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var levelStr = GetLevelString(level);

            return $"{timestamp} [{threadId}] {levelStr} - {message}";
        }

        private string GetLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return "DBG";
                case LogLevel.Info: return "INF";
                case LogLevel.Warning: return "WRN";
                case LogLevel.Error: return "ERR";
                case LogLevel.Critical: return "CRT";
                default: return "UNK";
            }
        }

        #endregion
    }
}