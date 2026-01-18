using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BBMath.Core
{
    /// <summary>
    /// 历史记录管理服务
    /// </summary>
    public class HistoryRecordService
    {
        private readonly IFileService _fileService;
        private readonly string _historyDirectory;
        private readonly string _historyFileName;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public HistoryRecordService() : this(new FileService())
        {
        }

        /// <summary>
        /// 带依赖注入的构造函数
        /// </summary>
        /// <param name="fileService">文件服务</param>
        public HistoryRecordService(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            
            var baseDir = _fileService.GetApplicationBaseDirectory();
            _historyDirectory = _fileService.CombinePaths(baseDir, "History");
            _historyFileName = _fileService.CombinePaths(_historyDirectory, "practice_history.xml");
            
            _fileService.EnsureDirectoryExists(_historyDirectory);
        }

        /// <summary>
        /// 保存练习记录
        /// </summary>
        /// <param name="record">练习记录</param>
        public void SavePracticeRecord(PracticeRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            var records = LoadAllRecords();
            records.Add(record);

            SaveAllRecords(records);
        }

        /// <summary>
        /// 获取指定日期范围内的练习记录
        /// </summary>
        /// <param name="dateFrom">开始日期</param>
        /// <param name="dateTo">结束日期</param>
        /// <returns>练习记录列表</returns>
        public List<PracticeRecord> GetPracticeHistory(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var records = LoadAllRecords();

            if (dateFrom.HasValue)
            {
                records = records.Where(r => r.StartTime >= dateFrom.Value).ToList();
            }

            if (dateTo.HasValue)
            {
                records = records.Where(r => r.StartTime <= dateTo.Value.AddDays(1)).ToList();
            }

            return records.OrderByDescending(r => r.StartTime).ToList();
        }

        /// <summary>
        /// 获取所有练习记录
        /// </summary>
        /// <returns>练习记录列表</returns>
        public List<PracticeRecord> GetAllRecords()
        {
            return LoadAllRecords().OrderByDescending(r => r.StartTime).ToList();
        }

        /// <summary>
        /// 导出历史记录到CSV文件
        /// </summary>
        /// <param name="outputPath">输出文件路径（可选，默认为带时间戳的文件）</param>
        /// <returns>导出的文件路径</returns>
        public string ExportToCsv(string outputPath = null)
        {
            var records = LoadAllRecords();

            if (string.IsNullOrEmpty(outputPath))
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                outputPath = _fileService.CombinePaths(_historyDirectory, $"practice_export_{timestamp}.csv");
            }

            var sb = new StringBuilder();
            
            foreach (var record in records)
            {
                sb.AppendLine(record.ToCsvString());
                sb.AppendLine("========================================");
            }

            _fileService.WriteAllText(outputPath, sb.ToString());
            return outputPath;
        }

        /// <summary>
        /// 清空所有历史记录
        /// </summary>
        public void ClearAllRecords()
        {
            _fileService.DeleteFile(_historyFileName);
        }

        /// <summary>
        /// 删除指定日期之前的记录
        /// </summary>
        /// <param name="beforeDate">删除此日期之前的记录</param>
        /// <returns>删除的记录数</returns>
        public int DeleteRecordsBefore(DateTime beforeDate)
        {
            var records = LoadAllRecords();
            var originalCount = records.Count;
            records = records.Where(r => r.StartTime >= beforeDate).ToList();
            
            SaveAllRecords(records);
            
            return originalCount - records.Count;
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        /// <param name="dateFrom">开始日期（可选）</param>
        /// <param name="dateTo">结束日期（可选）</param>
        /// <returns>统计信息</returns>
        public StatisticsInfo GetStatistics(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var records = GetPracticeHistory(dateFrom, dateTo);
            var stats = new StatisticsInfo();

            stats.TotalPractices = records.Count;
            stats.TotalQuestions = records.Sum(r => r.TotalQuestions);
            stats.TotalCorrect = records.Sum(r => r.CorrectCount);
            stats.TotalWrong = records.Sum(r => r.WrongCount);
            stats.TotalCoinsEarned = records.Sum(r => r.CoinsEarned);

            if (stats.TotalQuestions > 0)
            {
                stats.OverallAccuracy = (double)stats.TotalCorrect / stats.TotalQuestions * 100;
            }

            if (records.Count > 0)
            {
                stats.AverageAccuracy = records.Average(r => r.AccuracyRate);
                stats.AverageTimePerQuestion = records.Sum(r => r.TotalSeconds) / stats.TotalQuestions;
            }

            return stats;
        }

        #region 私有方法

        private List<PracticeRecord> LoadAllRecords()
        {
            try
            {
                if (!_fileService.FileExists(_historyFileName))
                {
                    return new List<PracticeRecord>();
                }

                var xml = _fileService.ReadAllText(_historyFileName);
                var serializer = new XmlSerializer(typeof(List<PracticeRecord>));
                
                using (var reader = new StringReader(xml))
                {
                    var records = (List<PracticeRecord>)serializer.Deserialize(reader);
                    return records ?? new List<PracticeRecord>();
                }
            }
            catch (Exception)
            {
                // 如果文件损坏，返回空列表
                return new List<PracticeRecord>();
            }
        }

        private void SaveAllRecords(List<PracticeRecord> records)
        {
            var serializer = new XmlSerializer(typeof(List<PracticeRecord>));
            
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, records);
                _fileService.WriteAllText(_historyFileName, writer.ToString());
            }
        }

        #endregion
    }

    /// <summary>
    /// 统计信息
    /// </summary>
    public class StatisticsInfo
    {
        /// <summary>
        /// 总练习次数
        /// </summary>
        public int TotalPractices { get; set; }

        /// <summary>
        /// 总题目数
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// 总正确数
        /// </summary>
        public int TotalCorrect { get; set; }

        /// <summary>
        /// 总错误数
        /// </summary>
        public int TotalWrong { get; set; }

        /// <summary>
        /// 总正确率（%）
        /// </summary>
        public double OverallAccuracy { get; set; }

        /// <summary>
        /// 平均正确率（%）
        /// </summary>
        public double AverageAccuracy { get; set; }

        /// <summary>
        /// 平均每题用时（秒）
        /// </summary>
        public double AverageTimePerQuestion { get; set; }

        /// <summary>
        /// 总获得金币
        /// </summary>
        public int TotalCoinsEarned { get; set; }
    }
}
