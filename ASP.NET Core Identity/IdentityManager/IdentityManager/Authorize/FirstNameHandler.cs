using IdentityManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityManager.Authorize
{
    public class FirstNameHandler : AuthorizationHandler<FirstNameRequrment>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public FirstNameHandler(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FirstNameRequrment requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            var claims = Task.Run(async() => await _userManager.GetClaimsAsync(user)).Result;
            var claim  = claims.FirstOrDefault(c => c.Type == "FirstName");
            if (claim != null)
            {
                if (claim.Value.ToLower().Contains(requirement.Name.ToLower()))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            } 
            return Task.CompletedTask;
        }
    }
}
