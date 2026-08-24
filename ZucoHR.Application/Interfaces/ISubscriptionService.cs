using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;

namespace ZucoHR.Application.Interfaces
{
    public interface ISubscriptionService
    {

        Task<OrganizationSubscription> ActivateFreeTrial(
    Guid organizationId
);
        Task<IEnumerable<SubscriptionResponseDto>> GetOrganizationSubscriptions(Guid organizationId);
        Task VerifyAndActivateSubscription(
    string reference
);
        Task<PaystackInitializeResponse> InitializePayment(
    InitializeSubscriptionPaymentDto dto
);
        Task<InitializeSubscriptionResponseDto> CreateSubscription(
          
            string email,
            CreateSubscriptionDto dto
        );

        Task<bool> CancelSubscription(int subscriptionId);

        Task<SubscriptionResponseDto?> GetActiveSubscription(Guid organizationId);
    }
}
