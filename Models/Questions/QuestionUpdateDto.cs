using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RedisUsage.Models.Questions
{
    public class QuestionUpdateDto
    {
        [Required]
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
        [Required]
        public List<string> PositionCategoryIds { get; set; }
        /// <summary>
        /// 题型1-单选、2-多选、3-判断
        /// </summary>
        [Required]
        [Range(1, 3, ErrorMessage = "题型只能是1-3：1-单选、2-多选、3-判断")]
        public int QuestionType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }
        /// <summary>
        /// 解析
        /// </summary>
        public string QuestionAnalysis { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        [Required]
        public List<OptionAddOrUpdateDto> Options { get; set; }
    }
}
