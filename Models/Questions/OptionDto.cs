namespace RedisUsage.Models.Questions
{
    public class OptionDto
    {
        /// <summary>
        /// 选项的Key,如：A,B,C,D
        /// </summary>
        public string OptionKey { get; set; }
        /// <summary>
        /// 选项Key对应的内容
        /// </summary>
        public string OptionValue { get; set; }
        /// <summary>
        /// 是否是正确选项(true/false)
        /// </summary>
        public bool IsRight { get; set; }
    }
}