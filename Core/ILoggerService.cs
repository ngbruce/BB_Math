using System;

namespace BBMath.Core
{
    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 调试信息，用于开发阶段
        /// </summary>
        Debug,
        
        /// <summary>
        /// 一般信息，用于记录正常操作
        /// </summary>
        Info,
        
        /// <summary>
        /// 警告信息，表示潜在问题
        /// </summary>
        Warning,
        
        /// <summary>
        /// 错误信息，表示操作失败但应用可继续运行
        /// </summary>
        Error,
        
        /// <summary>
        /// 严重错误，表示应用可能无法继续运行
        /// </summary>
        Critical
    }

    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// 记录调试级别日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Debug(string message);

        /// <summary>
        /// 记录信息级别日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Info(string message);

        /// <summary>
        /// 记录警告级别日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Warning(string message);

        /// <summary>
        /// 记录错误级别日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Error(string message);

        /// <summary>
        /// 记录严重级别日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Critical(string message);

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="message">可选的自定义消息</param>
        void Exception(Exception ex, string message = null);

        /// <summary>
        /// 记录带有格式字符串的日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="format">格式字符串</param>
        /// <param name="args">参数</param>
        void Log(LogLevel level, string format, params object[] args);

        /// <summary>
        /// 检查指定日志级别是否启用
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns>如果启用返回true</returns>
        bool IsEnabled(LogLevel level);
    }
}