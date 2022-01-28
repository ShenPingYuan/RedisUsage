using System;
using System.Collections.Generic;

namespace RedisUsage.Models.Questions
{
    public class Question
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 知识分类名
        /// </summary>
        public string KnowledgeCategoryName { get; set; }
        /// <summary>
        /// 条线分类Id
        /// </summary>
        public string StripLineCategoryId { get; set; }
        /// <summary>
        /// 岗位分类Ids
        /// </summary>
        public List<string> PositionCategoryIds { get; set; } = new List<string>();
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
        /// 创建人名字
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 题目的选项
        /// </summary>
        public List<Option> Options { get; set; }
    }
}
