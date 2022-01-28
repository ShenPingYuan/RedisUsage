using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Models.Positions
{
    public class Position
    {
        /// <summary>
        /// 职位Id
        /// </summary>
        public Guid Id{ get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public Guid ParentId { get; set; }
        /// <summary>
        /// 职位名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所有子岗位Id
        /// </summary>
        public List<Guid> Children { get; set; } = new List<Guid>();
    }
}
