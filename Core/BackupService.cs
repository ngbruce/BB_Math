using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BBMath.Core
{
    /// <summary>
    /// 文件备份服务
    /// </summary>
    public class BackupService
    {
        private readonly IFileService _fileService;
        private readonly string _backupDirectory;
        private const int MaxBackupDays = 30;
        private const int MaxBackupFilesPerDay = 10;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public BackupService() : this(new FileService())
        {
        }

        /// <summary>
        /// 带依赖注入的构造函数
        /// </summary>
        /// <param name="fileService">文件服务</param>
        public BackupService(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            
            var baseDir = _fileService.GetApplicationBaseDirectory();
            _backupDirectory = _fileService.CombinePaths(baseDir, "Backup");
            
            _fileService.EnsureDirectoryExists(_backupDirectory);
        }

        /// <summary>
        /// 备份配置文件
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>备份文件路径</returns>
        public string BackupConfigFile(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
                throw new ArgumentNullException(nameof(configFilePath));

            if (!_fileService.FileExists(configFilePath))
                throw new FileNotFoundException($"配置文件不存在: {configFilePath}");

            var fileName = Path.GetFileNameWithoutExtension(configFilePath);
            var extension = Path.GetExtension(configFilePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{fileName}_backup_{timestamp}{extension}";
            var backupPath = _fileService.CombinePaths(_backupDirectory, backupFileName);

            _fileService.CopyFile(configFilePath, backupPath, true);
            
            // 清理旧备份
            CleanupOldBackups();

            return backupPath;
        }

        /// <summary>
        /// 备份日志文件
        /// </summary>
        /// <param name="logFilePath">日志文件路径</param>
        /// <returns>备份文件路径</returns>
        public string BackupLogFile(string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
                throw new ArgumentNullException(nameof(logFilePath));

            if (!_fileService.FileExists(logFilePath))
                throw new FileNotFoundException($"日志文件不存在: {logFilePath}");

            var fileName = Path.GetFileNameWithoutExtension(logFilePath);
            var extension = Path.GetExtension(logFilePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{fileName}_backup_{timestamp}{extension}";
            var backupPath = _fileService.CombinePaths(_backupDirectory, backupFileName);

            _fileService.CopyFile(logFilePath, backupPath, true);
            
            // 清理旧备份
            CleanupOldBackups();

            return backupPath;
        }

        /// <summary>
        /// 备份历史记录文件
        /// </summary>
        /// <param name="historyFilePath">历史记录文件路径</param>
        /// <returns>备份文件路径</returns>
        public string BackupHistoryFile(string historyFilePath)
        {
            if (string.IsNullOrEmpty(historyFilePath))
                throw new ArgumentNullException(nameof(historyFilePath));

            if (!_fileService.FileExists(historyFilePath))
                throw new FileNotFoundException($"历史记录文件不存在: {historyFilePath}");

            var fileName = Path.GetFileNameWithoutExtension(historyFilePath);
            var extension = Path.GetExtension(historyFilePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{fileName}_backup_{timestamp}{extension}";
            var backupPath = _fileService.CombinePaths(_backupDirectory, backupFileName);

            _fileService.CopyFile(historyFilePath, backupPath, true);
            
            // 清理旧备份
            CleanupOldBackups();

            return backupPath;
        }

        /// <summary>
        /// 创建完整备份（配置、日志、历史）
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <param name="logFilePath">日志文件路径</param>
        /// <param name="historyFilePath">历史记录文件路径</param>
        /// <returns>备份文件路径列表</returns>
        public List<string> CreateFullBackup(string configFilePath = null, string logFilePath = null, string historyFilePath = null)
        {
            var backupFiles = new List<string>();

            try
            {
                if (!string.IsNullOrEmpty(configFilePath) && _fileService.FileExists(configFilePath))
                {
                    backupFiles.Add(BackupConfigFile(configFilePath));
                }

                if (!string.IsNullOrEmpty(logFilePath) && _fileService.FileExists(logFilePath))
                {
                    backupFiles.Add(BackupLogFile(logFilePath));
                }

                if (!string.IsNullOrEmpty(historyFilePath) && _fileService.FileExists(historyFilePath))
                {
                    backupFiles.Add(BackupHistoryFile(historyFilePath));
                }
            }
            catch (Exception ex)
            {
                // 如果备份失败，回滚已创建的备份
                foreach (var backupFile in backupFiles)
                {
                    try
                    {
                        _fileService.DeleteFile(backupFile);
                    }
                    catch
                    {
                        // 忽略删除失败的错误
                    }
                }

                throw new Exception("创建完整备份失败", ex);
            }

            return backupFiles;
        }

        /// <summary>
        /// 从备份恢复文件
        /// </summary>
        /// <param name="backupFilePath">备份文件路径</param>
        /// <param name="targetPath">目标文件路径（可选，默认恢复到原始位置）</param>
        /// <returns>恢复后的文件路径</returns>
        public string RestoreFromBackup(string backupFilePath, string targetPath = null)
        {
            if (string.IsNullOrEmpty(backupFilePath))
                throw new ArgumentNullException(nameof(backupFilePath));

            if (!_fileService.FileExists(backupFilePath))
                throw new FileNotFoundException($"备份文件不存在: {backupFilePath}");

            if (string.IsNullOrEmpty(targetPath))
            {
                // 移除文件名中的 _backup_ 和时间戳，恢复到原始文件名
                var fileName = Path.GetFileNameWithoutExtension(backupFilePath);
                var extension = Path.GetExtension(backupFilePath);
                var originalName = fileName;

                // 移除 _backup_ 时间戳部分
                var backupIndex = fileName.IndexOf("_backup_");
                if (backupIndex > 0)
                {
                    originalName = fileName.Substring(0, backupIndex);
                }

                targetPath = _fileService.CombinePaths(
                    _fileService.GetApplicationBaseDirectory(),
                    $"{originalName}{extension}"
                );
            }

            // 备份当前文件（如果存在）
            if (_fileService.FileExists(targetPath))
            {
                BackupConfigFile(targetPath);
            }

            _fileService.CopyFile(backupFilePath, targetPath, true);
            return targetPath;
        }

        /// <summary>
        /// 获取所有备份文件列表
        /// </summary>
        /// <param name="fileType">文件类型筛选（可选）</param>
        /// <returns>备份文件列表</returns>
        public List<BackupFileInfo> GetBackupFiles(string fileType = null)
        {
            var backupFiles = new List<BackupFileInfo>();
            
            if (!_fileService.FileExists(_backupDirectory))
                return backupFiles;

            var files = _fileService.GetFiles(_backupDirectory);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var backupInfo = new BackupFileInfo
                {
                    FilePath = file,
                    FileName = fileInfo.Name,
                    Size = fileInfo.Length,
                    CreationTime = fileInfo.CreationTime,
                    ModifiedTime = fileInfo.LastWriteTime
                };

                // 从文件名中提取原始文件名
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                var backupIndex = fileNameWithoutExt.IndexOf("_backup_");
                if (backupIndex > 0)
                {
                    backupInfo.OriginalFileName = fileNameWithoutExt.Substring(0, backupIndex) + fileInfo.Extension;
                }

                // 确定文件类型
                if (!string.IsNullOrEmpty(fileType))
                {
                    if (fileNameWithoutExt.IndexOf(fileType, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        backupFiles.Add(backupInfo);
                    }
                }
                else
                {
                    backupFiles.Add(backupInfo);
                }
            }

            return backupFiles.OrderByDescending(f => f.CreationTime).ToList();
        }

        /// <summary>
        /// 删除指定备份文件
        /// </summary>
        /// <param name="backupFilePath">备份文件路径</param>
        public void DeleteBackup(string backupFilePath)
        {
            if (string.IsNullOrEmpty(backupFilePath))
                throw new ArgumentNullException(nameof(backupFilePath));

            _fileService.DeleteFile(backupFilePath);
        }

        /// <summary>
        /// 清理指定天数之前的备份文件
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns>删除的文件数量</returns>
        public int CleanupOldBackups(int days = MaxBackupDays)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            var files = _fileService.GetFiles(_backupDirectory);
            int deletedCount = 0;

            foreach (var file in files)
            {
                try
                {
                    var lastWriteTime = _fileService.GetFileLastWriteTime(file);
                    if (lastWriteTime < cutoffDate)
                    {
                        _fileService.DeleteFile(file);
                        deletedCount++;
                    }
                }
                catch
                {
                    // 忽略删除失败的错误
                }
            }

            return deletedCount;
        }

        /// <summary>
        /// 清理旧的备份文件（自动清理）
        /// </summary>
        private void CleanupOldBackups()
        {
            CleanupOldBackups(MaxBackupDays);
        }
    }

    /// <summary>
    /// 备份文件信息
    /// </summary>
    public class BackupFileInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 格式化的大小
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (Size < 1024)
                    return $"{Size} B";
                else if (Size < 1024 * 1024)
                    return $"{Size / 1024.0:F2} KB";
                else
                    return $"{Size / (1024.0 * 1024):F2} MB";
            }
        }
    }
}
