using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace CineMatch.Extensions
{
    public static class RateLimiterExtension
    {
        public static IServiceCollection SetupRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter("Fixed", policy =>
                {
                    policy.PermitLimit = 5;
                    policy.Window = TimeSpan.FromSeconds(10);
                    policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    policy.QueueLimit = 0;
                });
            });

            return services;
        }
    }
}
