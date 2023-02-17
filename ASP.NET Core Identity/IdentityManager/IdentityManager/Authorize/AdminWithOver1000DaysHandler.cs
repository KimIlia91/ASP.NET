using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IdentityManager.Authorize
{
    public class AdminWithOver1000DaysHandler : AuthorizationHandler<AdminWhenMore1000Days>
    {
        private readonly INumberOfDaysForAccount _numberOfDays;

        public AdminWithOver1000DaysHandler(INumberOfDaysForAccount numberOfDays)
        {
            _numberOfDays = numberOfDays; 
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminWhenMore1000Days requirement)
        {
            if (!context.User.IsInRole("Admin"))
            {
                return Task.CompletedTask;
            }
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int numberOfDays = _numberOfDays.Get(userId);
            if (numberOfDays >= requirement.Days)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
