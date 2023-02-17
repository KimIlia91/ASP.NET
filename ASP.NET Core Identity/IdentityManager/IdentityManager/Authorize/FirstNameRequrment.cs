using Microsoft.AspNetCore.Authorization;

namespace IdentityManager.Authorize
{
    public class FirstNameRequrment : IAuthorizationRequirement
    {
        public string Name { get; set; }

        public FirstNameRequrment(string name)
        {
            Name = name;
        }
    }
}
