using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BBMath.Configuration
{
    /// <summary>
    /// INI文件配置服务实现，带有内存缓存
    /// </summary>
    public class IniConfigurationService : IConfigurationService
    {
        public string Inipath { get; private set; }
        
        // 线程安全的配置缓存：键 = "Section|Key"，值 = 字符串值
        private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        private readonly object _fileLock = new object();

        // 声明API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iniPath">INI文件路径</param>
        public IniConfigurationService(string iniPath)
        {
            Inipath = iniPath ?? throw new ArgumentNullException(nameof(iniPath));
            LoadAllIntoCache();
        }

        /// <summary>
        /// 默认构造函数（使用应用程序目录下的默认配置文件）
        /// </summary>
        public IniConfigurationService()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Core.AppConstants.ConfigFileName))
        {
        }

        /// <summary>
        /// 从文件加载所有配置项到缓存
        /// </summary>
        private void LoadAllIntoCache()
        {
            // INI API 不支持枚举所有section/key，因此我们无法预先加载所有项
            // 缓存将在首次读取时填充
            _cache.Clear();
        }

        /// <summary>
        /// 生成缓存键
        /// </summary>
        private static string MakeCacheKey(string section, string key)
        {
            return $"{section}|{key}";
        }

        /// <summary>
        /// 读取字符串配置项（带缓存）
        /// </summary>
        public string ReadValue(string section, string key)
        {
            if (string.IsNullOrEmpty(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("必须指定键名称", nameof(key));

            string cacheKey = MakeCacheKey(section, key);
            
            // 如果缓存中存在，直接返回
            if (_cache.TryGetValue(cacheKey, out string cachedValue))
                return cachedValue;

            // 否则从文件读取
            StringBuilder temp = new StringBuilder(Core.AppConstants.DefaultIniStringBufferSize);
            int length = GetPrivateProfileString(section, key, "", temp, Core.AppConstants.DefaultIniStringBufferSize, Inipath);
            string value = length > 0 ? temp.ToString() : string.Empty;
            
            // 存入缓存（即使为空字符串也缓存，避免重复读取）
            _cache.TryAdd(cacheKey, value);
            return value;
        }

        /// <summary>
        /// 读取整数配置项
        /// </summary>
        public int ReadInt(string section, string key)
        {
            string value = ReadValue(section, key);
            if (int.TryParse(value, out int result))
                return result;
            
            // 记录警告日志（TODO: 集成日志系统）
            // Debug.Print($"配置项 [{section}]/{key} 的值 '{value}' 无法解析为整数，返回默认值 {Core.AppConstants.DefaultIniIntValue}");
            return Core.AppConstants.DefaultIniIntValue; // 默认值
        }

        /// <summary>
        /// 读取整数配置项（带默认值）
        /// </summary>
        public int ReadInt(string section, string key, int defaultValue)
        {
            string value = ReadValue(section, key);
            if (int.TryParse(value, out int result))
                return result;
            
            // 记录警告日志（TODO: 集成日志系统）
            // Debug.Print($"配置项 [{section}]/{key} 的值 '{value}' 无法解析为整数，返回默认值 {defaultValue}");
            return defaultValue; // 使用指定的默认值
        }

        /// <summary>
        /// 读取布尔配置项
        /// </summary>
        public bool ReadBool(string section, string key)
        {
            string value = ReadValue(section, key);
            if (bool.TryParse(value, out bool result))
                return result;
            
            // 尝试解析 "1"/"0" 或 "true"/"false" 不区分大小写
            if (value.Equals("1", StringComparison.OrdinalIgnoreCase))
                return true;
            if (value.Equals("0", StringComparison.OrdinalIgnoreCase))
                return false;
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;
            
            // 记录警告日志
            // Debug.Print($"配置项 [{section}]/{key} 的值 '{value}' 无法解析为布尔值，返回默认值 false");
            return false;
        }

        /// <summary>
        /// 写入配置项（更新缓存并写入文件）
        /// </summary>
        public void WriteValue(string section, string key, string value)
        {
            if (string.IsNullOrEmpty(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("必须指定键名称", nameof(key));
            if (value == null)
                throw new ArgumentException("值不能为null", nameof(value));

            string cacheKey = MakeCacheKey(section, key);
            string valueToCache = value;
            _cache.AddOrUpdate(cacheKey, valueToCache, (k, old) => valueToCache);
            
            lock (_fileLock)
            {
                WritePrivateProfileString(section, key, value, Inipath);
            }
        }

        /// <summary>
        /// 检查配置项是否存在
        /// </summary>
        public bool Exists(string section, string key)
        {
            string value = ReadValue(section, key);
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 清除缓存并重新从文件加载
        /// </summary>
        public void ReloadCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 验证文件是否存在
        /// </summary>
        public bool ExistINIFile()
        {
            return File.Exists(Inipath);
        }

        /// <summary>
        /// 写入INI文件（兼容旧方法）
        /// </summary>
        /// <param name="section">项目名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void IniWriteValue(string section, string key, string value)
        {
            WriteValue(section, key, value);
        }

        /// <summary>
        /// 读取INI文件值（兼容旧方法）
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键值名称</param>
        /// <param name="value">引用的字符串，储存读取值</param>
        /// <returns>读取的字符数量，如果为0可能不存在此配置值</returns>
        public int IniReadValue(string section, string key, ref string value)
        {
            if (string.IsNullOrEmpty(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("必须指定键名称", nameof(key));

            StringBuilder temp = new StringBuilder(Core.AppConstants.DefaultIniStringBufferSize);
            int length = GetPrivateProfileString(section, key, "", temp, Core.AppConstants.DefaultIniStringBufferSize, Inipath);
            value = length > 0 ? temp.ToString() : string.Empty;
            
            // 更新缓存
            string cacheKey = MakeCacheKey(section, key);
            string valueToCache = value;
            _cache.AddOrUpdate(cacheKey, valueToCache, (k, old) => valueToCache);
            
            return length;
        }
    }
}