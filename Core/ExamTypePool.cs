using System;
using System.Collections.Generic;
using System.Linq;

namespace BBMath.Core
{
    /// <summary>
    /// 题型池，用于管理和分配题型
    /// </summary>
    public class ExamTypePool
    {
        /// <summary>
        /// 题型及其剩余数量的映射
        /// </summary>
        private Dictionary<ExamType, int> _typeCounts;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamTypePool()
        {
            _typeCounts = new Dictionary<ExamType, int>();
            _random = new Random();
        }

        /// <summary>
        /// 初始化题型池，分配题型数量
        /// </summary>
        /// <param name="totalQuestions">题目总数</param>
        /// <param name="supportedTypes">支持的题型列表</param>
        public void Initialize(int totalQuestions, List<ExamType> supportedTypes)
        {
            _typeCounts.Clear();

            if (supportedTypes == null || supportedTypes.Count == 0)
            {
                return;
            }

            int kinds = supportedTypes.Count;

            // 情况1：题目总数小于题型种类数
            // 随机选择题目总数种题型，各分配1题
            if (totalQuestions < kinds)
            {
                // 随机打乱题型列表
                var shuffledTypes = supportedTypes.OrderBy(x => _random.Next()).Take(totalQuestions).ToList();
                foreach (var type in shuffledTypes)
                {
                    _typeCounts[type] = 1;
                }
                return;
            }

            // 情况2：题目总数 >= 题型种类数
            // 基础分配：每种题型分配 totalQuestions / kinds 题
            int baseCount = totalQuestions / kinds;
            int remainder = totalQuestions % kinds;

            // 为每种题型分配基础数量
            foreach (var type in supportedTypes)
            {
                _typeCounts[type] = baseCount;
            }

            // 余数随机分配给部分题型
            if (remainder > 0)
            {
                var shuffledTypes = supportedTypes.OrderBy(x => _random.Next()).Take(remainder).ToList();
                foreach (var type in shuffledTypes)
                {
                    _typeCounts[type]++;
                }
            }
        }

        /// <summary>
        /// 从题型池中随机抽取一个题型
        /// </summary>
        /// <returns>抽取的题型，如果题型池为空则返回 null</returns>
        public ExamType? DrawType()
        {
            // 过滤出还有剩余题型的题型
            var availableTypes = _typeCounts.Where(kvp => kvp.Value > 0).ToList();

            if (availableTypes.Count == 0)
            {
                return null;
            }

            // 随机选择一个题型
            int randomIndex = _random.Next(availableTypes.Count);
            var selected = availableTypes[randomIndex];

            // 减少该题型的剩余数量
            _typeCounts[selected.Key]--;

            // 如果数量为0，从字典中移除（可选，但保留0值也可以）
            if (_typeCounts[selected.Key] == 0)
            {
                _typeCounts.Remove(selected.Key);
            }

            return selected.Key;
        }

        /// <summary>
        /// 获取题型池中剩余的题型数量总和
        /// </summary>
        public int RemainingCount
        {
            get { return _typeCounts.Values.Sum(); }
        }

        /// <summary>
        /// 获取所有题型及其剩余数量（用于调试）
        /// </summary>
        public Dictionary<ExamType, int> GetTypeCounts()
        {
            return new Dictionary<ExamType, int>(_typeCounts);
        }

        /// <summary>
        /// 清空题型池
        /// </summary>
        public void Clear()
        {
            _typeCounts.Clear();
        }

        /// <summary>
        /// 向指定题型追加题目数量（用于答错惩罚直接追加到错题型）
        /// </summary>
        /// <param name="type">题型</param>
        /// <param name="count">追加数量</param>
        public void AddToType(ExamType type, int count)
        {
            if (_typeCounts.ContainsKey(type))
            {
                _typeCounts[type] += count;
            }
            else
            {
                _typeCounts[type] = count;
            }
        }
    }
}
