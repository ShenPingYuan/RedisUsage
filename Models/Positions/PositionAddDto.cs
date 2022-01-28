using System.ComponentModel.DataAnnotations;

namespace RedisUsage.Models.Positions
{
    public class PositionAddDto
    {
        /// <summary>
        /// 职位名字
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
