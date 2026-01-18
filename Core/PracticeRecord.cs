using System;
using System.Collections.Generic;
using System.Text;

namespace BBMath.Core
{
    /// <summary>
    /// 练习记录
    /// </summary>
    public class PracticeRecord
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 总用时（秒）
        /// </summary>
        public int TotalSeconds { get; set; }

        /// <summary>
        /// 题目类型
        /// </summary>
        public ExamType ExamType { get; set; }

        /// <summary>
        /// 总题目数
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// 正确题目数
        /// </summary>
        public int CorrectCount { get; set; }

        /// <summary>
        /// 错误题目数
        /// </summary>
        public int WrongCount { get; set; }

        /// <summary>
        /// 正确率
        /// </summary>
        public double AccuracyRate { get; set; }

        /// <summary>
        /// 题目详情列表
        /// </summary>
        public List<QuestionRecord> Questions { get; set; }

        /// <summary>
        /// 获得的金币
        /// </summary>
        public int CoinsEarned { get; set; }

        /// <summary>
        /// 题目难度
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PracticeRecord()
        {
            Id = Guid.NewGuid().ToString();
            Questions = new List<QuestionRecord>();
        }

        /// <summary>
        /// 计算正确率
        /// </summary>
        public void CalculateAccuracy()
        {
            if (TotalQuestions > 0)
            {
                AccuracyRate = (double)CorrectCount / TotalQuestions * 100;
            }
            else
            {
                AccuracyRate = 0;
            }
        }

        /// <summary>
        /// 转换为CSV格式
        /// </summary>
        /// <returns>CSV字符串</returns>
        public string ToCsvString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"练习ID,开始时间,结束时间,总用时(秒),题型,难度,总题数,正确数,错误数,正确率(%),获得金币");
            sb.AppendLine($"{Id},{StartTime:yyyy-MM-dd HH:mm:ss},{EndTime:yyyy-MM-dd HH:mm:ss},{TotalSeconds},{ExamType},{Difficulty},{TotalQuestions},{CorrectCount},{WrongCount},{AccuracyRate:F2},{CoinsEarned}");
            sb.AppendLine();
            sb.AppendLine("题目序号,题目,用户答案,正确答案,结果,用时(秒)");

            for (int i = 0; i < Questions.Count; i++)
            {
                var q = Questions[i];
                sb.AppendLine($"{i + 1},{q.Equation},{q.UserAnswer},{q.CorrectAnswer},{q.Result},{q.TimeSpent}");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// 单个题目记录
    /// </summary>
    public class QuestionRecord
    {
        /// <summary>
        /// 题目算式
        /// </summary>
        public string Equation { get; set; }

        /// <summary>
        /// 用户答案
        /// </summary>
        public string UserAnswer { get; set; }

        /// <summary>
        /// 正确答案
        /// </summary>
        public string CorrectAnswer { get; set; }

        /// <summary>
        /// 结果（正确/错误）
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 用时（秒）
        /// </summary>
        public int TimeSpent { get; set; }
    }
}
