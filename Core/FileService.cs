using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BBMath.Core
{
    /// <summary>
    /// 文件服务实现
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FileService()
        {
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>如果文件存在返回true</returns>
        public bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                return File.Exists(filePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取文件所有文本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容</returns>
        public string ReadAllText(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!FileExists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 读取文件所有行
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件行的集合</returns>
        public IEnumerable<string> ReadAllLines(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!FileExists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            return File.ReadAllLines(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 写入文本到文件（覆盖）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要写入的内容</param>
        public void WriteAllText(string filePath, string content)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, content ?? string.Empty, Encoding.UTF8);
        }

        /// <summary>
        /// 追加文本到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要追加的内容</param>
        public void AppendAllText(string filePath, string content)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            File.AppendAllText(filePath, content ?? string.Empty, Encoding.UTF8);
        }

        /// <summary>
        /// 追加多行文本到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="lines">要追加的行</param>
        public void AppendAllLines(string filePath, IEnumerable<string> lines)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            File.AppendAllLines(filePath, lines, Encoding.UTF8);
        }

        /// <summary>
        /// 创建目录（如果不存在）
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        public void EnsureDirectoryExists(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return;

            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch (Exception)
            {
                // 静默失败，避免抛出异常
            }
        }

        /// <summary>
        /// 获取应用程序基目录
        /// </summary>
        /// <returns>应用程序基目录路径</returns>
        public string GetApplicationBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 组合路径
        /// </summary>
        /// <param name="paths">路径部分</param>
        /// <returns>组合后的路径</returns>
        public string CombinePaths(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            return Path.Combine(paths);
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destinationPath">目标文件路径</param>
        /// <param name="overwrite">是否覆盖</param>
        public void CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentNullException(nameof(sourcePath));

            if (string.IsNullOrEmpty(destinationPath))
                throw new ArgumentNullException(nameof(destinationPath));

            if (!FileExists(sourcePath))
                throw new FileNotFoundException($"源文件不存在: {sourcePath}");

            EnsureDirectoryExists(Path.GetDirectoryName(destinationPath));
            File.Copy(sourcePath, destinationPath, overwrite);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destinationPath">目标文件路径</param>
        public void MoveFile(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentNullException(nameof(sourcePath));

            if (string.IsNullOrEmpty(destinationPath))
                throw new ArgumentNullException(nameof(destinationPath));

            if (!FileExists(sourcePath))
                throw new FileNotFoundException($"源文件不存在: {sourcePath}");

            EnsureDirectoryExists(Path.GetDirectoryName(destinationPath));
            File.Move(sourcePath, destinationPath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (FileExists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 获取文件大小（字节）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件大小</returns>
        public long GetFileSize(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!FileExists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// 获取文件最后修改时间
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>最后修改时间</returns>
        public DateTime GetFileLastWriteTime(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!FileExists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            var fileInfo = new FileInfo(filePath);
            return fileInfo.LastWriteTime;
        }

        /// <summary>
        /// 获取目录中所有文件
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="searchPattern">搜索模式（如 "*.log"）</param>
        /// <returns>文件路径集合</returns>
        public IEnumerable<string> GetFiles(string directoryPath, string searchPattern = "*.*")
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentNullException(nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"目录不存在: {directoryPath}");

            return Directory.GetFiles(directoryPath, searchPattern);
        }
    }
}
