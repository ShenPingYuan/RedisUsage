using System;

namespace RedisUsage.Models.Questions
{
    public class Option
    {
        /// <summary>
        /// 题目Id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 选项Key，如：A,B,C,D
        /// </summary>
        public string OptionKey { get; set; }
        /// <summary>
        /// 选项内容
        /// </summary>
        public string OptionValue { get; set; }
        /// <summary>
        /// 是否是正确答案
        /// </summary>
        public bool IsRight { get; set; }
    }
}
