using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisUsage.Models
{
    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }
}
