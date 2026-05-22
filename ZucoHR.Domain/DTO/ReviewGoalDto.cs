using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class ReviewGoalDto
    {
        public string Title { get; set; } = null!;
        public bool IsCompleted { get; set; }
    }
}
