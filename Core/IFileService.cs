using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BBMath.Core
{
    /// <summary>
    /// 文件服务接口
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>如果文件存在返回true</returns>
        bool FileExists(string filePath);

        /// <summary>
        /// 读取文件所有文本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容</returns>
        string ReadAllText(string filePath);

        /// <summary>
        /// 读取文件所有行
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件行的集合</returns>
        IEnumerable<string> ReadAllLines(string filePath);

        /// <summary>
        /// 写入文本到文件（覆盖）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要写入的内容</param>
        void WriteAllText(string filePath, string content);

        /// <summary>
        /// 追加文本到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">要追加的内容</param>
        void AppendAllText(string filePath, string content);

        /// <summary>
        /// 追加多行文本到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="lines">要追加的行</param>
        void AppendAllLines(string filePath, IEnumerable<string> lines);

        /// <summary>
        /// 创建目录（如果不存在）
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        void EnsureDirectoryExists(string directoryPath);

        /// <summary>
        /// 获取应用程序基目录
        /// </summary>
        /// <returns>应用程序基目录路径</returns>
        string GetApplicationBaseDirectory();

        /// <summary>
        /// 组合路径
        /// </summary>
        /// <param name="paths">路径部分</param>
        /// <returns>组合后的路径</returns>
        string CombinePaths(params string[] paths);

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destinationPath">目标文件路径</param>
        /// <param name="overwrite">是否覆盖</param>
        void CopyFile(string sourcePath, string destinationPath, bool overwrite = false);

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destinationPath">目标文件路径</param>
        void MoveFile(string sourcePath, string destinationPath);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// 获取文件大小（字节）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件大小</returns>
        long GetFileSize(string filePath);

        /// <summary>
        /// 获取文件最后修改时间
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>最后修改时间</returns>
        DateTime GetFileLastWriteTime(string filePath);

        /// <summary>
        /// 获取目录中所有文件
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="searchPattern">搜索模式（如 "*.log"）</param>
        /// <returns>文件路径集合</returns>
        IEnumerable<string> GetFiles(string directoryPath, string searchPattern = "*.*");
    }
}