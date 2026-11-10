using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Domain.DTO
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public UserResponseDto User { get; set; } = null!;
        public OrganizationResponseDto Organization { get; set; } = null!;
    }

    public class UserResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public Guid OrganizationId { get; set; }
    }

    public class OrganizationResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public string CurrencySymbol { get; set; } = null!;
        public string Timezone { get; set; } = null!;
    }
}
