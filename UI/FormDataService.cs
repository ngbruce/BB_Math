using System;
using System.Collections.Generic;

namespace BBMath.UI
{
    /// <summary>
    /// 表单数据传递服务
    /// </summary>
    public class FormDataService
    {
        private static readonly Dictionary<string, object> _dataStore = new Dictionary<string, object>();
        private static readonly object _lock = new object();

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Store(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            lock (_lock)
            {
                _dataStore[key] = value;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>数据</returns>
        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            lock (_lock)
            {
                if (_dataStore.TryGetValue(key, out var value))
                {
                    if (value is T typedValue)
                    {
                        return typedValue;
                    }

                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch
                    {
                        return default(T);
                    }
                }

                return default(T);
            }
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">键</param>
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            lock (_lock)
            {
                _dataStore.Remove(key);
            }
        }

        /// <summary>
        /// 检查数据是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            lock (_lock)
            {
                return _dataStore.ContainsKey(key);
            }
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _dataStore.Clear();
            }
        }

        /// <summary>
        /// 获取所有键
        /// </summary>
        /// <returns>键集合</returns>
        public static string[] GetAllKeys()
        {
            lock (_lock)
            {
                var keys = new string[_dataStore.Count];
                _dataStore.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        /// <summary>
        /// 预定义的键常量
        /// </summary>
        public static class Keys
        {
            /// <summary>
            /// 游戏状态
            /// </summary>
            public const string GameState = "GameState";

            /// <summary>
            /// 用户设置
            /// </summary>
            public const string UserSettings = "UserSettings";

            /// <summary>
            /// 当前题型索引
            /// </summary>
            public const string CurrentExamTypeIndex = "CurrentExamTypeIndex";

            /// <summary>
            /// 暂停状态
            /// </summary>
            public const string PauseState = "PauseState";

            /// <summary>
            /// 金币数量
            /// </summary>
            public const string CoinCount = "CoinCount";

            /// <summary>
            /// 练习记录
            /// </summary>
            public const string PracticeRecord = "PracticeRecord";

            /// <summary>
            /// 日志消息
            /// </summary>
            public const string LogMessage = "LogMessage";
        }
    }
}
