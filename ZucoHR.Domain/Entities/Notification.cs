using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public string UserId { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;

        public string Type { get; set; } = "INFO"; // INFO, SUCCESS, WARNING

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
