using System;

namespace BBMath.Configuration
{
    /// <summary>
    /// 配置管理服务接口，提供统一的配置读写能力
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// 读取字符串配置项
        /// </summary>
        /// <param name="section">配置节点名称</param>
        /// <param name="key">配置键名称</param>
        /// <returns>配置值字符串；如果不存在则返回空字符串</returns>
        string ReadValue(string section, string key);

    /// <summary>
    /// 读取整数配置项
    /// </summary>
    /// <param name="section">配置节点名称</param>
    /// <param name="key">配置键名称</param>
    /// <returns>配置值整数；如果解析失败则返回默认值</returns>
    int ReadInt(string section, string key);

    /// <summary>
    /// 读取整数配置项（带默认值）
    /// </summary>
    /// <param name="section">配置节点名称</param>
    /// <param name="key">配置键名称</param>
    /// <param name="defaultValue">默认值，当配置不存在或解析失败时返回</param>
    /// <returns>配置值整数；如果解析失败则返回指定的默认值</returns>
    int ReadInt(string section, string key, int defaultValue);

        /// <summary>
        /// 读取布尔配置项
        /// </summary>
        /// <param name="section">配置节点名称</param>
        /// <param name="key">配置键名称</param>
        /// <returns>配置值布尔；如果解析失败则返回默认值</returns>
        bool ReadBool(string section, string key);

        /// <summary>
        /// 写入配置项
        /// </summary>
        /// <param name="section">配置节点名称</param>
        /// <param name="key">配置键名称</param>
        /// <param name="value">配置值</param>
        void WriteValue(string section, string key, string value);

        /// <summary>
        /// 检查配置项是否存在
        /// </summary>
        /// <param name="section">配置节点名称</param>
        /// <param name="key">配置键名称</param>
        /// <returns>如果配置项存在则返回true</returns>
        bool Exists(string section, string key);
    }
}