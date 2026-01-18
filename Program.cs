//#define ADD_ONLY (1)
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using BBMath.Core;

namespace BBMath.Application
{
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                // 初始化日志系统
                LoggerHelper.Initialize();

                // 初始化全局异常处理器（必须在 Application.EnableVisualStyles 之前）
                GlobalExceptionHandler.Initialize();

                // 设置应用程序样式
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                // 运行主窗体
                System.Windows.Forms.Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                LoggerHelper.Critical($"应用程序启动失败: {ex.Message}");
                LoggerHelper.Exception(ex);

                MessageBox.Show(
                    $"应用程序启动失败：{ex.Message}\n\n请查看日志文件获取详细信息。",
                    "启动错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // 清理资源
                GlobalExceptionHandler.Cleanup();
                LoggerHelper.Shutdown();
            }
        }
    }
}