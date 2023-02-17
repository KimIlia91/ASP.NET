using Microsoft.AspNetCore.Authorization;

namespace IdentityManager.Authorize
{
    public class AdminWhenMore1000Days : IAuthorizationRequirement
    {
        public int Days { get; set; }

        public AdminWhenMore1000Days(int days)
        {
            Days = days;
        }
    }
}
