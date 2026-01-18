using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using WinFormsApplication = System.Windows.Forms.Application;

namespace BBMath.Core
{
    /// <summary>
    /// 全局异常处理器，捕获未处理的异常并记录日志
    /// </summary>
    public static class GlobalExceptionHandler
    {
        /// <summary>
        /// 是否已初始化
        /// </summary>
        private static bool _isInitialized = false;

        /// <summary>
        /// 初始化全局异常处理
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                LoggerHelper.Warning("全局异常处理器已经初始化，跳过重复初始化");
                return;
            }

            try
            {
                // 捕获 UI 线程的未处理异常
                WinFormsApplication.ThreadException += OnThreadException;

                // 捕获非 UI 线程的未处理异常
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                _isInitialized = true;
                LoggerHelper.Info("全局异常处理器已初始化");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"初始化全局异常处理器失败: {ex.Message}");
                LoggerHelper.Exception(ex);
            }
        }

        /// <summary>
        /// 处理 UI 线程的未处理异常
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">异常事件参数</param>
        private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "UI 线程异常", true);
        }

        /// <summary>
        /// 处理非 UI 线程的未处理异常
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">未处理异常事件参数</param>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, "应用程序未处理异常", !e.IsTerminating);

            // 如果是致命异常，终止应用程序
            if (e.IsTerminating)
            {
                LoggerHelper.Critical("应用程序即将终止，因为发生了未处理的致命异常");
            }
        }

        /// <summary>
        /// 统一处理异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="exceptionType">异常类型描述</param>
        /// <param name="canContinue">是否可以继续运行</param>
        private static void HandleException(Exception exception, string exceptionType, bool canContinue)
        {
            if (exception == null)
            {
                LoggerHelper.Warning($"{exceptionType}: 异常对象为 null");
                return;
            }

            try
            {
                // 记录详细的异常信息
                LoggerHelper.Error($"异常类型: {exceptionType}");
                LoggerHelper.Error($"异常消息: {exception.Message}");
                LoggerHelper.Error($"异常堆栈: {exception.StackTrace}");
                LoggerHelper.Exception(exception);

                // 记录内部异常（如果有）
                if (exception.InnerException != null)
                {
                    LoggerHelper.Error($"内部异常消息: {exception.InnerException.Message}");
                    LoggerHelper.Error($"内部异常堆栈: {exception.InnerException.StackTrace}");
                }

                // 显示用户友好的错误消息
                string userFriendlyMessage = GetUserFriendlyMessage(exception);
                ShowUserFriendlyError(exceptionType, userFriendlyMessage, canContinue);

                // 如果可以继续，尝试恢复或关闭相关窗体
                if (canContinue && WinFormsApplication.OpenForms.Count > 0)
                {
                    LoggerHelper.Info("尝试继续运行应用程序");
                }
            }
            catch (Exception ex)
            {
                // 如果异常处理过程中再发生异常，记录到事件日志
                try
                {
                    LoggerHelper.Critical($"处理异常时发生错误: {ex.Message}");
                }
                catch
                {
                    // 最后的手段：写入 Windows 事件日志
                    try
                    {
                        string eventLogName = "BBMath Application";
                        if (!EventLog.SourceExists(eventLogName))
                        {
                            EventLog.CreateEventSource(eventLogName, eventLogName);
                        }
                        EventLog.WriteEntry(eventLogName, $"处理异常时发生错误: {ex.Message}\n原始异常: {exception.Message}", EventLogEntryType.Error);
                    }
                    catch
                    {
                        // 事件日志也无法访问，无法继续
                    }
                }
            }
        }

        /// <summary>
        /// 获取用户友好的错误消息
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <returns>用户友好的错误消息</returns>
        private static string GetUserFriendlyMessage(Exception exception)
        {
            if (exception == null)
            {
                return "发生了未知错误";
            }

            // 根据异常类型返回不同的友好消息
            if (exception is UnauthorizedAccessException)
            {
                return "没有足够的权限访问文件或文件夹。请检查文件权限或以管理员身份运行程序。";
            }
            else if (exception is FileNotFoundException)
            {
                return "找不到必要的文件。请检查文件是否已被删除或移动。";
            }
            else if (exception is DirectoryNotFoundException)
            {
                return "找不到必要的文件夹。请检查文件夹是否存在。";
            }
            else if (exception is IOException)
            {
                return "文件读写操作失败。请检查磁盘空间或文件是否被其他程序占用。";
            }
            else if (exception is ArgumentException || exception is ArgumentNullException)
            {
                return "程序参数错误。请联系开发者并提供错误详情。";
            }
            else if (exception is FormatException)
            {
                return "数据格式错误。请检查输入的内容是否正确。";
            }
            else if (exception is OverflowException)
            {
                return "数值超出范围。请输入有效的数值。";
            }
            else if (exception is InvalidOperationException)
            {
                return "操作无法在当前状态下执行。请重试或重启程序。";
            }
            else if (exception is NullReferenceException)
            {
                return "程序内部错误。请联系开发者并提供错误详情。";
            }
            else if (exception is OutOfMemoryException)
            {
                return "内存不足。请关闭其他程序后重试。";
            }
            else
            {
                // 包含内部异常的消息
                if (exception.InnerException != null)
                {
                    return $"{exception.Message}\n详细原因: {exception.InnerException.Message}";
                }
                return exception.Message;
            }
        }

        /// <summary>
        /// 显示用户友好的错误对话框
        /// </summary>
        /// <param name="exceptionType">异常类型描述</param>
        /// <param name="message">用户友好的消息</param>
        /// <param name="canContinue">是否可以继续运行</param>
        private static void ShowUserFriendlyError(string exceptionType, string message, bool canContinue)
        {
            string title = canContinue ? "程序异常" : "严重错误";
            string buttonText = canContinue ? "确定" : "退出程序";
            MessageBoxIcon icon = canContinue ? MessageBoxIcon.Warning : MessageBoxIcon.Error;

            string fullMessage = $"发生错误：\n\n{message}\n\n" +
                              $"错误类型: {exceptionType}\n\n" +
                              $"日志已保存。如需帮助，请联系开发者并提供日志文件。";

            // 在 UI 线程中显示对话框
            if (WinFormsApplication.OpenForms.Count > 0)
            {
                // 使用主窗体显示错误
                var mainForm = WinFormsApplication.OpenForms[0];
                mainForm.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(mainForm, fullMessage, title, MessageBoxButtons.OK, icon);
                });
            }
            else
            {
                // 如果没有打开的窗体，显示在任务栏
                MessageBox.Show(fullMessage, title, MessageBoxButtons.OK, icon);
            }
        }

        /// <summary>
        /// 记录性能警告（用于性能监控）
        /// </summary>
        /// <param name="component">组件名称</param>
        /// <param name="operation">操作名称</param>
        /// <param name="durationMs">耗时（毫秒）</param>
        /// <param name="thresholdMs">警告阈值（毫秒）</param>
        public static void LogPerformanceWarning(string component, string operation, long durationMs, long thresholdMs)
        {
            if (durationMs > thresholdMs)
            {
                string message = $"{component}.{operation} 耗时 {durationMs}ms，超过阈值 {thresholdMs}ms";
                LoggerHelper.Warning(message);
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                if (_isInitialized)
                {
                    WinFormsApplication.ThreadException -= OnThreadException;
                    AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
                    _isInitialized = false;
                    LoggerHelper.Info("全局异常处理器已清理");
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"清理全局异常处理器时发生错误: {ex.Message}");
            }
        }
    }
}
