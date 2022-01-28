using System.ComponentModel.DataAnnotations;

namespace RedisUsage.Models.Questions
{
    public class OptionAddOrUpdateDto
    {
        /// <summary>
        /// 选项的Key，如：A，B，C
        /// </summary>
        [Required]
        [MaxLength(1,ErrorMessage ="选项的长度最长是一个字符，如：A,B,C,D,E,F")]
        public string OptionKey { get; set; }
        /// <summary>
        /// 选项key对应的内容
        /// </summary>
        [Required]
        public string OptionValue { get; set; }
        /// <summary>
        /// 是否是正确选项(true/false)
        /// </summary>
        [Required]
        [Range(typeof(bool),"false", "true", ErrorMessage ="是否是正确答案，只能填'true' or 'false'")]
        public bool IsRight { get; set; }
    }
}