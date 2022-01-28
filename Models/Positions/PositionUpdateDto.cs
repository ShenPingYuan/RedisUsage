using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Models.Positions
{
    public class PositionUpdateDto
    {
        /// <summary>
        /// 职位名字
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
