using System;

namespace BBMath.Core
{
    /// <summary>
    /// 日志助手类，提供全局日志访问
    /// </summary>
    public static class LoggerHelper
    {
        private static ILoggerService _logger;

        /// <summary>
        /// 获取或设置全局日志服务实例
        /// </summary>
        public static ILoggerService Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerService();
                }
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        /// <summary>
        /// 记录调试级别日志
        /// </summary>
        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// 记录调试级别日志（格式化）
        /// </summary>
        public static void Debug(string format, params object[] args)
        {
            Logger.Log(LogLevel.Debug, format, args);
        }

        /// <summary>
        /// 记录信息级别日志
        /// </summary>
        public static void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// 记录信息级别日志（格式化）
        /// </summary>
        public static void Info(string format, params object[] args)
        {
            Logger.Log(LogLevel.Info, format, args);
        }

        /// <summary>
        /// 记录警告级别日志
        /// </summary>
        public static void Warning(string message)
        {
            Logger.Warning(message);
        }

        /// <summary>
        /// 记录警告级别日志（格式化）
        /// </summary>
        public static void Warning(string format, params object[] args)
        {
            Logger.Log(LogLevel.Warning, format, args);
        }

        /// <summary>
        /// 记录错误级别日志
        /// </summary>
        public static void Error(string message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// 记录错误级别日志（格式化）
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            Logger.Log(LogLevel.Error, format, args);
        }

        /// <summary>
        /// 记录严重级别日志
        /// </summary>
        public static void Critical(string message)
        {
            Logger.Critical(message);
        }

        /// <summary>
        /// 记录严重级别日志（格式化）
        /// </summary>
        public static void Critical(string format, params object[] args)
        {
            Logger.Log(LogLevel.Critical, format, args);
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        public static void Exception(Exception ex, string message = null)
        {
            Logger.Exception(ex, message);
        }

        /// <summary>
        /// 记录异常信息（格式化）
        /// </summary>
        public static void Exception(Exception ex, string format, params object[] args)
        {
            var message = string.Format(format, args);
            Logger.Exception(ex, message);
        }

        /// <summary>
        /// 替换原有的 Debug.Print 方法，提供向后兼容
        /// </summary>
        public static void Print(string message)
        {
            // 将原来的Debug.Print转换为Info级别日志
            Logger.Info(message);
        }

        /// <summary>
        /// 替换原有的 Debug.Print 方法（格式化）
        /// </summary>
        public static void Print(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Logger.Info(message);
        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        public static void Initialize()
        {
            // 使用默认配置初始化日志服务
            _logger = new LoggerService();
        }

        /// <summary>
        /// 清理日志资源
        /// </summary>
        public static void Shutdown()
        {
            if (_logger is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _logger = null;
        }
    }
}