using Microsoft.AspNetCore.Authorization;

namespace IdentityManager
{
    public static class AuthorizedHandler
    {
        public static bool AuthorizedAdminWithClaimsOrSuperAdmin(AuthorizationHandlerContext context)
        {
            var policyHandler = (context.User.HasClaim("Create", "True")
                                 && context.User.HasClaim("Edit", "True")
                                 && context.User.IsInRole("Admin")
                                 && context.User.HasClaim("Delete", "True"))
                || context.User.IsInRole("SuperAdmin");
            return policyHandler;
        }

       public static bool AdminCreateAccessHandler(AuthorizationHandlerContext context)
       {
            var policyHandler = (context.User.HasClaim("Create", "True")
                                 && context.User.IsInRole("Admin")
                                 && !context.User.HasClaim("Edit", "True")
                                 && !context.User.HasClaim("Delete", "True"));
            return policyHandler;
       }
    }
}
