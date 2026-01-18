using System;

namespace BBMath.Core
{
    /// <summary>
    /// 题目对象类
    /// </summary>
    public class ExamObject
    {
        public int Count { get; set; }
        public int Time { get; set; }
        public bool Punish { get; set; }
        public string Name { get; set; }
        public ExamType Examtype { get; set; }
        public int Level { get; set; }
        public int Wrongcount { get; set; }
        public bool IsNew { get; set; }
        
        // 新增属性
        public int TimeLimit { get; set; }
        public int ElapsedTime { get; set; }
        public int TotalQty { get; set; }
        public int CorrectQty { get; set; }
        public int WrongQty { get; set; }
        public int Percent { get; set; }
        public string Description { get; set; }
        public int IntBits { get; set; }
        public int DecBits { get; set; }
        public bool AllowNegativeResult { get; set; }
        public int SumQty { get; set; }
        
        /// <summary>
        /// 题型总数
        /// </summary>
        public static int TotalTypeQty { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamObject(int count, int time, bool punish, string name, ExamType examtype, int level, int wrongcount, bool isNew)
        {
            Count = count;
            Time = time;
            Punish = punish;
            Name = name;
            Examtype = examtype;
            Level = level;
            Wrongcount = wrongcount;
            IsNew = isNew;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ExamObject()
        {
        }
    }
}