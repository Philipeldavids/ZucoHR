using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZucoHR.Application.Interfaces;
using ZucoHR.Domain.DTO;
using ZucoHR.Domain.Entities;
using ZucoHR.Infrastructure.Data;

namespace ZucoHR.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ZucoHrDbContext _context;
        private readonly IConfiguration _configuration;

        public SubscriptionService(IConfiguration configuration,ZucoHrDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task VerifyAndActivateSubscription(
    string reference
)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _configuration["Paystack:SecretKey"]
                );

            var response =
                await client.GetAsync(
                    $"https://api.paystack.co/transaction/verify/{reference}"
                );

            dynamic result =
                JsonConvert.DeserializeObject(
                    await response.Content
                        .ReadAsStringAsync()
                );

            if (result.data.status != "success")
                return;

            Guid organizationId =
                result.data.metadata.organizationId;

            int planId =
                result.data.metadata.planId;

            var plan =
                await _context.SubscriptionPlans
                    .FindAsync(planId);

            var subscription = await _context.OrgSubscription.FirstOrDefaultAsync(x => x.PaymentReference == reference);
            subscription.SubscriptionId = planId;

            subscription.StartDate = DateTime.UtcNow;

            subscription.EndDate =
                        DateTime.UtcNow
                            .AddMonths(1);

            subscription.IsActive = true;
            subscription.PaymentConfirmed = true;

            subscription.Amount = plan.Price;
               

            //_context.OrgSubscription
              //  .Update(subscription);

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<SubscriptionResponseDto>>
            GetOrganizationSubscriptions(Guid organizationId)
        {
            return await _context.OrgSubscription
                .Where(x => x.OrganizationId == organizationId)
                .Include(x => x.Plan)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SubscriptionResponseDto
                {
                    Id = x.Id,
                    PlanName = x.Plan.Name,
                    Price = x.Plan.Price,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    PaymentConfirmed = x.PaymentConfirmed,
                    IsActive = x.IsActive,
                    Status = x.Status
                })
                .ToListAsync();
        }

        public async Task<InitializeSubscriptionResponseDto>
            CreateSubscription( string email, CreateSubscriptionDto dto)
        {
                            

             var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.Id == dto.PlanId);

            if (plan == null)
                throw new Exception("Plan not found");

            var currentSubscription =
                await _context.OrgSubscription.FirstOrDefaultAsync(x =>
                        x.OrganizationId == Guid.Parse(dto.OrganizationId) &&
                        x.IsActive
                    );

            if (currentSubscription != null)
            {
                currentSubscription.IsActive = false;
                currentSubscription.Status = "expired";
            }
            InitializeSubscriptionPaymentDto pytdto = new InitializeSubscriptionPaymentDto()
            {
                OrganizationId = Guid.Parse(dto.OrganizationId),
                Email = email,
                PlanId = plan.Id,
            };
            var paymentResponse =
    await InitializePayment(pytdto);
            var subscription = new OrganizationSubscription
            {
                
                OrganizationId = Guid.Parse(dto.OrganizationId),
                SubscriptionId = dto.PlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                PaymentConfirmed= false,
                Status = "active"
            };

            _context.OrgSubscription.Add(subscription);

            await _context.SaveChangesAsync();

            return new InitializeSubscriptionResponseDto
            {
                AuthorizationUrl = paymentResponse.GetProperty("data").GetProperty("authorization_url").GetString(),

                SubscriptionId =
            subscription.Id
            };
        }
        public async Task<JsonElement> InitializePayment(
    InitializeSubscriptionPaymentDto dto
)
        {
            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.Id == dto.PlanId);

            if (plan == null)
                throw new Exception("Plan not found");

            var reference =
                $"SUB-{Guid.NewGuid()}";

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _configuration["Paystack:SecretKey"]
                );

            var payload = new
            {
                email = dto.Email,

                amount =
                    (int)(plan.Price * 100),

                reference,

                callback_url =
                    "http://zucohr.com/payment-success",

                metadata = new
                {
                    organizationId =
                        dto.OrganizationId,

                    planId =
                        dto.PlanId
                }
            };

            var response =
                await client.PostAsJsonAsync(
                    "https://api.paystack.co/transaction/initialize",
                    payload
                );
            var content = await response.Content.ReadAsStringAsync();
            //var result =
            //    await response.Content
            //        .ReadFromJsonAsync<PaystackInitializeResponse>();
            if(!response.IsSuccessStatusCode)
                throw new Exception($"Paystack Init Error: {content}");

            var data = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(content);
            return data;
        }

        public async Task<bool> CancelSubscription(int subscriptionId)
        {
            var subscription =
                await _context.OrgSubscription
                    .FirstOrDefaultAsync(x => x.Id == subscriptionId);

            if (subscription == null)
                return false;

            subscription.IsActive = false;
            subscription.Status = "cancelled";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<SubscriptionResponseDto?>
            GetActiveSubscription(Guid organizationId)
        {
            var subscription = await _context.OrgSubscription
                .Where(x =>
                    x.OrganizationId == organizationId &&
                    x.IsActive
                )
                .Include(x => x.Plan)
                .Select(x => new SubscriptionResponseDto
                {
                    Id = x.Id,
                    PlanName = x.Plan.Name,
                    Price = x.Plan.Price,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive,
                    PaymentConfirmed = x.PaymentConfirmed,
                    Status = x.Status
                    
                })
                .FirstOrDefaultAsync();

            if (subscription == null)
                throw new Exception("Subscription not found");
            return subscription;
        }
    }
}
