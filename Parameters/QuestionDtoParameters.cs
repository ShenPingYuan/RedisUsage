using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Parameters
{
    public class QuestionDtoParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        /// <summary>
        /// 页数
        /// </summary>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// 分页大小最多50，默认是10
        /// </summary>
        public int PageSize 
        {
            get => _pageSize;
            set
            {
                if (value <= 0) return;
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }
        /// <summary>
        /// 知识分类
        /// </summary>
        public string KnowledgeCategory { get; set; }
        /// <summary>
        /// 条线分类Id
        /// </summary>
        public string StripLineCategoryId { get; set; }
        /// <summary>
        /// 岗位分类集合
        /// </summary>
        public List<string> PositionCategoryIds { get; set; }
        /// <summary>
        /// 题型集合，1-单选、2-多选、3-判断
        /// </summary>
        public List<int> QuestionTypes { get; set; }
        /// <summary>
        /// 搜索参数
        /// </summary>
        public string Q { get; set; }
        /// <summary>
        /// 是否是随机（默认按创建时间排序）
        /// </summary>
        public bool IsRandom { get; set; }
    }
}
