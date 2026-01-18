using System;
using System.IO;
using BBMath.Configuration;

namespace BBMath.Core
{
    /// <summary>
    /// 错误恢复服务，提供错误发生后的恢复策略
    /// </summary>
    public class ErrorRecoveryService
    {
        private readonly IFileService _fileService;
        private readonly IConfigurationService _configService;
        private readonly string _baseDirectory;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileService">文件服务</param>
        /// <param name="configService">配置服务</param>
        /// <param name="baseDirectory">基础目录（可选，默认为应用程序基目录）</param>
        public ErrorRecoveryService(IFileService fileService, IConfigurationService configService, string baseDirectory = null)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _baseDirectory = baseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ErrorRecoveryService()
            : this(new FileService(), new IniConfigurationService())
        {
        }

        /// <summary>
        /// 恢复配置文件（如果损坏）
        /// </summary>
        /// <returns>是否恢复成功</returns>
        public bool RecoverConfigurationFile()
        {
            try
            {
                string configPath = Path.Combine(_baseDirectory, AppConstants.ConfigFileName);
                string backupPath = configPath + ".backup";

                // 检查配置文件是否存在
                if (!File.Exists(configPath))
                {
                    LoggerHelper.Info($"配置文件不存在: {configPath}，将创建默认配置");
                    return CreateDefaultConfiguration();
                }

                // 尝试读取配置文件
                try
                {
                    var testConfig = new IniConfigurationService(configPath);
                    string testValue = testConfig.ReadValue("General", "PSW");
                    LoggerHelper.Info("配置文件验证通过");
                    return true;
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warning($"配置文件损坏: {ex.Message}");

                    // 尝试从备份恢复
                    if (File.Exists(backupPath))
                    {
                        try
                        {
                            File.Copy(backupPath, configPath, true);
                            LoggerHelper.Info($"已从备份恢复配置文件: {backupPath}");
                            return true;
                        }
                        catch (Exception backupEx)
                        {
                            LoggerHelper.Error($"从备份恢复配置文件失败: {backupEx.Message}");
                        }
                    }

                    // 备份恢复失败，创建默认配置
                    return CreateDefaultConfiguration();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"恢复配置文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        /// <returns>是否创建成功</returns>
        private bool CreateDefaultConfiguration()
        {
            try
            {
                string configPath = Path.Combine(_baseDirectory, AppConstants.ConfigFileName);

                // 使用 AppSettings 写入默认值
                var settings = new AppSettings(_configService);
                settings.Password = AppConstants.DefaultPassword;
                settings.CoinTotal = AppConstants.DefaultCoinTotal;
                settings.HelpBox = AppConstants.DefaultHelpBoxEnabled;
                settings.HelpBoxFree = AppConstants.DefaultHelpBoxFree;
                settings.PauseType = AppConstants.DefaultPauseType;
                settings.PauseSecondsLeft = AppConstants.DefaultPauseSecondsLeft;
                settings.AllowPauseCount = AppConstants.DefaultAllowPauseCount;
                settings.ErrorMessageShowTime = AppConstants.DefaultErrorMessageShowTime;
                settings.AwardCoinPerCorrect = AppConstants.DefaultAwardCoinPerCorrect;
                settings.CostCoinCheck = AppConstants.DefaultCostCoinCheck;
                settings.CostCoinGive = AppConstants.DefaultCostCoinGive;
                settings.PunishmentAddQuestions = AppConstants.DefaultPunishmentAddQuestions;
                settings.PunishmentTimeout = AppConstants.DefaultPunishmentTimeout;

                settings.Save();
                LoggerHelper.Info($"已创建默认配置文件: {configPath}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"创建默认配置文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 备份配置文件
        /// </summary>
        /// <returns>是否备份成功</returns>
        public bool BackupConfigurationFile()
        {
            try
            {
                string configPath = Path.Combine(_baseDirectory, AppConstants.ConfigFileName);
                string backupPath = configPath + ".backup";

                if (File.Exists(configPath))
                {
                    File.Copy(configPath, backupPath, true);
                    LoggerHelper.Info($"已备份配置文件: {backupPath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"备份配置文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 恢复日志文件（如果损坏）
        /// </summary>
        /// <returns>是否恢复成功</returns>
        public bool RecoverLogFile()
        {
            try
            {
                string logPath = Path.Combine(_baseDirectory, AppConstants.LogDataFileName);

                // 检查日志文件是否存在
                if (!File.Exists(logPath))
                {
                    LoggerHelper.Info($"日志文件不存在: {logPath}，将创建新文件");
                    return CreateDefaultLogFile();
                }

                // 尝试读取日志文件
                try
                {
                    using (var reader = new StreamReader(logPath))
                    {
                        // 尝试读取第一行
                        string line = reader.ReadLine();
                        LoggerHelper.Info("日志文件验证通过");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Warning($"日志文件损坏: {ex.Message}");

                    // 尝试从备份恢复
                    string backupPath = logPath + ".backup";
                    if (File.Exists(backupPath))
                    {
                        try
                        {
                            File.Copy(backupPath, logPath, true);
                            LoggerHelper.Info($"已从备份恢复日志文件: {backupPath}");
                            return true;
                        }
                        catch (Exception backupEx)
                        {
                            LoggerHelper.Error($"从备份恢复日志文件失败: {backupEx.Message}");
                        }
                    }

                    // 备份恢复失败，重命名损坏文件并创建新文件
                    return HandleCorruptedLogFile(logPath);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"恢复日志文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 处理损坏的日志文件
        /// </summary>
        /// <param name="logPath">日志文件路径</param>
        /// <returns>是否处理成功</returns>
        private bool HandleCorruptedLogFile(string logPath)
        {
            try
            {
                // 重命名损坏的日志文件
                string corruptedPath = logPath + ".corrupted." + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (File.Exists(logPath))
                {
                    File.Move(logPath, corruptedPath);
                    LoggerHelper.Warning($"已重命名损坏的日志文件: {corruptedPath}");
                }

                return CreateDefaultLogFile();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"处理损坏的日志文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 创建默认日志文件
        /// </summary>
        /// <returns>是否创建成功</returns>
        private bool CreateDefaultLogFile()
        {
            try
            {
                string logPath = Path.Combine(_baseDirectory, AppConstants.LogDataFileName);

                // 创建空日志文件
                File.WriteAllText(logPath, string.Empty);
                LoggerHelper.Info($"已创建新的日志文件: {logPath}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"创建日志文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 清理损坏的备份文件（超过 7 天的备份）
        /// </summary>
        /// <returns>清理的文件数量</returns>
        public int CleanupOldBackups()
        {
            try
            {
                string baseDirectory = _baseDirectory;
                var backupFiles = Directory.GetFiles(baseDirectory, "*.backup");
                var corruptedFiles = Directory.GetFiles(baseDirectory, "*.corrupted.*");

                int cleanedCount = 0;
                DateTime cutoffDate = DateTime.Now.AddDays(-7);

                foreach (string backupFile in backupFiles)
                {
                    try
                    {
                        var fileInfo = new FileInfo(backupFile);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            File.Delete(backupFile);
                            LoggerHelper.Info($"已删除旧备份文件: {backupFile}");
                            cleanedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Warning($"删除备份文件失败 {backupFile}: {ex.Message}");
                    }
                }

                foreach (string corruptedFile in corruptedFiles)
                {
                    try
                    {
                        var fileInfo = new FileInfo(corruptedFile);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            File.Delete(corruptedFile);
                            LoggerHelper.Info($"已删除旧损坏文件: {corruptedFile}");
                            cleanedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Warning($"删除损坏文件失败 {corruptedFile}: {ex.Message}");
                    }
                }

                if (cleanedCount > 0)
                {
                    LoggerHelper.Info($"清理了 {cleanedCount} 个旧备份和损坏文件");
                }

                return cleanedCount;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"清理旧备份文件失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return 0;
            }
        }

        /// <summary>
        /// 检查并修复游戏状态
        /// </summary>
        /// <returns>是否修复成功</returns>
        public bool CheckAndFixGameState()
        {
            try
            {
                bool needsFix = false;
                string issues = string.Empty;

                // 检查金币数量
                if (GameStateManager.coinTtl < AppConstants.MinCoinTotal)
                {
                    GameStateManager.coinTtl = AppConstants.DefaultCoinTotal;
                    issues += $"金币数量异常（{GameStateManager.coinTtl}），已重置为默认值；";
                    needsFix = true;
                }

                // 检查配置的暂停时间（pauseSecondsLeftConfig，而不是运行时值 pauseSecLeft）
                if (GameStateManager.pauseSecondsLeftConfig < AppConstants.MinPauseSecondsLeft)
                {
                    GameStateManager.pauseSecondsLeftConfig = AppConstants.DefaultPauseSecondsLeft;
                    GameStateManager.pauseSecLeft = GameStateManager.pauseSecondsLeftConfig; // 同步运行时值
                    issues += $"暂停时间异常（{GameStateManager.pauseSecLeft}），已重置为默认值；";
                    needsFix = true;
                }

                // 检查配置的可用暂停次数（allowPauseConfig，而不是运行时值 allowPause）
                if (GameStateManager.allowPauseConfig < AppConstants.MinAllowPauseCount)
                {
                    GameStateManager.allowPauseConfig = AppConstants.DefaultAllowPauseCount;
                    GameStateManager.allowPause = GameStateManager.allowPauseConfig; // 同步运行时值
                    issues += $"暂停次数异常（{GameStateManager.allowPause}），已重置为默认值；";
                    needsFix = true;
                }

                // 检查题目数量
                if (GameStateManager.examTtl < AppConstants.MinExamTotal)
                {
                    GameStateManager.examTtl = AppConstants.DefaultExamTotal;
                    issues += $"题目数量异常（{GameStateManager.examTtl}），已重置为默认值；";
                    needsFix = true;
                }

                if (needsFix)
                {
                    LoggerHelper.Warning($"发现游戏状态异常: {issues}");
                    // 保存修复后的状态
                    GameStateManager.SaveProgSettings();
                    return true;
                }

                LoggerHelper.Info("游戏状态检查正常");
                return false;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"检查和修复游戏状态失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        /// <returns>诊断信息字符串</returns>
        public string GetDiagnosticInfo()
        {
            try
            {
                var info = new System.Text.StringBuilder();

                info.AppendLine("=== BBMath 诊断信息 ===");
                info.AppendLine($"生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                info.AppendLine("\n=== 系统信息 ===");
                info.AppendLine($"操作系统: {Environment.OSVersion}");
                info.AppendLine($".NET 版本: {Environment.Version}");
                info.AppendLine($"机器名称: {Environment.MachineName}");
                info.AppendLine($"用户名称: {Environment.UserName}");

                info.AppendLine("\n=== 应用目录 ===");
                info.AppendLine($"基础目录: {AppDomain.CurrentDomain.BaseDirectory}");
                info.AppendLine($"可执行路径: {System.Reflection.Assembly.GetExecutingAssembly().Location}");

                info.AppendLine("\n=== 文件状态 ===");
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppConstants.ConfigFileName);
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppConstants.LogDataFileName);

                info.AppendLine($"配置文件: {(File.Exists(configPath) ? "存在" : "不存在")} - {configPath}");
                info.AppendLine($"日志文件: {(File.Exists(logPath) ? "存在" : "不存在")} - {logPath}");

                info.AppendLine("\n=== 游戏状态 ===");
                info.AppendLine($"金币数量: {GameStateManager.coinTtl}");
                info.AppendLine($"题目总数: {GameStateManager.examTtl}");
                info.AppendLine($"正确数量: {GameStateManager.correct}");
                info.AppendLine($"错误数量: {GameStateManager.wrong}");
                var pauseType = GameStateManager.GetPauseTypeEnum();
                info.AppendLine($"暂停类型: {(int)pauseType} ({(pauseType == PauseType.ByCount ? "限次数" : "限时间")})");
                info.AppendLine($"可用暂停: {(pauseType == PauseType.ByCount ? GameStateManager.allowPause.ToString() + " 次" : GameStateManager.pauseSecLeft.ToString() + " 秒")}");

                return info.ToString();
            }
            catch (Exception ex)
            {
                return $"获取诊断信息失败: {ex.Message}\n{ex.StackTrace}";
            }
        }

        /// <summary>
        /// 保存诊断信息到文件
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <returns>是否保存成功</returns>
        public bool SaveDiagnosticInfo(string filePath)
        {
            try
            {
                string diagnosticInfo = GetDiagnosticInfo();
                File.WriteAllText(filePath, diagnosticInfo);
                LoggerHelper.Info($"诊断信息已保存到: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"保存诊断信息失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }
    }
}
