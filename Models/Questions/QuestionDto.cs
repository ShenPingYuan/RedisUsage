using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Models.Questions
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 知识分类
        /// </summary>
        public string KnowledgeCategoryName { get; set; }
        /// <summary>
        /// 条线分类Id
        /// </summary>
        public string StripLineCategoryId { get; set; }
        /// <summary>
        /// 岗位分类
        /// </summary>
        public List<string> PositionCategoryIds { get; set; }
        /// <summary>
        /// 题型1-单选、2-多选、3-判断
        /// </summary>
        public int QuestionType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 解析
        /// </summary>
        public string QuestionAnalysis { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        public List<OptionDto> Options { get; set; }
    }
}
