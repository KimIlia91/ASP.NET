using Microsoft.AspNetCore.Authorization;

namespace IdentityManager.Authorize
{
    public class OnlySuperAdminChacker : AuthorizationHandler<OnlySuperAdminChacker>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            OnlySuperAdminChacker requirement)
        {
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}
