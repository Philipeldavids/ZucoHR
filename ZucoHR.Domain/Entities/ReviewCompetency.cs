using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class ReviewCompetency
    {
        public Guid Id { get; set; }

        public Guid PerformanceReviewId { get; set; }

        [JsonIgnore]
        public PerformanceReview? PerformanceReview { get; set; }

        public string Label { get; set; } = null!;

        public decimal Score { get; set; }
    }
}
