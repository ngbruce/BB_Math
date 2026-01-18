using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BBMath.Core;

namespace BBMath.UI
{
    /// <summary>
    /// 基础表单类，提供通用UI功能
    /// </summary>
    public class BaseForm : Form
    {
        /// <summary>
        /// 应用程序默认字体（避免与基类 Form.DefaultFont 冲突）
        /// </summary>
        protected Font AppDefaultFont => new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);

        /// <summary>
        /// 标题字体
        /// </summary>
        protected Font TitleFont => new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);

        /// <summary>
        /// 默认按钮颜色
        /// </summary>
        protected Color DefaultButtonColor => Color.FromArgb(51, 122, 183);

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseForm()
        {
            SetupFormStyle();
        }

        /// <summary>
        /// 设置表单样式
        /// </summary>
        protected virtual void SetupFormStyle()
        {
            this.Font = AppDefaultFont;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        /// <summary>
        /// 显示信息提示框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        protected void ShowInfo(string message, string title = "提示")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示警告提示框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        protected DialogResult ShowWarning(string message, string title = "警告")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 显示错误提示框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="logError">是否记录到日志</param>
        protected void ShowError(string message, string title = "错误", bool logError = true)
        {
            if (logError)
            {
                LoggerHelper.Error($"UI 错误提示: {message}");
            }
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <returns>用户选择</returns>
        protected DialogResult ShowConfirm(string message, string title = "确认")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// 显示用户友好的异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="operation">操作描述</param>
        protected void ShowUserFriendlyException(Exception ex, string operation)
        {
            string message = GetUserFriendlyMessage(ex, operation);
            ShowError(message);
        }

        /// <summary>
        /// 获取用户友好的异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="operation">操作描述</param>
        /// <returns>用户友好的消息</returns>
        private string GetUserFriendlyMessage(Exception ex, string operation)
        {
            if (ex == null)
            {
                return $"{operation} 失败：未知错误";
            }

            string baseMessage = $"{operation} 失败";
            string detailMessage = string.Empty;

            if (ex is UnauthorizedAccessException)
            {
                detailMessage = "权限不足，请检查文件权限或以管理员身份运行";
            }
            else if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                detailMessage = "找不到文件或文件夹，请检查文件是否存在";
            }
            else if (ex is IOException)
            {
                detailMessage = "文件操作失败，可能是磁盘已满或文件被占用";
            }
            else if (ex is FormatException)
            {
                detailMessage = "数据格式不正确，请检查输入";
            }
            else if (ex is OverflowException)
            {
                detailMessage = "数值超出允许范围";
            }
            else if (ex is InvalidOperationException)
            {
                detailMessage = "当前状态下无法执行此操作";
            }
            else if (ex is ArgumentException || ex is ArgumentNullException)
            {
                detailMessage = "参数错误，请联系技术支持";
            }
            else
            {
                detailMessage = ex.Message;
            }

            return $"{baseMessage}\n{detailMessage}";
        }

        /// <summary>
        /// 安全执行操作，捕获异常并显示错误
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="operationName">操作名称（用于日志）</param>
        /// <returns>是否成功</returns>
        protected bool SafeExecute(Action action, string operationName = "操作")
        {
            try
            {
                action?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"{operationName} 失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                ShowUserFriendlyException(ex, operationName);
                return false;
            }
        }

        /// <summary>
        /// 安全执行操作，捕获异常并显示错误
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="func">要执行的函数</param>
        /// <param name="defaultValue">失败时的默认值</param>
        /// <param name="operationName">操作名称（用于日志）</param>
        /// <returns>函数返回值或默认值</returns>
        protected T SafeExecute<T>(Func<T> func, T defaultValue = default(T), string operationName = "操作")
        {
            try
            {
                return func != null ? func() : defaultValue;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"{operationName} 失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                ShowUserFriendlyException(ex, operationName);
                return defaultValue;
            }
        }
    }
}
