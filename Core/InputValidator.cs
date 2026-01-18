using System;
using System.Text.RegularExpressions;

namespace BBMath.Core
{
    /// <summary>
    /// 输入验证类，提供各种输入数据的验证功能
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// 验证字符串是否为空或空白
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentException">字符串为空或空白时抛出</exception>
        public static void ValidateNotEmptyOrWhitespace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{parameterName} 不能为空或空白字符", parameterName);
            }
        }

        /// <summary>
        /// 验证字符串长度是否在指定范围内
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <exception cref="ArgumentException">字符串长度超出范围时抛出</exception>
        public static void ValidateStringLength(string value, string parameterName, int minLength, int maxLength)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} 长度不能少于 {minLength} 个字符，当前长度: {value.Length}", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new ArgumentException($"{parameterName} 长度不能超过 {maxLength} 个字符，当前长度: {value.Length}", parameterName);
            }
        }

        /// <summary>
        /// 验证数值范围
        /// </summary>
        /// <param name="value">要验证的数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <exception cref="ArgumentOutOfRangeException">数值超出范围时抛出</exception>
        public static void ValidateRange(int value, string parameterName, int minValue, int maxValue)
        {
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} 不能小于 {minValue}，当前值: {value}");
            }

            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} 不能大于 {maxValue}，当前值: {value}");
            }
        }

        /// <summary>
        /// 验证数值是否为正数
        /// </summary>
        /// <param name="value">要验证的数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentOutOfRangeException">数值不为正数时抛出</exception>
        public static void ValidatePositive(int value, string parameterName)
        {
            ValidateRange(value, parameterName, 1, int.MaxValue);
        }

        /// <summary>
        /// 验证数值是否为非负数
        /// </summary>
        /// <param name="value">要验证的数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentOutOfRangeException">数值为负数时抛出</exception>
        public static void ValidateNonNegative(int value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} 不能为负数，当前值: {value}");
            }
        }

        /// <summary>
        /// 验证数值是否在百分比范围内（0-100）
        /// </summary>
        /// <param name="value">要验证的百分比</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentOutOfRangeException">百分比不在 0-100 范围内时抛出</exception>
        public static void ValidatePercentage(int value, string parameterName)
        {
            ValidateRange(value, parameterName, 0, 100);
        }

        /// <summary>
        /// 验证字符串是否为有效的整数
        /// </summary>
        /// <param name="value">要验证的字符串</param>
        /// <param name="parameterName">参数名称</param>
        /// <param name="result">解析后的整数（验证通过时）</param>
        /// <returns>是否为有效的整数</returns>
        public static bool TryParseInt(string value, string parameterName, out int result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                LoggerHelper.Warning($"{parameterName} 为空字符串");
                return false;
            }

            if (!int.TryParse(value, out result))
            {
                LoggerHelper.Warning($"{parameterName} 不是有效的整数: {value}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证字符串是否为有效的密码
        /// </summary>
        /// <param name="password">密码字符串</param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>验证结果和错误消息</returns>
        public static Tuple<bool, string> ValidatePassword(string password, int minLength = 4, int maxLength = 20)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return Tuple.Create(false, "密码不能为空");
            }

            if (password.Length < minLength)
            {
                return Tuple.Create(false, $"密码长度不能少于 {minLength} 个字符");
            }

            if (password.Length > maxLength)
            {
                return Tuple.Create(false, $"密码长度不能超过 {maxLength} 个字符");
            }

            return Tuple.Create(true, string.Empty);
        }

        /// <summary>
        /// 验证文件路径是否有效
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentException">文件路径无效时抛出</exception>
        public static void ValidateFilePath(string filePath, string parameterName)
        {
            ValidateNotEmptyOrWhitespace(filePath, parameterName);

            // 检查是否包含无效字符
            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            if (filePath.IndexOfAny(invalidChars) >= 0)
            {
                throw new ArgumentException($"{parameterName} 包含无效字符: {filePath}", parameterName);
            }

            // 检查文件名是否有效
            string fileName = System.IO.Path.GetFileName(filePath);
            char[] invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidFileChars) >= 0)
            {
                throw new ArgumentException($"{parameterName} 的文件名包含无效字符: {fileName}", parameterName);
            }
        }

        /// <summary>
        /// 验证配置节名称
        /// </summary>
        /// <param name="sectionName">节名称</param>
        /// <exception cref="ArgumentException">节名称无效时抛出</exception>
        public static void ValidateConfigSection(string sectionName)
        {
            ValidateNotEmptyOrWhitespace(sectionName, nameof(sectionName));

            // 配置节名称通常不能包含某些特殊字符
            if (sectionName.Contains("=") || sectionName.Contains(";"))
            {
                throw new ArgumentException($"配置节名称不能包含 '=' 或 ';' 字符: {sectionName}", nameof(sectionName));
            }
        }

        /// <summary>
        /// 验证配置键名称
        /// </summary>
        /// <param name="keyName">键名称</param>
        /// <exception cref="ArgumentException">键名称无效时抛出</exception>
        public static void ValidateConfigKey(string keyName)
        {
            ValidateNotEmptyOrWhitespace(keyName, nameof(keyName));

            // 配置键名称不能包含某些特殊字符
            if (keyName.Contains("=") || keyName.Contains(";") || keyName.Contains("[") || keyName.Contains("]"))
            {
                throw new ArgumentException($"配置键名称不能包含 '=', ';', '[', ']' 字符: {keyName}", nameof(keyName));
            }
        }

        /// <summary>
        /// 验证题目数量配置
        /// </summary>
        /// <param name="totalQuestions">总题数</param>
        /// <param name="parameterName">参数名称</param>
        public static void ValidateQuestionCount(int totalQuestions, string parameterName)
        {
            if (totalQuestions < AppConstants.DefaultExamTotal)
            {
                LoggerHelper.Warning($"{parameterName} 小于默认值 {AppConstants.DefaultExamTotal}，可能影响使用体验");
            }

            if (totalQuestions < 1)
            {
                throw new ArgumentException($"{parameterName} 不能小于 1，当前值: {totalQuestions}", parameterName);
            }

            if (totalQuestions > 1000)
            {
                throw new ArgumentException($"{parameterName} 不能超过 1000，当前值: {totalQuestions}", parameterName);
            }
        }

        /// <summary>
        /// 验证金币数量
        /// </summary>
        /// <param name="coins">金币数量</param>
        /// <param name="parameterName">参数名称</param>
        public static void ValidateCoinAmount(int coins, string parameterName)
        {
            ValidateNonNegative(coins, parameterName);

            if (coins > 999999)
            {
                LoggerHelper.Warning($"{parameterName} 数量过大（超过 999999），可能导致显示异常");
            }
        }

        /// <summary>
        /// 验证时间限制（秒）
        /// </summary>
        /// <param name="seconds">时间限制（秒）</param>
        /// <param name="parameterName">参数名称</param>
        public static void ValidateTimeLimit(int seconds, string parameterName)
        {
            ValidatePositive(seconds, parameterName);

            if (seconds > 86400) // 24小时
            {
                throw new ArgumentException($"{parameterName} 不能超过 24 小时（86400 秒），当前值: {seconds}", parameterName);
            }
        }

        /// <summary>
        /// 验证题型索引
        /// </summary>
        /// <param name="typeIndex">题型索引</param>
        /// <param name="maxIndex">最大索引值</param>
        public static void ValidateExamTypeIndex(int typeIndex, int maxIndex)
        {
            ValidateNonNegative(typeIndex, nameof(typeIndex));

            if (typeIndex >= maxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(typeIndex), typeIndex,
                    $"题型索引 {typeIndex} 超出范围（最大: {maxIndex - 1}）");
            }
        }

        /// <summary>
        /// 验证数组是否为空
        /// </summary>
        /// <typeparam name="T">数组元素类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentNullException">数组为 null 时抛出</exception>
        /// <exception cref="ArgumentException">数组为空时抛出</exception>
        public static void ValidateArrayNotEmpty<T>(T[] array, string parameterName)
        {
            if (array == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (array.Length == 0)
            {
                throw new ArgumentException($"{parameterName} 不能为空数组", parameterName);
            }
        }

        /// <summary>
        /// 验证列表是否为空
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentNullException">列表为 null 时抛出</exception>
        /// <exception cref="ArgumentException">列表为空时抛出</exception>
        public static void ValidateListNotEmpty<T>(System.Collections.Generic.IList<T> list, string parameterName)
        {
            if (list == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (list.Count == 0)
            {
                throw new ArgumentException($"{parameterName} 不能为空列表", parameterName);
            }
        }

        /// <summary>
        /// 验证数字位数是否有效
        /// </summary>
        /// <param name="intBits">整数位数</param>
        /// <param name="decBits">小数位数</param>
        public static void ValidateNumberBits(int intBits, int decBits)
        {
            ValidateNonNegative(intBits, nameof(intBits));
            ValidateNonNegative(decBits, nameof(decBits));

            if (intBits + decBits > 10)
            {
                throw new ArgumentException($"整数位数和小数位数之和不能超过 10（当前: {intBits + decBits}）");
            }

            if (intBits > 6)
            {
                LoggerHelper.Warning($"整数位数 {intBits} 较大，可能增加计算难度");
            }
        }

        /// <summary>
        /// 安全执行操作并捕获异常
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="operationName">操作名称（用于日志）</param>
        /// <returns>是否成功执行</returns>
        public static bool SafeExecute(Action action, string operationName)
        {
            if (action == null)
            {
                LoggerHelper.Error($"{operationName}: 操作为 null");
                return false;
            }

            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"{operationName} 执行失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 安全执行操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">要执行的函数</param>
        /// <param name="operationName">操作名称（用于日志）</param>
        /// <param name="defaultValue">发生异常时的默认值</param>
        /// <returns>函数执行结果或默认值</returns>
        public static T SafeExecute<T>(Func<T> func, string operationName, T defaultValue = default(T))
        {
            if (func == null)
            {
                LoggerHelper.Error($"{operationName}: 操作为 null");
                return defaultValue;
            }

            try
            {
                return func();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"{operationName} 执行失败: {ex.Message}");
                LoggerHelper.Exception(ex);
                return defaultValue;
            }
        }
    }
}
